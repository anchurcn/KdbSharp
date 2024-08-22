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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace KdbSharp.Serialization;

/// <summary>
/// A buffer to read data from stream. This class handles the message header, endianness and compression.
/// </summary>
/// <remarks>
/// The buffer is initialized with the whole message, and then the buffer can be used to read data.
/// If the message is compressed, the _buffer is set to null, and the raw data is stored in _rawData.
/// After the message is read, we need to check the compression status and uncompress if needed.
/// </remarks>
internal class KReadBuffer
{
    internal ReadOnlyMemory<byte> _buffer = default!;
    internal ReadOnlyMemory<byte> RestBuffer => _buffer[_position..];
    internal ReadOnlySpan<byte> RestSpan => RestBuffer.Span;
    internal int _position = 0;
    public bool IsLittleEndian { get; internal set; }


    public KReader KdbReader { get; private set; }

    public KReadBuffer(ReadOnlyMemory<byte> buffer, KReaderOptions? options = null)
    {
        options ??= new KReaderOptions();
        SetBuffer(buffer);
        IsLittleEndian = options.IsLittleEndian;
        KdbReader = new KReader(this, options);
    }
    internal KReadBuffer()
    {
        SetBuffer(ReadOnlyMemory<byte>.Empty);
        IsLittleEndian = true;
        KdbReader = new KReader(this, new KReaderOptions());
    }

    internal void SetBuffer(ReadOnlyMemory<byte> buffer)
    {
        _buffer = buffer;
        _position = 0;
    }

    #region Read methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe T Read<T>() where T : unmanaged
    {
        var len = sizeof(T);
        return MemoryMarshal.Read<T>(CreateSpan(len));
    }

    #region Integer

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadInt64() => ReadInt64(IsLittleEndian);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // The parameter littleEndian indicates whether the data should be read in little endian byte order.
    // If true, the buffer data is little endian, and the host is big endian, then reverse the byte order.
    public long ReadInt64(bool littleEndian)
    {
        var result = Read<long>();
        return littleEndian == BitConverter.IsLittleEndian
            ? result : BinaryPrimitives.ReverseEndianness(result);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32() => ReadInt32(IsLittleEndian);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32(bool littleEndian)
    {
        var result = Read<int>();
        return littleEndian == BitConverter.IsLittleEndian
            ? result : BinaryPrimitives.ReverseEndianness(result);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16() => ReadInt16(IsLittleEndian);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16(bool littleEndian)
    {
        var result = Read<short>();
        return littleEndian == BitConverter.IsLittleEndian
            ? result : BinaryPrimitives.ReverseEndianness(result);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte() => Read<byte>();
    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadSingle() => Read<float>();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDouble() => Read<double>();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBool()
    {
        var result = Read<byte>();
        return result != 0;
    }
    #region Byte sequence

    public ReadOnlyMemory<byte> CreateMemory(int len)
    {
        if (RestBuffer.Length < len)
        {
            throw new ArgumentOutOfRangeException(nameof(len));
        }
        var result = RestBuffer[..len];
        _position += len;
        return result;
    }
    public ReadOnlySpan<byte> CreateSpan(int len)
    {
        return CreateMemory(len).Span;
    }
    public Stream CreateStream(int len, bool canSeek)
    {
        var result = new ReadOnlyMemoryStream(CreateMemory(len), canSeek);
        return result;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadBytes(Span<byte> span)
    {
        CreateSpan(span.Length).CopyTo(span);
    }

    public void ReadBytes(byte[] output, int outputOffset, int len)
    {
        CreateSpan(len).CopyTo(output.AsSpan(outputOffset, len));
    }
    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString(int byteLen, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var result = encoding.GetString(_buffer.Span.Slice(_position, byteLen));
        _position += byteLen;
        return result;
    }



    // Implements ReadOnlyMemoryStream class
    public class ReadOnlyMemoryStream : Stream
    {
        private readonly ReadOnlyMemory<byte> _memory;
        private readonly bool _canSeek;
        private int _position;
        public ReadOnlyMemoryStream(ReadOnlyMemory<byte> buffer, bool canSeek)
        {
            _memory = buffer;
            _canSeek = canSeek;
        }
        public override bool CanRead => true;
        public override bool CanSeek => _canSeek;
        public override bool CanWrite => false;
        public override long Length => _memory.Length;
        public override long Position
        {
            get => _position;
            set
            {
                if (!_canSeek)
                {
                    throw new NotSupportedException();
                }
                if (value < 0 || value > int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _position = (int)value;
            }
        }
        public override void Flush()
        {
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = Math.Min(count, _memory.Length - _position);
            _memory.Span.Slice(_position, read).CopyTo(buffer.AsSpan(offset, read));
            _position += read;
            return read;
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!_canSeek)
            {
                throw new NotSupportedException();
            }
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin));
            }
            return Position;
        }
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }

    public string ReadNullTerminatedString(Encoding encoding)
    {
        return encoding.GetString(GetNullTerminatedBytes());
    }

    public ReadOnlySpan<byte> GetNullTerminatedBytes()
    {
        var index = RestSpan.IndexOf((byte)0);
        if (index == -1)
        {
            throw new KdbProtocolException("Null terminator not found.");
        }
        var result = CreateSpan(index);
        Skip(1);
        return result;
    }

    public void Skip(int len)
    {
        _ = CreateSpan(len);
    }

    #endregion

}
