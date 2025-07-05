using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace KdbSharp.Buffers;

/// <summary>
/// Provides extension methods for <see cref="IBufferWriter{T}"/> implementations.
/// </summary>
public static class BufferWriterExtensions
{
    /// <summary>
    /// Writes a byte to the buffer.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The byte to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteByte(this IBufferWriter<byte> writer, byte value)
    {
        var span = writer.GetSpan(1);
        span[0] = value;
        writer.Advance(1);
    }

    /// <summary>
    /// Writes a boolean value to the buffer.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The boolean value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBoolean(this IBufferWriter<byte> writer, bool value)
    {
        writer.WriteByte(value ? (byte)1 : (byte)0);
    }

    /// <summary>
    /// Writes a 16-bit integer to the buffer in little-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The 16-bit integer to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt16LittleEndian(this IBufferWriter<byte> writer, short value)
    {
        var span = writer.GetSpan(2);
        BinaryPrimitives.WriteInt16LittleEndian(span, value);
        writer.Advance(2);
    }

    /// <summary>
    /// Writes a 16-bit integer to the buffer in big-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The 16-bit integer to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt16BigEndian(this IBufferWriter<byte> writer, short value)
    {
        var span = writer.GetSpan(2);
        BinaryPrimitives.WriteInt16BigEndian(span, value);
        writer.Advance(2);
    }

    /// <summary>
    /// Writes a 32-bit integer to the buffer in little-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The 32-bit integer to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt32LittleEndian(this IBufferWriter<byte> writer, int value)
    {
        var span = writer.GetSpan(4);
        BinaryPrimitives.WriteInt32LittleEndian(span, value);
        writer.Advance(4);
    }

    /// <summary>
    /// Writes a 32-bit integer to the buffer in big-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The 32-bit integer to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt32BigEndian(this IBufferWriter<byte> writer, int value)
    {
        var span = writer.GetSpan(4);
        BinaryPrimitives.WriteInt32BigEndian(span, value);
        writer.Advance(4);
    }

    /// <summary>
    /// Writes a 64-bit integer to the buffer in little-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The 64-bit integer to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt64LittleEndian(this IBufferWriter<byte> writer, long value)
    {
        var span = writer.GetSpan(8);
        BinaryPrimitives.WriteInt64LittleEndian(span, value);
        writer.Advance(8);
    }

    /// <summary>
    /// Writes a 64-bit integer to the buffer in big-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The 64-bit integer to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt64BigEndian(this IBufferWriter<byte> writer, long value)
    {
        var span = writer.GetSpan(8);
        BinaryPrimitives.WriteInt64BigEndian(span, value);
        writer.Advance(8);
    }

    /// <summary>
    /// Writes a single-precision floating-point value to the buffer in little-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The single-precision floating-point value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSingleLittleEndian(this IBufferWriter<byte> writer, float value)
    {
        var span = writer.GetSpan(4);
        BinaryPrimitives.WriteSingleLittleEndian(span, value);
        writer.Advance(4);
    }

    /// <summary>
    /// Writes a single-precision floating-point value to the buffer in big-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The single-precision floating-point value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSingleBigEndian(this IBufferWriter<byte> writer, float value)
    {
        var span = writer.GetSpan(4);
        BinaryPrimitives.WriteSingleBigEndian(span, value);
        writer.Advance(4);
    }

    /// <summary>
    /// Writes a double-precision floating-point value to the buffer in little-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The double-precision floating-point value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteDoubleLittleEndian(this IBufferWriter<byte> writer, double value)
    {
        var span = writer.GetSpan(8);
        BinaryPrimitives.WriteDoubleLittleEndian(span, value);
        writer.Advance(8);
    }

    /// <summary>
    /// Writes a double-precision floating-point value to the buffer in big-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The double-precision floating-point value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteDoubleBigEndian(this IBufferWriter<byte> writer, double value)
    {
        var span = writer.GetSpan(8);
        BinaryPrimitives.WriteDoubleBigEndian(span, value);
        writer.Advance(8);
    }

    /// <summary>
    /// Writes a string to the buffer using the specified encoding.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The string to write.</param>
    /// <param name="encoding">The encoding to use.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteString(this IBufferWriter<byte> writer, string value, Encoding encoding)
    {
        if (string.IsNullOrEmpty(value))
            return;

        int maxByteCount = encoding.GetMaxByteCount(value.Length);
        var span = writer.GetSpan(maxByteCount);
        
        int bytesWritten = encoding.GetBytes(value, span);
        writer.Advance(bytesWritten);
    }

    /// <summary>
    /// Writes a null-terminated string to the buffer using the specified encoding.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The string to write.</param>
    /// <param name="encoding">The encoding to use.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNullTerminatedString(this IBufferWriter<byte> writer, string value, Encoding encoding)
    {
        WriteString(writer, value, encoding);
        WriteByte(writer, 0);
    }

    /// <summary>
    /// Writes a span of bytes to the buffer.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The span of bytes to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBytes(this IBufferWriter<byte> writer, ReadOnlySpan<byte> value)
    {
        if (value.IsEmpty)
            return;

        var span = writer.GetSpan(value.Length);
        value.CopyTo(span);
        writer.Advance(value.Length);
    }

    /// <summary>
    /// Writes a 16-bit unsigned integer to the buffer in little-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The 16-bit unsigned integer to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt16LittleEndian(this IBufferWriter<byte> writer, ushort value)
    {
        var span = writer.GetSpan(2);
        BinaryPrimitives.WriteUInt16LittleEndian(span, value);
        writer.Advance(2);
    }

    /// <summary>
    /// Writes a 16-bit unsigned integer to the buffer in big-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The 16-bit unsigned integer to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt16BigEndian(this IBufferWriter<byte> writer, ushort value)
    {
        var span = writer.GetSpan(2);
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
        writer.Advance(2);
    }

    /// <summary>
    /// Writes a 32-bit unsigned integer to the buffer in little-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The 32-bit unsigned integer to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt32LittleEndian(this IBufferWriter<byte> writer, uint value)
    {
        var span = writer.GetSpan(4);
        BinaryPrimitives.WriteUInt32LittleEndian(span, value);
        writer.Advance(4);
    }

    /// <summary>
    /// Writes a 32-bit unsigned integer to the buffer in big-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The 32-bit unsigned integer to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt32BigEndian(this IBufferWriter<byte> writer, uint value)
    {
        var span = writer.GetSpan(4);
        BinaryPrimitives.WriteUInt32BigEndian(span, value);
        writer.Advance(4);
    }

    /// <summary>
    /// Writes a 64-bit unsigned integer to the buffer in little-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The 64-bit unsigned integer to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt64LittleEndian(this IBufferWriter<byte> writer, ulong value)
    {
        var span = writer.GetSpan(8);
        BinaryPrimitives.WriteUInt64LittleEndian(span, value);
        writer.Advance(8);
    }

    /// <summary>
    /// Writes a 64-bit unsigned integer to the buffer in big-endian byte order.
    /// </summary>
    /// <param name="writer">The buffer writer.</param>
    /// <param name="value">The 64-bit unsigned integer to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt64BigEndian(this IBufferWriter<byte> writer, ulong value)
    {
        var span = writer.GetSpan(8);
        BinaryPrimitives.WriteUInt64BigEndian(span, value);
        writer.Advance(8);
    }
}
