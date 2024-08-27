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
using System.Reflection;
using System.Runtime.CompilerServices;

namespace KdbSharp;

public readonly struct KdbCommand<T>
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
        await _connection.SendParameterizedRequestAsync(_parameterizedQuery, options, cancellation);
        return await _connection.RecvResponseObjectAsync<TResult>(options, cancellation);
    }

    public Task SetAsync(KSerializerOptions? options = null, CancellationToken cancellation = default)
    {
        return _connection.SendParameterizedAsyncAsync(_parameterizedQuery, options, cancellation);
    }
}

public class KdbConnection : KdbConnectionBase
{
    #region Write

    private readonly KWriteBuffer _writeBuffer;
    private KWriter Writer => _writeBuffer.Writer;
    private readonly KMessage _messageWrite = KMessage.CreateReusable();
    #endregion

    #region Read

    private readonly KReaderOptions _readerOptions;
    #endregion

    public KdbConnection(KdbConnectionOptions options, KReaderOptions? readerOptions = null) : base(options)
    {
        _readerOptions = readerOptions ?? new KReaderOptions()
        {
            TextEncoding = TextEncoding,
            IsLittleEndian = true
        };
        _writeBuffer = new KWriteBuffer(new KWriterOptions() { ProtocolVersion = ProtocolVersion });
    }

    public async Task SendRequestAsync(string expr, KSerializerOptions? options, CancellationToken cancellation)
    {
        KSerializer.Serialize(Writer, expr, KType.CharList, options);
        _messageWrite.RepackMessage(MessageType.Request, Writer.Buffer.IsLittleEndian, Writer.Buffer.GetWritedMemory());
        await SendAsync(_messageWrite, cancellation);
        Writer.Buffer.Clear();
    }
    
    // Where T is ValueTuple and if first is string then serialize as KType.CharList.
    class ParameterizedQueryConverter<T>
    {
        static readonly FieldInfo[] _orderedTupleFields;
        static ParameterizedQueryConverter()
        {
            var tupleType =typeof(T);
            var tupleFields = tupleType.GetFields();
            // By convention, the field name is Item<n> where n is the 1-based index of the field.
            // The fields are ordered by the declaration order, but we order here explicitly by the field name.
            _orderedTupleFields = tupleFields.OrderBy(f => f.Name).ToArray();
        }

        public static void Write(KWriter writer, T value, KSerializerOptions options)
        {
            writer.WriteStartList(KType.GeneralList, _orderedTupleFields.Length);
            KSerializer.Serialize(writer, (string)_orderedTupleFields[0].GetValue(value)!, KType.CharList, options);
            for (int i = 1; i < _orderedTupleFields.Length; i++)
            {
                var field = _orderedTupleFields[i];
                var item = field.GetValue(value);
                KSerializer.Serialize(writer, item, field.FieldType, options);
            }
            writer.WriteEndList();
        }
    }
    internal async Task SendParameterizedRequestAsync<T>(T value, KSerializerOptions? options, CancellationToken cancellation)
    {
        KSerializer.Serialize(Writer, value, ConvertParameterizedQuery, options);
        _messageWrite.RepackMessage(MessageType.Request, Writer.Buffer.IsLittleEndian, Writer.Buffer.GetWritedMemory());
        await SendAsync(_messageWrite, cancellation);
        Writer.Buffer.Clear();
        
        static void ConvertParameterizedQuery(KWriter writer, T? value, KSerializerOptions options)
        {
            var type = typeof(T);
            // Check if T is ValueTuple and the first field is string.
            if (type.Name.Contains(nameof(ValueTuple)) && type.GetField("Item1")?.FieldType == typeof(string))
            {
                ParameterizedQueryConverter<T>.Write(writer, value!, options);
            }
            else
            {
                throw new InvalidOperationException("Not a parameterized query.");
            }
        }
    }
    internal async Task<T?> RecvResponseObjectAsync<T>(KSerializerOptions? options , CancellationToken cancellation)
    {
        var message = await RecvAsync(cancellation);
        if (message.Type != MessageType.Response)
        {
            throw new InvalidOperationException("Unexpected message type.");
        }
        if (message.Compressed)
        {
            var uncompressed = message;
            message = KMessage.Uncompress(message);
            uncompressed.Dispose();
        }
        _readerOptions.IsLittleEndian = message.IsLittleEndian;
        _readerOptions.TextEncoding = TextEncoding;
        return KSerializer.Deserialize<T>(message.Body, _readerOptions, options);
    }

    internal async Task SendParameterizedAsyncAsync<T>(T value, KSerializerOptions? options, CancellationToken cancellation)
    {
        KSerializer.Serialize(Writer, value, ConvertParameterizedQuery, options);
        _messageWrite.RepackMessage(MessageType.Async, Writer.Buffer.IsLittleEndian, Writer.Buffer.GetWritedMemory());
        await SendAsync(_messageWrite, cancellation);
        Writer.Buffer.Clear();

        static void ConvertParameterizedQuery(KWriter writer, T? value, KSerializerOptions options)
        {
            var type = typeof(T);
            // Check if T is ValueTuple and the first field is string.
            if (type.Name.Contains(nameof(ValueTuple)) && type.GetField("Item1")?.FieldType == typeof(string))
            {
                ParameterizedQueryConverter<T>.Write(writer, value!, options);
            }
            else
            {
                throw new InvalidOperationException("Not a parameterized query.");
            }
        }
    }

    // Reader/Writer holds a buffer, and KMessage can just reference the buffer by Memory<byte>(a view of part of the buffer).
    public async Task<TResult?> Get<TResult>(string expr, KSerializerOptions? options = null, CancellationToken cancellation = default)
    {
        await SendRequestAsync(expr, options, cancellation);
        return await RecvResponseObjectAsync<TResult>(options, cancellation);
    }

    static KdbCommand<T> CreateCommandInternal<T>(T parameterizedQuery, KdbConnection connection)
    {
        return new KdbCommand<T>(parameterizedQuery, connection);
    }

    #region CreateCommand

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

    #region Subscription

    public Task Set(string expr, CancellationToken cancellation = default)
    {
        // Serialize and send an async KMessage.
        KSerializer.Serialize(Writer, expr, KType.CharList);
        _messageWrite.RepackMessage(MessageType.Async, Writer.Buffer.IsLittleEndian, Writer.Buffer.GetWritedMemory());

        return SendAsync(_messageWrite, cancellation);
    }

    public async IAsyncEnumerable<KMessage> SubscribeAsync([EnumeratorCancellation] CancellationToken cancellation = default)
    {
        while (!cancellation.IsCancellationRequested)
        {
            var message = await RecvAsync(cancellation);
            if (message.Type != MessageType.Async)
            {
                if (message.Type == MessageType.Request)
                {
                    // TODO: SendMessage(Response, "Unexpected request message.")
                    throw new InvalidOperationException("Unexpected request message.");
                }
                else if(message.Type == MessageType.Response)
                {
                    throw new InvalidOperationException("Unexpected response message.");
                }
                else
                {
                    throw new InvalidOperationException("Unknown message type.");
                }
            }
            yield return message;
        }
    }
    #endregion
}

public static class ConnectionExtensions
{
}
