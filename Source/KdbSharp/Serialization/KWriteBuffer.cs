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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KdbSharp.Serialization;

public class KWriteBuffer
{
    private byte[] _buffer;
    private int _position;
    private KWriter _writer;

    public KWriteBuffer(KWriterOptions options)
    {
        _buffer = Array.Empty<byte>();
        _position = 0;
        _writer = new KWriter(this, options);
    }

    internal KWriteBuffer()
    {
        var options = new KWriterOptions();
        _buffer = Array.Empty<byte>();
        _position = 0;
        _writer = new KWriter(this, options);
    }

    public bool IsLittleEndian => BitConverter.IsLittleEndian;

    public int Length => _position;

    public int Capacity => _buffer.Length;

    public KWriter Writer => _writer;

    public void WriteByte(byte value)
    {
        EnsureCapacity(1);
        _buffer[_position++] = value;
    }

    public void WriteInt16(short value)
    {
        EnsureCapacity(2);
        BitConverter.TryWriteBytes(_buffer.AsSpan(_position), value);
        _position += 2;
    }

    public void WriteInt32(int value)
    {
        EnsureCapacity(4);
        BitConverter.TryWriteBytes(_buffer.AsSpan(_position), value);
        _position += 4;
    }

    public void WriteInt64(long value)
    {
        EnsureCapacity(8);
        BitConverter.TryWriteBytes(_buffer.AsSpan(_position), value);
        _position += 8;
    }

    public void WriteSingle(float value)
    {
        EnsureCapacity(4);
        BitConverter.TryWriteBytes(_buffer.AsSpan(_position), value);
        _position += 4;
    }

    public void WriteDouble(double value)
    {
        EnsureCapacity(8);
        BitConverter.TryWriteBytes(_buffer.AsSpan(_position), value);
        _position += 8;
    }

    public int WriteString(ReadOnlySpan<char> value, Encoding encoding)
    {
        // Ensure max capacity of given string in given encoding.
        int maxBytes = encoding.GetMaxByteCount(value.Length);
        EnsureCapacity(maxBytes);
        int bytesWritten = encoding.GetBytes(value, _buffer.AsSpan(_position));
        _position += bytesWritten;     
        return bytesWritten;
    }

    public void WriteNullTerminatedString(ReadOnlySpan<char> value, Encoding encoding)
    {
        WriteString(value, encoding);
        WriteByte(0);
    }

    public Span<byte> AllocSpan(int length)
    {
        EnsureCapacity(length);
        Span<byte> span = _buffer.AsSpan(_position, length);
        _position += length;
        return span;
    }

    public byte[] ToArray()
    {
        byte[] result = new byte[_position];
        Buffer.BlockCopy(_buffer, 0, result, 0, _position);
        return result;
    }

    // GetWritedBuffer
    public ReadOnlySpan<byte> GetWritedBuffer()
    {
        return _buffer.AsSpan(0, _position);
    }

    // Memory<byte>
    public Memory<byte> GetWritedMemory()
    {
        return _buffer.AsMemory(0, _position);
    }

    public void Clear()
    {
        _position = 0;
    }

    private void EnsureCapacity(int additionalBytes)
    {
        if (_position + additionalBytes > _buffer.Length)
        {
            int newCapacity = Math.Max(_buffer.Length * 2, _position + additionalBytes);
            byte[] newBuffer = new byte[newCapacity];
            Buffer.BlockCopy(_buffer, 0, newBuffer, 0, _position);
            _buffer = newBuffer;
        }
    }

    internal void WriteBytes(ReadOnlySpan<byte> span)
    {
        EnsureCapacity(span.Length);
        span.CopyTo(_buffer.AsSpan(_position));
        _position += span.Length;
    }

    public unsafe void Write<T>(T value) where T : unmanaged
    {
        int size = sizeof(T);
        var span = AllocSpan(size);
        MemoryMarshal.Write(span, ref value);
    }

    #region Internal API

    internal void WriteBool(bool value)
    {
        WriteByte(value ? (byte)1 : (byte)0);
    }
    #endregion
}

