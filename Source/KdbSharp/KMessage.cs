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
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace KdbSharp;

/*
 * Kdb IPC message definition
 */

public enum MessageType : byte
{
    Async,
    Request,
    Response
}

public enum Endianess : byte
{
    BigEndian,
    LittleEndian
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct MessageHeaderMeta
{
    public Endianess Endianess;
    public MessageType Type;
    public bool Compressed;
    public byte Unused;
}

public class KMessage : IDisposable
{
    public const int HEADER_SIZE = 8;

    internal MessageHeaderMeta HeaderMeta;
    // Cannot directly set this field, use Body or AllocNewBody
    // Set by Body property to be a consumer of the memory.
    // Set by AllocNewBody to be a owner of the memory.
    private Memory<byte> _bodyMemory;
    private IMemoryOwner<byte>? _bodyOwner;
    public Memory<byte> Body
    {
        get => _bodyMemory;
        private set
        {
            _bodyOwner?.Dispose();
            _bodyOwner = null;
            _bodyMemory = value;
        }
    }

    public MessageType Type => HeaderMeta.Type;
    public bool IsLittleEndian => HeaderMeta.Endianess == Endianess.LittleEndian;
    public bool Compressed => HeaderMeta.Compressed;
    public int BodySize => Body.Length;
    public int? UncompressedSize => Compressed ? (IsLittleEndian
        ? BinaryPrimitives.ReadInt32LittleEndian(Body.Span) : BinaryPrimitives.ReadInt32BigEndian(Body.Span)) : null;

    public int Size => HEADER_SIZE + BodySize;

    private void AllocNewBody(int size)
    {
        _bodyOwner?.Dispose();
        _bodyOwner = MemoryPool<byte>.Shared.Rent(size);
        _bodyMemory = _bodyOwner.Memory[..size];
    }

    public static KMessage CreateReusable()
    {
        return new KMessage();
    }

    public void RepackUncompressedMessage(MessageType type, Endianess endianess, Memory<byte> body)
    {
        HeaderMeta = new MessageHeaderMeta
        {
            Endianess = endianess,
            Type = type,
            Compressed = false,
            Unused = 0
        };
        Body = body;
    }

    public void RepackMessage(MessageType type, bool endianess, Memory<byte> body)
    {
        // TODO: Compress if needed
        // TODO: Can pass an allocator for compressed buffer.
        RepackUncompressedMessage(type, endianess ? Endianess.LittleEndian : Endianess.BigEndian, body);
    }

    public static KMessage Alloc(MessageType type, Endianess endianess, bool compressed, int messageSize)
    {
        var message = new KMessage
        {
            HeaderMeta = new MessageHeaderMeta
            {
                Endianess = endianess,
                Type = type,
                Compressed = compressed,
                Unused = 0
            }
        };
        message.AllocNewBody(messageSize - HEADER_SIZE);
        return message;
    }
    internal static KMessage Alloc(MessageHeaderMeta headerMeta, int messageLen)
    {
        return Alloc(headerMeta.Type, headerMeta.Endianess, headerMeta.Compressed, messageLen);
    }

    public void Dispose()
    {
        _bodyOwner?.Dispose();
        _bodyOwner = null;
    }

    ~KMessage()
    {
        Dispose();
    }

    public KMessage Compress()
    {
        if (Compressed)
        {
            throw new InvalidOperationException("Already compressed.");
        }
        throw new NotImplementedException();
    }
    public void Uncompress()
    {
        if (!Compressed)
        {
            throw new InvalidOperationException("Not compressed.");
        }
        throw new NotImplementedException();
    }

    public static KMessage Uncompress(KMessage message)
    {
        if (!message.Compressed)
        {
            throw new InvalidOperationException("Not compressed.");
        }
        var result = Alloc(message.Type, message.HeaderMeta.Endianess, false, message.UncompressedSize!.Value);
        Uncompress(message.Body.Span[4..], result.Body.Span);
        return result;
    }

    /// <summary>
    /// Uncompress the compressed kdb message payload.
    /// </summary>
    /// <param name="compressed">Input.</param>
    /// <param name="uncompressed">Output.</param>
    /// <remarks>
    /// Modified from https://github.com/exxeleron/qSharp/blob/acd64929ce84e4329ba74ff58356eb4f5aafaf03/qSharp/src/QReader.cs#L136.
    /// </remarks>
    private static void Uncompress(ReadOnlySpan<byte> compressed, Span<byte> uncompressed)
    {
        var uncompressedSize = uncompressed.Length;

        Span<int> buffer = stackalloc int[256];
        short i = 0;
        var n = 0;
        int f = 0, s = 0, p = 0, d = 0;

        while (s < uncompressedSize)
        {
            if (i == 0)
            {
                f = 0xff & compressed[d++];
                i = 1;
            }
            if ((f & i) != 0)
            {
                var r = buffer[0xff & compressed[d++]];
                uncompressed[s++] = uncompressed[r++];
                uncompressed[s++] = uncompressed[r++];
                n = 0xff & compressed[d++];
                for (var m = 0; m < n; m++)
                {
                    uncompressed[s + m] = uncompressed[r + m];
                }
            }
            else
            {
                uncompressed[s++] = compressed[d++];
            }
            while (p < s - 1)
            {
                buffer[(0xff & uncompressed[p]) ^ (0xff & uncompressed[p + 1])] = p++;
            }
            if ((f & i) != 0)
            {
                p = s += n;
            }
            i *= 2;
            if (i == 256)
            {
                i = 0;
            }
        }
    }
}

//public class MessageBuilder
//{

//    public MessageBuilder() { }
//    public MessageBuilder(MessageType type, Endianess endianess, bool compressed)
//    {
//        _type = type;
//        _endianess = endianess;
//        _compressed = compressed;
//    }
//    private MessageType _type;
//    private Endianess _endianess;
//    private bool _compressed;

//    private List<byte> _buffer = new List<byte>();
//    public KdbDataWriter _bodyWriter { get; set; }
//    public KdbDataWriter StartWriteBody()
//    {
//        _bodyWriter new KdbDataWriter(new KdbWriteBuffer(_buffer, _endianess));
//        return _bodyWriter;
//    }
//    public unsafe Message Build()
//    {
//        var buffer = _bodyWriter.ToArray();
//        return new Message(_type, _endianess, _compressed, buffer);
//    }

//}
//public class KdbDataWriter
//{
//    MemoryStream MemoryStream;
//    public byte[] ToArray()
//    {
//        return MemoryStream.ToArray();
//    }
//    public void Write(byte[] buffer)
//    {
//        MemoryStream.Write(buffer);
//    }
//    public void WriteInt32(int value)
//    {
//        MemoryStream.Write(BitConverter.GetBytes(value));
//    }
//    public void Write<T>(KdbType atomType, T value)
//    {

//    }
//}
/*

## Protocol
Kdb+ protocol is a simple binary protocol and runs on the top of TCP.

### Connection
1. TCP - The client connects to the server using TCP. The server listens on a specific port for incoming connections.
2. Handshake - The client sends a handshake info to the server. The server responds with a handshake info. 
* After a client has opened a socket to the server, it sends a null-terminated string "username:password\N" where \N is the client supported protocol version.
* If the server accepts the credentials, it sends a single-byte response which represents the common capability (protocol version).



### Message
The protocol is message based. Each message has a header and a body. 
The header contains the message type, endianess, and compression flag. 
The body contains the actual data.


________________________________________________________________________
|  Endianess  |  Type  | Compression |  Unused  |  Len |  Body         |
|-------------|--------|-------------|----------|------|_______________|
|      1      |    1   |      1      |     1    |   4  |  Len-8        |
|_____________|________|_____________|__________|______|_______________|
________________________________________________________________________
|  Endianess  |  Type  | Compression |  Unused  |  Len |  OLen |  Body |
|-------------|--------|-------------|----------|------|-------|-------|
|      1      |    1   |      1      |     1    |   4  |   4   | OLen-8|
|_____________|________|_____________|__________|______|_______|_______|



### Body


*/
