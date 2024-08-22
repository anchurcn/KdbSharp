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
using System.Text;
using KdbSharp.Types;
using System.Text.Json;
using System.Diagnostics;
using KdbSharp.Serialization.Converters;


namespace KdbSharp.Serialization;


public delegate T? ConvertDelegate<T>(KReader reader, KSerializerOptions options);
public delegate void ConvertBackDelegate<T>(KWriter writer, T? value, KSerializerOptions options);

/*
     internal void GetInfo(Type? type, ref PgConverterInfo lastConverterInfo, out bool asObject)
        like Resolve info converter with KdbType and Requested type

    Deserialize api 3 levels:
    * Deserialize KMessage with options
    * Deserialize(body, readerOptions, options) // Split KMessage to body(serialized k object) and readerOptions(endian, encoding...)
    * Deserialize(KReader, options) // Reader contains body and readerOptions, maybe some state
 */
public partial class KSerializer
{
    // Default options
    public static KSerializerOptions DefaultOptions { get; } = new KSerializerOptions();

    #region Deserialize

    [Obsolete("由于 KReadBuffer 的实现，不支持 ReadOnlySpan<byte>，请使用 byte[] 作为参数")]
    public static T Deserialize<T>(ReadOnlySpan<byte> serializedKObj, KReaderOptions readerOptions, KSerializerOptions? options = null)
    {
        // 1. Converter resolver 参考 npgsql 的 实现，因为除了要根据 Requested type，
        // 还要根据读取时实际的 KdbType 来选择 Converter
        // Requested type 如果通过泛型参数传入，可以用 TypeInfo 固化一些信息，减少反射开销
        options ??= DefaultOptions;
        var info = GetTypeInfo<T>(options);
        throw new NotImplementedException();
    }
    public static T? Deserialize<T>(ReadOnlyMemory<byte> data, KReaderOptions readerOptions, KSerializerOptions? options = null)
    {
        options ??= DefaultOptions;
        var info = GetTypeInfo<T>(options);
        var buffer = new KReadBuffer(data, readerOptions);
        return info.Deserialize(buffer.KdbReader);
    }
    public static T Deserialize<T>(KReader reader, KSerializerOptions? options = null)
    {
        var info = GetTypeInfo<T>(options ?? DefaultOptions);
        return info.Deserialize(reader);
    }

    #region Not impl
    // Other params: JsonTypeInfo, JsonSerializerContext, options
    // Type returnType

    public static T Deserialize<T>(Stream data, KSerializerOptions? options = null)
    {
        throw new NotImplementedException();
    }
    public static T Deserialize<T>(KReader jsonReader, KSerializerOptions? options = null, ConvertDelegate<T> convert = null!)
    {
        Deserialize<int>(null, null, (r, o) => default);
        throw new NotImplementedException();
    }
    #endregion
    #endregion

    #region Serialize

    public static int Serialize<T>(Span<byte> output, T value, KSerializerOptions? options = null)
    {
        throw new NotImplementedException();
    }
    public static void Serialize<T>(Stream output, T value, KSerializerOptions? options = null)
    {
        throw new NotImplementedException();
    }
    public static byte[] Serialize<T>(T value, KSerializerOptions? options = null)
    {
        throw new NotImplementedException();
    }
    public static void Serialize<T>(Stream writer, T value, Type kt, JsonSerializerOptions? options = null)
    {
    }
    #endregion

    public static KTypeInfo GetTypeInfo(Type type, KSerializerOptions options)
    {
        return type == KTypeInfo.ObjectType ?
            options.ObjectTypeInfo :
            options.GetTypeInfoForRootType(type);
    }
    public static KTypeInfo<T> GetTypeInfo<T>(KSerializerOptions options)
    {
        return (KTypeInfo<T>)GetTypeInfo(typeof(T), options);
    }

    internal static void Serialize<T>(KWriter writer, T value, KSerializerOptions? options = null)
    {
        options ??= DefaultOptions;
        GetTypeInfo<T>(options).Serialize(writer, value);
    }
    public static void Serialize(KWriter writer, object? value, Type inputType, KSerializerOptions? options = null)
    {
        options ??= DefaultOptions;
        GetTypeInfo(inputType, options).SerializeAsObject(writer, value);
    }
    internal static void Serialize<T>(KWriter writer, T value, ConvertBackDelegate<T> convert, KSerializerOptions? options = null)
    {
        options ??= DefaultOptions;
        convert(writer, value, options);
    }

    internal static void Serialize<T>(KWriter writer, T value, KType kt, KSerializerOptions? options = null)
    {
        options ??= DefaultOptions;
        GetTypeInfo<T>(options).Serialize(writer, value, kt);
    }
}
/*
public class KdbTupleConverter<T> : KTypeConverter<T> where T : ITuple
{
    public override bool CanConvert(Type? t, KType? kt)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type t, KType kt)
    {
        throw new NotImplementedException();
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        // 来到这里一定是此 Converter 能处理的类型，外部应该做到正确派遣的工作
        // 这里再次检查类型是否匹配，是为了保证 Converter 的正确性
        if (reader.CurrentReadType != KType.GeneralList)
        {
            throw new InvalidOperationException($"Expected GeneralList, but got {reader.CurrentReadType}");
        }
        if (typeof(T) == typeof(ValueTuple<string, string>))
        {
            ValueTuple<string, string> res = default;
            KTypeInfo<string> kdbTypeInfo = new KTypeInfo<string>();
            res.Item1 = kdbTypeInfo.Deserialize(reader);
            res.Item2 = kdbTypeInfo.Deserialize(reader);
        }
        return default;
    }

    public override void Write(KWriter writer, T value)
    {
        throw new NotImplementedException();
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
*/
