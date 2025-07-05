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

public delegate T? DeserializeHandler<T>(ref KSerializationReader reader, KSerializerOptions options);
public delegate void SerializeHandler<T>(ref KSerializationWriter writer, T? value, KSerializerOptions options);

public partial class KSerializer
{
    // Default options
    public static KSerializerOptions DefaultOptions { get; } = new KSerializerOptions();

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
    
    #region Deserialize

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
        return deserializeHandler(ref reader, options ?? DefaultOptions);
    }

    public static object? Deserialize(ref KSerializationReader reader, Type outputType, KSerializerOptions? options = null)
    {
        var info = GetTypeInfo(outputType, options ?? DefaultOptions);
        return info.DeserializeAsObject(ref reader);
    }

    public static object? Deserialize(ref KSerializationReader reader, DeserializeHandler<object> deserializeHandler, KSerializerOptions? options = null)
    {
        return deserializeHandler(ref reader, options ?? DefaultOptions);
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
    /// <summary>
    /// Serializes a value using the provided KSerializationWriter with a specific target KType.
    /// </summary>
    public static void Serialize<T>(ref KSerializationWriter writer, T? value, KType targetType, KSerializerOptions? options = null)
    {
        var info = GetTypeInfo<T>(options ?? DefaultOptions);
        info.Serialize(ref writer, value, targetType);
    }

    /// <summary>
    /// Serializes a value using a custom serialize handler.
    /// </summary>
    public static void Serialize<T>(ref KSerializationWriter writer, T? value, SerializeHandler<T> serializeHandler, KSerializerOptions? options = null)
    {
        serializeHandler(ref writer, value, options ?? DefaultOptions);
    }

    /// <summary>
    /// Serializes an object value using its runtime type.
    /// </summary>
    public static void Serialize(ref KSerializationWriter writer, object? value, Type inputType, KSerializerOptions? options = null)
    {
        var info = GetTypeInfo(inputType, options ?? DefaultOptions);
        info.SerializeAsObject(ref writer, value);
    }

    /// <summary>
    /// Serializes an object value using its runtime type with a specific target KType.
    /// </summary>
    public static void Serialize(ref KSerializationWriter writer, object? value, Type inputType, KType targetType, KSerializerOptions? options = null)
    {
        var info = GetTypeInfo(inputType, options ?? DefaultOptions);
        info.SerializeAsObject(ref writer, value, targetType);
    }
    
    /// <summary>
    /// Serializes a value to a byte array using the struct-based KSerializationWriter.
    /// </summary>
    public static byte[] Serialize<T>(T? value, KType? targetType, KSerializerOptions? options = null)
    {
        options ??= DefaultOptions;
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new KSerializationWriter(buffer)
        {
            TextEncoding = options.TextEncoding,
            ProtocolVersion = KConstant.ClientProtocolVersion,
            CancellationToken = default,
        };

        var info = GetTypeInfo<T>(options);
        info.Serialize(ref writer, value, targetType);
        writer.Flush();

        return buffer.WrittenSpan.ToArray();
    }

    /// <summary>
    /// Serializes a value to a stream.
    /// </summary>
    public static void Serialize<T>(Stream output, T? value, KSerializerOptions? options = null)
    {
        options ??= DefaultOptions;
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new KSerializationWriter(buffer)
        {
            TextEncoding = options.TextEncoding,
            ProtocolVersion = KConstant.ClientProtocolVersion,
            CancellationToken = default,
        };

        var info = GetTypeInfo<T>(options);
        info.Serialize(ref writer, value);
        writer.Flush();

        output.Write(buffer.WrittenSpan);
    }

    #endregion
}
