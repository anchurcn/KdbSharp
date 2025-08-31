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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using KdbSharp.Extensions;
using KdbSharp.Types;

namespace KdbSharp.Serialization;

/// <summary>
/// Base class for type information used in KDB serialization/deserialization.
/// Manages type converters and provides abstract methods for serialization operations.
/// </summary>
public abstract class KTypeInfo
{
    /// <summary>
    /// Gets the object type used for dynamic type handling.
    /// </summary>
    public static Type ObjectType { get; } = typeof(object);

    /// <summary>
    /// Gets the serializer options associated with this type info.
    /// </summary>
    public KSerializerOptions Options { get; }

    /// <summary>
    /// Gets the dictionary of converters for specific KTypes.
    /// </summary>
    public Dictionary<KType, KTypeConverter?> Converters { get; } = new();

    /// <summary>
    /// Gets the lazy-loaded default converter for this type.
    /// </summary>
    public Lazy<KTypeConverter?> DefaultConverter { get; private set; }

    /// <summary>
    /// Initializes a new instance of the KTypeInfo class with the specified options.
    /// </summary>
    /// <param name="options">The serializer options to use.</param>
    public KTypeInfo(KSerializerOptions options)
    {
        Options = options;
        DefaultConverter = new(() => Options.GetConverter(Type));
    }

    /// <summary>
    /// Gets the .NET type that this type info represents.
    /// </summary>
    public abstract Type Type { get; }

    /// <summary>
    /// Deserializes an object from the reader without type safety.
    /// </summary>
    /// <param name="reader">The serialization reader.</param>
    /// <returns>The deserialized object.</returns>
    public abstract object? DeserializeAsObject(ref KSerializationReader reader);

    /// <summary>
    /// Serializes an object to the writer without type safety.
    /// </summary>
    /// <param name="writer">The serialization writer.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="kt">Optional target KType for serialization.</param>
    public abstract void SerializeAsObject(ref KSerializationWriter writer, object? value, KType? kt = null);

    /// <summary>
    /// Tries to get a converter for the specified KType.
    /// Resolves from Options if not exists in cache, then adds to cache.
    /// If key exists but value is null, returns false.
    /// </summary>
    /// <param name="kt">The KType to get converter for.</param>
    /// <param name="converter">The converter if found.</param>
    /// <returns>True if converter was found and is not null.</returns>
    public bool TryGetConverter(KType kt, [NotNullWhen(true)] out KTypeConverter? converter)
    {
        if (Converters.TryGetValue(kt, out converter))
        {
            return converter is not null;
        }
        else
        {
            converter = Options.GetConverter(Type, kt);
            Converters.Add(kt, converter);
            return converter is not null;
        }
    }

    public bool TryGetDefaultKTypeConverter(KType targetType, out KTypeConverter? converter)
    {
        if(Converters.TryGetValue(targetType, out converter))
        {
            return converter is not null;
        }
        else
        {
            converter = Options.GetConverter(targetType);
            Converters.Add(targetType, converter);
            return converter is not null;
        }
    }

    /// <summary>
    /// Tries to get a typed converter for the specified KType.
    /// </summary>
    /// <typeparam name="T">The target type for the converter.</typeparam>
    /// <param name="t">The KType to get converter for.</param>
    /// <param name="converter">The typed converter if found.</param>
    /// <returns>True if converter was found and is of the correct type.</returns>
    public bool TryGetConverter<T>(KType t, [NotNullWhen(true)] out KTypeConverter<T>? converter)
    {
        if (TryGetConverter(t, out var c) && c is KTypeConverter<T> typedConverter)
        {
            converter = typedConverter;
            return true;
        }
        else
        {
            converter = null;
            return false;
        }
    }

    /// <summary>
    /// Tries to get the default converter for this type.
    /// </summary>
    /// <param name="converter">The default converter if found.</param>
    /// <returns>True if default converter exists and is not null.</returns>
    public bool TryGetConverter([NotNullWhen(true)] out KTypeConverter? converter)
    {
        converter = DefaultConverter.Value;
        return converter is not null;
    }

    /// <summary>
    /// Tries to get the default typed converter for this type.
    /// </summary>
    /// <typeparam name="T">The target type for the converter.</typeparam>
    /// <param name="converter">The typed default converter if found.</param>
    /// <returns>True if default converter exists and is of the correct type.</returns>
    public bool TryGetConverter<T>([NotNullWhen(true)] out KTypeConverter<T>? converter)
    {
        converter = (KTypeConverter<T>?)DefaultConverter.Value;
        return converter is not null;
    }
}

/// <summary>
/// Generic type information for KDB serialization/deserialization of type T.
/// Provides type-safe serialization operations and converter management.
/// </summary>
/// <typeparam name="T">The type this type info represents.</typeparam>
public class KTypeInfo<T> : KTypeInfo
{
    /// <summary>
    /// Initializes a new instance of the KTypeInfo&lt;T&gt; class.
    /// </summary>
    /// <param name="options">The serializer options to use.</param>
    public KTypeInfo(KSerializerOptions options) : base(options)
    {
    }
    /// <summary>
    /// Gets the .NET type that this type info represents.
    /// </summary>
    public override Type Type => typeof(T);

    /// <summary>
    /// Deserializes a value of type T from the reader.
    /// Handles various KTypes including Unit, Error, and type-specific conversions.
    /// </summary>
    /// <param name="reader">The serialization reader.</param>
    /// <returns>The deserialized value of type T.</returns>
    public virtual T? Deserialize(ref KSerializationReader reader)
    {
        var res = default(T);
        var kt = reader.NextTypeStamp;

        // 如果有显式的 Converter，就用它，否则使用默认行为
        // 例如如果请求类型是 Unit，会走显式的 Converter
        if (TryGetConverter<T>(kt, out var converter))
        {
            res = converter.Read(ref reader, Options);
        }
        else
        {
            if (kt == KType.Error)
            {
                var error = ((KTypeInfo<KdbException>)Options.GetTypeInfoForRootType(typeof(KdbException)))
                    .Deserialize(ref reader);
                throw error!;
            }
            else
            {
                ThrowHelper.ThrowConversionNotSupport(Type, kt);
            }
        }

        return res;
    }

    /// <summary>
    /// Deserializes an object from the reader without type safety.
    /// </summary>
    /// <param name="reader">The serialization reader.</param>
    /// <returns>The deserialized object.</returns>
    public override object? DeserializeAsObject(ref KSerializationReader reader) => Deserialize(ref reader);

    /// <summary>
    /// Serializes a value of type T to the writer.
    /// Handles object type delegation, null values, and converter selection.
    /// </summary>
    /// <param name="writer">The serialization writer.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="kType">Optional target KType for serialization.</param>
    public virtual void Serialize(ref KSerializationWriter writer, T? value, KType? kType = null)
    {

        if (kType is null && TryGetConverter<T>(out var defaultConverter))
        {
            defaultConverter.Write(ref writer, value, Options);
        }
        else if (kType is KType kt && TryGetConverter<T>(kt, out var converter))
        {
            converter.Write(ref writer, value, Options);
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

    /// <summary>
    /// Serializes an object to the writer without type safety.
    /// </summary>
    /// <param name="writer">The serialization writer.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="kt">Optional target KType for serialization.</param>
    public override void SerializeAsObject(ref KSerializationWriter writer, object? value, KType? kt = null) => Serialize(ref writer, (T?)value, kt);
}

/// <summary>
/// Type information for dynamic object serialization/deserialization.
/// Handles runtime type resolution and converter selection based on actual object types.
/// </summary>
public class ObjectTypeInfo : KTypeInfo<object>
{
    /// <summary>
    /// Initializes a new instance of the ObjectTypeInfo class.
    /// </summary>
    /// <param name="options">The serializer options to use.</param>
    public ObjectTypeInfo(KSerializerOptions options) : base(options)
    {
    }
    /// <summary>
    /// Gets the object type (always returns typeof(object)).
    /// </summary>
    public override Type Type => ObjectType;

    /// <summary>
    /// Deserializes an object by reading the type stamp and using the appropriate converter.
    /// </summary>
    /// <param name="reader">The serialization reader.</param>
    /// <returns>The deserialized object.</returns>
    public override object? Deserialize(ref KSerializationReader reader)
    {
        var nextTypeStamp = reader.NextTypeStamp;
        if (TryGetDefaultKTypeConverter(nextTypeStamp, out var converter))
        {
            var value = converter.ReadAsObject(ref reader, Options);
            if (value is KdbException exception)
            {
                throw exception;
            }

            return value;
        }
        else
        {
            throw new KSerializationException($"Converter of KType {nextTypeStamp} not found.");
        }
    }

    /// <summary>
    /// Serializes an object by determining its runtime type and using the appropriate converter.
    /// </summary>
    /// <param name="writer">The serialization writer.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="kType">Optional target KType for serialization.</param>
    public override void Serialize(ref KSerializationWriter writer, object? value, KType? kType = null)
    {
        if (kType is null)
        {
            if (value is null)
            {
                // TODO: Atom.Null for AtomList, Unit for others.
                throw new NotImplementedException("TODO: write proper null value for writing container type present.");
            }
            else
            {
                Options.GetTypeInfo(value.GetType()).SerializeAsObject(ref writer, value);
            }
        }
        else
        {
            if (value is null)
            {
                // TODO: GetConverter of kType, let it write the proper 'null' value
                // for the kType,
                // 这要求 Converter 实现 WriteAsObject，当传入 null 则写入 null 值，
                // 比如 KIntConverter 写 Kint.Null，BooleanConverter 抛出异常
                // 其他写 UnaryPrimitive.Unit
                throw new NotImplementedException("TODO: write proper null value for kType.");
            }
            else
            {
                Options.GetTypeInfo(value.GetType()).SerializeAsObject(ref writer, value, kType);
            }
        }
    }
}
