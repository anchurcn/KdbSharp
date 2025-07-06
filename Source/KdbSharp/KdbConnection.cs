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
using KdbSharp.Serialization;
using KdbSharp.Types;
using System.Buffers;
using System.Reflection;

namespace KdbSharp;

public class KdbConnection : KdbConnectionBase
{

    public KdbConnection(KdbConnectionOptions options) : base(options)
    {
    }
    private readonly ArrayBufferWriter<byte> _bufferWriter = new();

    // Reader/Writer holds a buffer, and KMessage can just reference the buffer by Memory<byte>(a view of part of the buffer).
    public async Task<TResult?> GetAsync<TResult>(string expr, KSerializerOptions? options = null, CancellationToken cancellation = default)
    {
        await SendQueryObjectAsync(expr, MessageType.Request, options, cancellation);
        return await RecvResponseObjectAsync<TResult>(options, cancellation);
    }

    public Task SetAsync(string expr, CancellationToken cancellation = default)
    {
        // Serialize and send an async KMessage.
        return SendQueryObjectAsync(expr, MessageType.Async, null, cancellation);
    }

    #region CreateCommand

    static KdbCommand<T> CreateCommandInternal<T>(T parameterizedQuery, KdbConnection connection)
        where T : struct
    {
        return new KdbCommand<T>(parameterizedQuery, connection);
    }

    public KdbCommand<(string, TArg1)> CreateCommand<TArg1>(string func, TArg1 arg1)
    {
        return CreateCommandInternal((func, arg1), this);
    }

    public KdbCommand<(string, TArg1, TArg2)> CreateCommand<TArg1, TArg2>(string func, TArg1 arg1, TArg2 arg2)
    {
        return CreateCommandInternal((func, arg1, arg2), this);
    }

    public KdbCommand<(string, TArg1, TArg2, TArg3)> CreateCommand<TArg1, TArg2, TArg3>(string func, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        return CreateCommandInternal((func, arg1, arg2, arg3), this);
    }

    public KdbCommand<(string, TArg1, TArg2, TArg3, TArg4)> CreateCommand<TArg1, TArg2, TArg3, TArg4>(string func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
    {
        return CreateCommandInternal((func, arg1, arg2, arg3, arg4), this);
    }

    public KdbCommand<(string, TArg1, TArg2, TArg3, TArg4, TArg5)> CreateCommand<TArg1, TArg2, TArg3, TArg4, TArg5>(string func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
    {
        return CreateCommandInternal((func, arg1, arg2, arg3, arg4, arg5), this);
    }

    public KdbCommand<(string, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6)> CreateCommand<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(string func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
    {
        return CreateCommandInternal((func, arg1, arg2, arg3, arg4, arg5, arg6), this);
    }

    public KdbCommand<(string, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7)> CreateCommand<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(string func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
    {
        return CreateCommandInternal((func, arg1, arg2, arg3, arg4, arg5, arg6, arg7), this);
    }

    public KdbCommand<(string, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8)> CreateCommand<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(string func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
    {
        return CreateCommandInternal((func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), this);
    }
    #endregion

#region Others
    

    // Where T is ValueTuple and if first is string then serialize as KType.CharList.
    class ParameterizedQueryConverter<T>
    {
        static readonly FieldInfo[] _orderedTupleFields;
        static ParameterizedQueryConverter()
        {
            var tupleType = typeof(T);
            var tupleFields = tupleType.GetFields();
            // By convention, the field name is Item<n> where n is the 1-based index of the field.
            // The fields are ordered by the declaration order, but we order here explicitly by the field name.
            _orderedTupleFields = tupleFields.OrderBy(f => f.Name).ToArray();
        }

        public static void Write(ref KSerializationWriter writer, T value, KSerializerOptions options)
        {
            writer.StartWriteList(KType.GeneralList, _orderedTupleFields.Length);
            KSerializer.Serialize(ref writer, (string)_orderedTupleFields[0].GetValue(value)!, KType.CharList, options);
            for (int i = 1; i < _orderedTupleFields.Length; i++)
            {
                var field = _orderedTupleFields[i];
                var item = field.GetValue(value);
                KSerializer.Serialize(ref writer, item, field.FieldType, options);
            }
            writer.EndWriteList();
        }
    }
    public async Task<T?> RecvResponseObjectAsync<T>(KSerializerOptions? options, CancellationToken cancellation)
    {
        var message = await RecvAsync(cancellation);
        if (message.Type != MessageType.Response)
        {
            throw new InvalidOperationException($"Unexpected message type {message.Type}.");
        }
        if (message.Compressed)
        {
            var uncompressed = message;
            message = KMessage.Uncompress(message);
            uncompressed.Dispose();
        }
        return DeserializeMessage<T>(message, options);
    }

    private static T? DeserializeMessage<T>(KMessage message, KSerializerOptions? options)
    {
        var reader = new KSerializationReader(message.Body)
        {
            IsLittleEndian = message.IsLittleEndian,
        };
        return KSerializer.Deserialize<T>(ref reader, options);
    }
    public async Task SendQueryObjectAsync(string expr, MessageType messageType, KSerializerOptions? options, CancellationToken cancellation)
    {
        var writer = new KSerializationWriter(_bufferWriter);
        KSerializer.Serialize(ref writer, expr, KType.CharList, options);
        await SendAsync(_bufferWriter.WrittenMemory, messageType, writer.IsLittleEndian, false, cancellation).ConfigureAwait(false);
        _bufferWriter.Clear();
    }
    public async Task SendParameterizedQueryObjectAsync<TQuery>(TQuery parameterizedQuery, MessageType messageType, KSerializerOptions? options, CancellationToken cancellation)
        where TQuery : struct
    {
        var writer = new KSerializationWriter(_bufferWriter);
        KSerializer.Serialize(ref writer, parameterizedQuery, ConvertParameterizedQuery, options);
        await SendAsync(_bufferWriter.WrittenMemory, messageType, writer.IsLittleEndian, false, cancellation).ConfigureAwait(false);
        _bufferWriter.Clear();

        static void ConvertParameterizedQuery(ref KSerializationWriter writer, TQuery value, KSerializerOptions options)
        {
            var type = typeof(TQuery);
            // Check if T is ValueTuple and the first field is string.
            if (type.Name.Contains(nameof(ValueTuple)) && type.GetField("Item1")?.FieldType == typeof(string))
            {
                ParameterizedQueryConverter<TQuery>.Write(ref writer, value, options);
            }
            else
            {
                throw new InvalidOperationException("Not a parameterized query.");
            }
        }
    }
    public async Task SendMessage(ReadOnlyMemory<byte> uncompressedBody, MessageType type, bool isLittleEndian, CancellationToken cancellation = default)
    {
        // TODO: compress body if needed
        await SendAsync(uncompressedBody, type, isLittleEndian, compressed: false, cancellation).ConfigureAwait(false);
    }

    private async Task SendAsync(ReadOnlyMemory<byte> body, MessageType type, bool isLittleEndian, bool compressed, CancellationToken cancellation)
    {
        // Create a KMessage with the provided body data
        var endianess = isLittleEndian ? Endianess.LittleEndian : Endianess.BigEndian;
        var message = KMessage.Alloc(type, endianess, compressed, KMessage.HEADER_SIZE + body.Length);

        // Copy the body data to the message
        body.CopyTo(message.Body);

        try
        {
            // Send the message using the base class method
            await SendAsync(message, cancellation).ConfigureAwait(false);
        }
        finally
        {
            // Dispose the message to free the allocated memory
            message.Dispose();
        }
    }

#endregion

}

// For parameterized query
public readonly struct KdbCommand<T>
    where T : struct
{
    private readonly T _parameterizedQuery;
    private readonly KdbConnection _connection;
    public KdbCommand(T parameterizedQuery, KdbConnection connection)
    {
        _parameterizedQuery = parameterizedQuery;
        _connection = connection;
    }

    public async Task<TResult?> GetAsync<TResult>(KSerializerOptions? options = null, CancellationToken cancellation = default)
    {
        await _connection.SendParameterizedQueryObjectAsync(_parameterizedQuery, MessageType.Request, options, cancellation);
        return await _connection.RecvResponseObjectAsync<TResult>(options, cancellation);
    }

    public Task<object?> GetAsync(KSerializerOptions? options = null, CancellationToken cancellation = default)
    {
        return GetAsync<object?>(options, cancellation);
    }
    public Task SetAsync(KSerializerOptions? options = null, CancellationToken cancellation = default)
    {
        return _connection.SendParameterizedQueryObjectAsync(_parameterizedQuery, MessageType.Async, options, cancellation);
    }
}
