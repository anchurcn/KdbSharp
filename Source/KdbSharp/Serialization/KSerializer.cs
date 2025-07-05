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
using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using KdbSharp.Serialization.Converters;
using KdbSharp.Types;


namespace KdbSharp.Serialization;

/// <summary>
/// A buffer writer that writes to a fixed-size memory.
/// </summary>
internal class FixedSizeBufferWriter : IBufferWriter<byte>
{
    private readonly Memory<byte> _buffer;
    private int _written;

    public FixedSizeBufferWriter(Memory<byte> buffer)
    {
        _buffer = buffer;
        _written = 0;
    }

    public int WrittenCount => _written;

    public void Advance(int count)
    {
        if (count < 0 || _written + count > _buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        _written += count;
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        if (sizeHint < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeHint));
        }

        var remaining = _buffer.Length - _written;
        if (sizeHint > remaining)
        {
            throw new InvalidOperationException("Not enough space in buffer");
        }

        return _buffer.Slice(_written, remaining);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        if (sizeHint < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeHint));
        }

        var remaining = _buffer.Length - _written;
        if (sizeHint > remaining)
        {
            throw new InvalidOperationException("Not enough space in buffer");
        }

        return _buffer.Span.Slice(_written, remaining);
    }
}



// SerializeHandler / DeserializeHandler - Struct-based API
public delegate T? DeserializeHandler<T>(ref KSerializationReader reader, KSerializerOptions options);
public delegate void SerializeHandler<T>(ref KSerializationWriter writer, T? value, KSerializerOptions options);

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

    //[Obsolete("由于 KReadBuffer 的实现，不支持 ReadOnlySpan<byte>，请使用 byte[] 作为参数")]
    //public static T Deserialize<T>(ReadOnlySpan<byte> serializedKObj, KReaderOptions readerOptions, KSerializerOptions? options = null)
    //{
    //    // 1. Converter resolver 参考 npgsql 的 实现，因为除了要根据 Requested type，
    //    // 还要根据读取时实际的 KdbType 来选择 Converter
    //    // Requested type 如果通过泛型参数传入，可以用 TypeInfo 固化一些信息，减少反射开销
    //    options ??= DefaultOptions;
    //    var info = GetTypeInfo<T>(options);
    //    throw new NotImplementedException();
    //}
    /// <summary>
    /// Deserializes data using the struct-based KSerializationReader.
    /// </summary>
    public static T? Deserialize<T>(ReadOnlyMemory<byte> data, KSerializerOptions? options = null)
    {
        options ??= DefaultOptions;
        var info = GetTypeInfo<T>(options);
        var reader = new KSerializationReader(data)
        {
            TextEncoding = options.TextEncoding,
            ProtocolVersion = KConstant.ClientProtocolVersion
        };
        return info.Deserialize(ref reader);
    }

    /// <summary>
    /// Deserializes data using the struct-based KSerializationReader.
    /// </summary>
    public static T? Deserialize<T>(ref KSerializationReader reader, KSerializerOptions? options = null)
    {
        var info = GetTypeInfo<T>(options ?? DefaultOptions);
        return info.Deserialize(ref reader);
    }

    public static T? Deserialize<T>(ref KSerializationReader reader, DeserializeHandler<T> deserializeHandler, KSerializerOptions? options = null)
    {
        var info = GetTypeInfo<T>(options ?? DefaultOptions);
        return info.Deserialize(ref reader);
    }

    public static object? Deserialize(ref KSerializationReader reader, Type outputType, KSerializerOptions? options = null)
    {
        return default;
    }
    public static object? Deserialize(ref KSerializationReader reader, DeserializeHandler<object> deserializeHandler, KSerializerOptions? options = null)
    {
        return default;
    }

    #endregion

    #region Serialize

    /// <summary>
    /// Serializes a value using the provided KSerializationWriter.
    /// </summary>
    public static void Serialize<T>(ref KSerializationWriter writer, T? value, KSerializerOptions? options = null)
    {
        var info = GetTypeInfo<T>(options ?? DefaultOptions);
        info.Serialize(ref writer, value);
    }
    public static void Serialize<T>(ref KSerializationWriter writer, T? value, KType targetType, KSerializerOptions? options = null)
    {
    }
    public static void Serialize<T>(ref KSerializationWriter writer, T? value, SerializeHandler<T> serializeHandler, KSerializerOptions? options = null)
    {
    }
    public static void Serialize(ref KSerializationWriter writer, object? value, Type inputType, KSerializerOptions? options = null)
    {
    }
    public static void Serialize(ref KSerializationWriter writer, object? value,Type inputType, KType targetType, KSerializerOptions? options = null)
    {
    }
    
    /// <summary>
    /// Serializes a value to a byte array using the struct-based KSerializationWriter.
    /// </summary>
    public static byte[] Serialize<T>(T? value, KSerializerOptions? options = null)
    {
        options ??= DefaultOptions;
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new KSerializationWriter(buffer)
        {
            TextEncoding = options.TextEncoding,
            ProtocolVersion = KConstant.ClientProtocolVersion
        };

        var info = GetTypeInfo<T>(options);
        info.Serialize(ref writer, value);
        writer.Flush();

        return buffer.WrittenSpan.ToArray();
    }

    /// <summary>
    /// Serializes a value to a stream.
    /// </summary>
    public static void Serialize<T>(Stream output, T? value, KSerializerOptions? options = null)
    {
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new KSerializationWriter(buffer)
        {
            TextEncoding = options?.TextEncoding ?? Encoding.UTF8,
            ProtocolVersion = KConstant.ClientProtocolVersion
        };

        var info = GetTypeInfo<T>(options ?? DefaultOptions);
        info.Serialize(ref writer, value);
        writer.Flush();

        output.Write(buffer.WrittenSpan);
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
}
