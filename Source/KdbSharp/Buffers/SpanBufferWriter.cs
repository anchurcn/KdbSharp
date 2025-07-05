// using System;
// using System.Buffers;
// using System.IO;
// using System.Runtime.CompilerServices;

// namespace KdbSharp.Buffers;

// /// <summary>
// /// An implementation of <see cref="IBufferWriter{T}"/> that writes to a fixed-size span.
// /// </summary>
// /// <typeparam name="T">The type of elements in the buffer.</typeparam>
// public ref struct SpanBufferWriter<T>: IBufferWriter<T>
// {
//     private Span<T> _buffer;
//     private int _written;

//     /// <summary>
//     /// Initializes a new instance of the <see cref="SpanBufferWriter{T}"/> struct.
//     /// </summary>
//     /// <param name="buffer">The buffer to write to.</param>
//     public SpanBufferWriter(Span<T> buffer)
//     {
//         _buffer = buffer;
//         _written = 0;
//     }

//     /// <summary>
//     /// Gets the number of elements written to the buffer.
//     /// </summary>
//     public int Written => _written;

//     /// <summary>
//     /// Gets the span representing the written data.
//     /// </summary>
//     public ReadOnlySpan<T> WrittenSpan => _buffer.Slice(0, _written);

//     /// <summary>
//     /// Gets the span representing the free space in the buffer.
//     /// </summary>
//     public Span<T> FreeSpan => _buffer.Slice(_written);

//     /// <summary>
//     /// Gets the current position in the buffer.
//     /// </summary>
//     public int Position
//     {
//         get => _written;
//         set
//         {
//             if (value < 0 || value > _buffer.Length)
//                 throw new ArgumentOutOfRangeException(nameof(value));
//             _written = value;
//         }
//     }

//     /// <summary>
//     /// Advances the writer by the specified number of elements.
//     /// </summary>
//     /// <param name="count">The number of elements to advance.</param>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void Advance(int count)
//     {
//         if (count < 0)
//             throw new ArgumentOutOfRangeException(nameof(count));

//         if (_written > _buffer.Length - count)
//             throw new InvalidOperationException("Cannot advance past the end of the buffer");

//         _written += count;
//     }

//     /// <summary>
//     /// Gets a span of at least the specified length.
//     /// </summary>
//     /// <param name="sizeHint">The minimum length of the span.</param>
//     /// <returns>A span of at least the specified length.</returns>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public Span<T> GetSpan(int sizeHint = 0)
//     {
//         CheckAndResizeBuffer(sizeHint);
//         return _buffer.Slice(_written);
//     }

//     /// <summary>
//     /// Gets a memory of at least the specified length.
//     /// </summary>
//     /// <param name="sizeHint">The minimum length of the memory.</param>
//     /// <returns>A memory of at least the specified length.</returns>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public Memory<T> GetMemory(int sizeHint = 0)
//     {
//         throw new NotSupportedException("SpanBufferWriter does not support GetMemory");
//     }

//     /// <summary>
//     /// Writes the specified value to the buffer.
//     /// </summary>
//     /// <param name="value">The value to write.</param>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void Write(T value)
//     {
//         CheckAndResizeBuffer(1);
//         _buffer[_written++] = value;
//     }

//     /// <summary>
//     /// Writes the specified span to the buffer.
//     /// </summary>
//     /// <param name="source">The span to write.</param>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void Write(ReadOnlySpan<T> source)
//     {
//         if (source.IsEmpty)
//             return;

//         CheckAndResizeBuffer(source.Length);
//         source.CopyTo(_buffer.Slice(_written));
//         _written += source.Length;
//     }

//     /// <summary>
//     /// Clears the buffer and resets the writer.
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void Clear()
//     {
//         _written = 0;
//     }

//     /// <summary>
//     /// Seeks to the specified position in the buffer.
//     /// </summary>
//     /// <param name="offset">The offset to seek to.</param>
//     /// <param name="origin">The origin of the seek operation.</param>
//     /// <returns>The new position in the buffer.</returns>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public int Seek(int offset, SeekOrigin origin)
//     {
//         int newPosition;
//         switch (origin)
//         {
//             case SeekOrigin.Begin:
//                 newPosition = offset;
//                 break;
//             case SeekOrigin.Current:
//                 newPosition = _written + offset;
//                 break;
//             case SeekOrigin.End:
//                 newPosition = _buffer.Length + offset;
//                 break;
//             default:
//                 throw new ArgumentOutOfRangeException(nameof(origin));
//         }

//         if (newPosition < 0 || newPosition > _buffer.Length)
//             throw new ArgumentOutOfRangeException(nameof(offset));

//         _written = newPosition;
//         return _written;
//     }

//     /// <summary>
//     /// Checks if the buffer has enough space for the specified size hint and throws if not.
//     /// </summary>
//     /// <param name="sizeHint">The size hint.</param>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     private void CheckAndResizeBuffer(int sizeHint)
//     {
//         if (sizeHint < 0)
//             throw new ArgumentOutOfRangeException(nameof(sizeHint));

//         if (sizeHint == 0)
//             sizeHint = 1;

//         if (_written > _buffer.Length - sizeHint)
//             throw new InvalidOperationException("Not enough space in buffer");
//     }
// }
