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
using System.Buffers.Binary;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KdbSharp.Serialization;

public struct KWriteBuffer
{
    public IBufferWriter<byte> BufferWriter { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KWriteBuffer"/> class with the specified buffer writer.
    /// </summary>
    /// <param name="bufferWriter">The underlying buffer writer.</param>
    public KWriteBuffer(IBufferWriter<byte> bufferWriter)
    {
        BufferWriter = bufferWriter ?? throw new ArgumentNullException(nameof(bufferWriter));
    }

    /// <summary>
    /// Gets a value indicating whether the system architecture is little-endian.
    /// </summary>
    public bool IsLittleEndian => BitConverter.IsLittleEndian;

    /// <summary>
    /// Advances the writer by the specified count of bytes.
    /// </summary>
    /// <param name="count">The number of bytes to advance.</param>
    public void Advance(int count)
    {
        BufferWriter.Advance(count);
    }

    /// <summary>
    /// Gets a memory region to write to that is at least the specified size.
    /// </summary>
    /// <param name="sizeHint">The minimum size of the requested memory region.</param>
    /// <returns>A memory region to write to.</returns>
    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        return BufferWriter.GetMemory(sizeHint);
    }

    /// <summary>
    /// Gets a span to write to that is at least the specified size.
    /// </summary>
    /// <param name="sizeHint">The minimum size of the requested span.</param>
    /// <returns>A span to write to.</returns>
    public Span<byte> GetSpan(int sizeHint = 0)
    {
        return BufferWriter.GetSpan(sizeHint);
    }

    /// <summary>
    /// Writes a byte to the buffer.
    /// </summary>
    /// <param name="value">The byte value to write.</param>
    public void WriteByte(byte value)
    {
        var span = GetSpan(1);
        span[0] = value;
        Advance(1);
    }

    /// <summary>
    /// Writes a 16-bit integer to the buffer.
    /// </summary>
    /// <param name="value">The 16-bit integer value to write.</param>
    public void WriteInt16(short value)
    {
        var span = GetSpan(2);
        BinaryPrimitives.WriteInt16LittleEndian(span, value);
        Advance(2);
    }

    /// <summary>
    /// Writes a 32-bit integer to the buffer.
    /// </summary>
    /// <param name="value">The 32-bit integer value to write.</param>
    public void WriteInt32(int value)
    {
        var span = GetSpan(4);
        BinaryPrimitives.WriteInt32LittleEndian(span, value);
        Advance(4);
    }

    /// <summary>
    /// Writes a 64-bit integer to the buffer.
    /// </summary>
    /// <param name="value">The 64-bit integer value to write.</param>
    public void WriteInt64(long value)
    {
        var span = GetSpan(8);
        BinaryPrimitives.WriteInt64LittleEndian(span, value);
        Advance(8);
    }

    /// <summary>
    /// Writes a single-precision floating-point value to the buffer.
    /// </summary>
    /// <param name="value">The single-precision floating-point value to write.</param>
    public void WriteSingle(float value)
    {
        var span = GetSpan(4);
        BitConverter.TryWriteBytes(span, value);
        Advance(4);
    }

    /// <summary>
    /// Writes a double-precision floating-point value to the buffer.
    /// </summary>
    /// <param name="value">The double-precision floating-point value to write.</param>
    public void WriteDouble(double value)
    {
        var span = GetSpan(8);
        BitConverter.TryWriteBytes(span, value);
        Advance(8);
    }

    /// <summary>
    /// Writes a string to the buffer using the specified encoding.
    /// </summary>
    /// <param name="value">The string value to write.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The number of bytes written.</returns>
    public int WriteString(ReadOnlySpan<char> value, Encoding encoding)
    {
        int maxBytes = encoding.GetMaxByteCount(value.Length);
        var span = GetSpan(maxBytes);
        int bytesWritten = encoding.GetBytes(value, span);
        Advance(bytesWritten);
        return bytesWritten;
    }

    /// <summary>
    /// Writes a null-terminated string to the buffer using the specified encoding.
    /// </summary>
    /// <param name="value">The string value to write.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void WriteNullTerminatedString(ReadOnlySpan<char> value, Encoding encoding)
    {
        WriteString(value, encoding);
        WriteByte(0);
    }

    /// <summary>
    /// Writes a span of bytes to the buffer.
    /// </summary>
    /// <param name="span">The span of bytes to write.</param>
    public void WriteBytes(ReadOnlySpan<byte> span)
    {
        var bufferSpan = GetSpan(span.Length);
        span.CopyTo(bufferSpan);
        Advance(span.Length);
    }

    /// <summary>
    /// Writes an unmanaged value to the buffer.
    /// </summary>
    /// <typeparam name="T">The type of the unmanaged value.</typeparam>
    /// <param name="value">The value to write.</param>
    public unsafe void Write<T>(T value) where T : unmanaged
    {
        int size = sizeof(T);
        var span = GetSpan(size);
        MemoryMarshal.Write(span, ref value);
        Advance(size);
    }

    #region Internal API

    internal void WriteBool(bool value)
    {
        WriteByte(value ? (byte)1 : (byte)0);
    }

    #endregion
}

