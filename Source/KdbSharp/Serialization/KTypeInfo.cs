/*
 Copyright (C) 2024 Anchur
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
*/
using KdbSharp.Extensions;
using KdbSharp.Types;
using System.Diagnostics.CodeAnalysis;

namespace KdbSharp.Serialization;


public abstract class KTypeInfo
{
    public static Type ObjectType { get; } = typeof(object);

    public KSerializerOptions Options { get; }

    public Dictionary<KType, KTypeConverter?> Converters { get; } = new();
    public Lazy<KTypeConverter?> DefaultConverter { get; private set; }

    public KTypeInfo(KSerializerOptions options)
    {
        Options = options;
        DefaultConverter = new(() => Options.GetConverter(Type));
    }
    //public KType KType { get; set; } // TODO: remove

    public abstract Type Type { get; }
    public abstract object? DeserializeAsObject(KReader reader);
    public abstract void SerializeAsObject(KWriter reader, object? value, KType? kt = null);

    // Resolve from Options if not exists key, then add to cache.
    // If key exists, but value is null, then return false.
    public bool TryGetConverter(KType kt, [NotNullWhen(true)] out KTypeConverter? converter)
    {
        if (Converters.TryGetValue(kt, out converter))
        {
            return converter is not null;
        }
        else
        {
            if (Type == ObjectType)
            {
                converter = Options.GetConverter(kt);
                if (converter is not null)
                    converter = new ShimObjectConverter(converter);
            }
            else
            {
                converter = Options.GetConverter(Type, kt);
            }
            Converters.Add(kt, converter);
            return converter is not null;
        }
    }
    public bool TryGetConverter<T>(KType t, [NotNullWhen(true)] out KTypeConverter<T>? converter)
    {
        if (TryGetConverter(t, out var c))
        {
            converter = (KTypeConverter<T>)c!;
            return true;
        }
        else
        {
            converter = null;
            return false;
        }
    }
    public bool TryGetConverter([NotNullWhen(true)] out KTypeConverter? converter)
    {
        converter = DefaultConverter.Value;
        return converter is not null;
    }
    public bool TryGetConverter<T>([NotNullWhen(true)] out KTypeConverter<T>? converter)
    {
        converter = (KTypeConverter<T>?)DefaultConverter.Value;
        return converter is not null;
    }
}
public class KTypeInfo<T> : KTypeInfo
{
    public KTypeInfo(KSerializerOptions options) : base(options)
    {
    }

    public override Type Type => typeof(T);

    public T? Deserialize(KReader reader)
    {
        var res = default(T);
        var kt = reader.BeginReadType();

        #region Fast path

        //var lastChildKt = reader.LastNestedType;
        //if (lastChildKt is not null && kt == lastChildKt)
        //{
        //    res = reader.GetLastNestedTypeConverter<T>().Read(reader, Options);
        //}
        #endregion

        // 如果有显式的 Converter，就用它，否则使用默认行为
        // 例如如果请求类型是 Unit，会走显式的 Converter
        if (TryGetConverter<T>(kt, out var converter))
        {
            res = converter.Read(reader, Options);
        }
        else
        {
            // 默认行为（宽松模式）：
            // 如果请求的类型是可空的值类型，可以接受 Unit 作为 null （意味着 Unit 和可视为空的 Atom 会被解释为 null，round trip 可能不对称）
            // 如果是值类型，但不可空，应该抛出异常
            if (kt == KType.Unit)
            {
                if (Options.NullHandleStategy == NullHandleStrategy.Default)
                {
                    _ = Options.GetConverterByTypeInfo<KUnit>(kt)?.Read(reader, Options)
                        ?? throw new InvalidOperationException("No converter registered for Unit type.");
                    if (Type.IsValueType)
                    {
                        if (Type.IsNullable())
                        {
                            res = default;
                        }
                        else
                        {
                            throw new InvalidOperationException("Cannot read unit (::) to non-nullable value type.");
                        }
                    }
                    else
                    {
                        res = default;
                    }
                }
            }
            else if (kt == KType.Error)
            {
                var error = Options.GetConverterByTypeInfo<KdbException>(kt)?.Read(reader, Options)
                    ?? throw new InvalidOperationException("No converter registered for Error type.");
                throw error;
            }
            else
            {
                ThrowHelper.ThrowConversionNotSupport(Type, kt);
            }
        }

        reader.EndReadType();
        return res;
    }

    public override object? DeserializeAsObject(KReader reader)
    {
        return Deserialize(reader);
    }


    public void Serialize(KWriter writer, T? value, KType? kType = null)
    {
        // Delegate to actual type's type info.
        if (Type == ObjectType && value is not null)
        {
            Options.GetTypeInfoForRootType(value.GetType()).SerializeAsObject(writer, value, kType);
            return;
        }
        else if (Type == ObjectType && value is null)
        {
            throw new NotImplementedException();
        }

        if (kType is null && TryGetConverter<T>(out var defaultConverter))
        {
            defaultConverter.Write(writer, value!, Options);
        }
        else if (kType is KType kt && TryGetConverter<T>(kt, out var converter))
        {
            converter.Write(writer, value!, Options);
        }
        else
        {
            if (kType is null)
            {
                ThrowHelper.ThrowNoDefaultConversionForType(Type);
            }
            else
            {
                ThrowHelper.ThrowConversionNotSupport(Type, kType.GetValueOrDefault());
            }
        }
    }

    public override void SerializeAsObject(KWriter writer, object? value, KType? kt = null)
    {
        Serialize(writer, (T?)value, kt);
    }
}

public class ShimObjectConverter : KTypeConverter<object>
{
    public ShimObjectConverter(KTypeConverter converter)
    {
        Converter = converter;
    }
    public KTypeConverter Converter { get; }

    public override bool CanConvert(Type t, KType kt)
    {
        throw new NotSupportedException();
    }

    public override object Read(KReader reader, KSerializerOptions options)
    {
        return Converter.ReadAsObject(reader, options)!;
    }

    public override void Write(KWriter writer, object value, KSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}
