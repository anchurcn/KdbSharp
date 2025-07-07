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
using System.Runtime.InteropServices;
using System.Text;

namespace KdbSharp.Serialization
{
    /// <summary>
    /// Make MemoryMarshal easier to use. (Extension methods)
    /// </summary>
    public static class BufferHelper
    {
        // As : Different underlying type.
        // Get: The same underlying type.
        public static ref T AsRef<T>(this Span<byte> span) where T : struct
            => ref MemoryMarshal.AsRef<T>(span);
        public static ref T GetRef<T>(this Span<T> span) where T : struct
            => ref MemoryMarshal.GetReference(span);

        public static Span<T> GetSpan<T>(this ref T value) where T : struct
            => MemoryMarshal.CreateSpan(ref value, 1);
        public static Span<byte> AsSpan<T>(this ref T value) where T : struct
            => MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1));

        public static Span<TTo> Cast<TFrom, TTo>(this Span<TFrom> span) where TFrom : struct where TTo : struct
            => MemoryMarshal.Cast<TFrom, TTo>(span);
        public static Span<byte> CastToBytes<T>(this Span<T> span) where T : struct
            => MemoryMarshal.Cast<T, byte>(span);

        public static ReadOnlySpan<TTo> Cast<TFrom, TTo>(this ReadOnlySpan<TFrom> span) where TFrom : struct where TTo : struct
            => MemoryMarshal.Cast<TFrom, TTo>(span);
    }

    /// <summary>
    /// Extension methods for IBufferWriter&lt;byte&gt; to write binary data.
    /// </summary>
    public static class BufferWriterExtensions
    {
        public static void WriteByte(this IBufferWriter<byte> writer, byte value)
        {
            var span = writer.GetSpan(1);
            span[0] = value;
            writer.Advance(1);
        }

        public static void WriteBytes(this IBufferWriter<byte> writer, ReadOnlySpan<byte> bytes)
        {
            var span = writer.GetSpan(bytes.Length);
            bytes.CopyTo(span);
            writer.Advance(bytes.Length);
        }

        public static void WriteBool(this IBufferWriter<byte> writer, bool value)
        {
            writer.WriteByte(value ? (byte)1 : (byte)0);
        }

        public static void WriteInt16(this IBufferWriter<byte> writer, short value)
        {
            var span = writer.GetSpan(2);
            MemoryMarshal.Write(span, ref value);
            writer.Advance(2);
        }

        public static void WriteInt32(this IBufferWriter<byte> writer, int value)
        {
            var span = writer.GetSpan(4);
            MemoryMarshal.Write(span, ref value);
            writer.Advance(4);
        }

        public static void WriteInt64(this IBufferWriter<byte> writer, long value)
        {
            var span = writer.GetSpan(8);
            MemoryMarshal.Write(span, ref value);
            writer.Advance(8);
        }

        public static void WriteSingle(this IBufferWriter<byte> writer, float value)
        {
            var span = writer.GetSpan(4);
            MemoryMarshal.Write(span, ref value);
            writer.Advance(4);
        }

        public static void WriteDouble(this IBufferWriter<byte> writer, double value)
        {
            var span = writer.GetSpan(8);
            MemoryMarshal.Write(span, ref value);
            writer.Advance(8);
        }

        /// <summary>
        /// Writes a string using the specified encoding.
        /// </summary>
        /// <param name="writer">The buffer writer.</param>
        /// <param name="value">The string value to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>The number of bytes written.</returns>
        public static int WriteString(this IBufferWriter<byte> writer, ReadOnlySpan<char> value, Encoding encoding)
        {
            var sizeHint = encoding.GetMaxByteCount(value.Length);
            var span = writer.GetSpan(sizeHint);
            var written = encoding.GetBytes(value, span);
            writer.Advance(written);
            return written;
        }

        public static int WriteNullTerminatedString(this IBufferWriter<byte> writer, ReadOnlySpan<char> value, Encoding encoding)
        {
            var bytesWritten = writer.WriteString(value, encoding);
            writer.WriteByte(0);
            return bytesWritten + 1;
        }

        /// <summary>
        /// Writes an unmanaged value directly to the buffer.
        /// </summary>
        /// <typeparam name="T">The unmanaged type.</typeparam>
        /// <param name="writer">The buffer writer.</param>
        /// <param name="value">The value to write.</param>
        public static unsafe void Write<T>(this IBufferWriter<byte> writer, T value) where T : unmanaged
        {
            int size = sizeof(T);
            var span = writer.GetSpan(size);
            MemoryMarshal.Write(span, ref value);
            writer.Advance(size);
        }
    }

    internal static class SequenceReaderExtensions
    {
        // ReadByte
        public static byte ReadByte(this ref SequenceReader<byte> reader)
        {
            if (!reader.TryRead(out byte value))
            {
                throw new InvalidOperationException("Not enough data to read a byte.");
            }
            return value;
        }
    }

    public static class SerializationHelper
    {
        public static void FlipGuidTop3Parts(this ref Guid guid)
        {
            var span = guid.AsSpan();
            span.Slice(0, 4).Reverse();
            span.Slice(4, 2).Reverse();
            span.Slice(6, 2).Reverse();
        }
    }
}
