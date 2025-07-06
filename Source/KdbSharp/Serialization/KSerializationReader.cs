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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using KdbSharp.Types;

namespace KdbSharp.Serialization;

/// <summary>
/// A struct-based reader for KDB data types.
/// </summary>
public ref struct KSerializationReader
{
    private SequenceReader<byte> _reader;
    private readonly Stack<ReadStackFrame> _stack;

    public struct ReadStackFrame
    {
        public KType TypeStamp;
        public byte? Attributes;
        public int? ListLength;
        public KType? AtomTypeStamp;
        public KType? LastNestedType;
    }

    /// <summary>
    /// Gets or sets the text encoding used for string deserialization.
    /// </summary>
    public Encoding TextEncoding { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the data being read is little-endian.
    /// </summary>
    public bool IsLittleEndian { get; set; }

    /// <summary>
    /// Gets or sets the cancellation token for this deserialization operation.
    /// </summary>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Gets the current position of the reader within the sequence.
    /// </summary>
    public SequencePosition Position => _reader.Position;

    /// <summary>
    /// Gets the number of bytes consumed by the reader.
    /// </summary>
    public long Consumed => _reader.Consumed;

    /// <summary>
    /// Gets a value indicating whether the reader is at the end of the sequence.
    /// </summary>
    public bool End => _reader.End;

    /// <summary>
    /// Gets the remaining bytes in the sequence.
    /// </summary>
    public long Remaining => _reader.Remaining;

    /// <summary>
    /// Initializes a new instance of the KSerializationReader struct.
    /// </summary>
    /// <param name="memory">The buffer to read from.</param>
    public KSerializationReader(ReadOnlyMemory<byte> memory)
    {
        _reader = new SequenceReader<byte>(memory);
        _stack = new Stack<ReadStackFrame>();
        TextEncoding = Encoding.UTF8;
        CancellationToken = default;
        IsLittleEndian = false;
    }

    private ReadStackFrame GetCurrentFrame()
    {
        return _stack.Count > 0 ? _stack.Peek() :
            throw new InvalidOperationException("No type is being read.");
    }
    private bool IsReadingType(out ReadStackFrame frame)
    {
        if (_stack.Count > 0)
        {
            frame = _stack.Peek();
            return true;
        }
        frame = default;
        return false;
    }
    /// <summary>
    /// Gets the next type stamp that will be read without consuming it.
    /// </summary>
    public KType NextTypeStamp
    {
        get
        {
            if (!_reader.TryPeek(out byte typeByte))
            {
                throw new InvalidOperationException("Cannot peek type stamp: end of buffer reached.");
            }
            return (KType)typeByte;
        }
    }

    /// <summary>
    /// Gets the current type stamp being read.
    /// </summary>
    public KType TypeStamp => GetCurrentFrame().TypeStamp;

    /// <summary>
    /// Gets the current attributes being read.
    /// </summary>
    public byte? Attributes => GetCurrentFrame().Attributes;

    /// <summary>
    /// Gets the current list length being read.
    /// </summary>
    public int? ListLength => GetCurrentFrame().ListLength;

    /// <summary>
    /// Gets the current atom type stamp (for atom lists).
    /// </summary>
    public KType? AtomTypeStamp => GetCurrentFrame().AtomTypeStamp;

    #region Static

    /// <summary>
    /// Reads a type stamp from the buffer.
    /// </summary>
    /// <returns>The KType read from the buffer.</returns>
    public KType ReadTypeStamp()
    {
        if (!_reader.TryRead(out byte typeByte))
        {
            throw new InvalidOperationException("Cannot read type stamp: end of buffer reached.");
        }
        return (KType)typeByte;
    }

    /// <summary>
    /// Begins reading a type by consuming the type stamp from the buffer.
    /// </summary>
    /// <returns>The type stamp that was read.</returns>
    public KType BeginReadType()
    {
        var actualType = ReadTypeStamp();

        var frame = new ReadStackFrame
        {
            TypeStamp = actualType
        };
        _stack.Push(frame);
        return actualType;
    }

    /// <summary>
    /// Begins reading an atom. If not reading an atom list, reads the type stamp.
    /// </summary>
    public void BeginReadAtom()
    {
        if (_stack.Count == 0 || !GetCurrentFrame().TypeStamp.IsAtomList())
        {
            ReadTypeStamp();
        }
    }

    /// <summary>
    /// Ends reading the current type.
    /// </summary>
    public void EndReadType()
    {
        if (_stack.Count == 0)
        {
            throw new InvalidOperationException("No type is being read.");
        }
        _stack.Pop();
    }

    /// <summary>
    /// Skips the specified number of bytes in the buffer.
    /// </summary>
    /// <param name="count">The number of bytes to skip.</param>
    public void Skip(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
        }

        if (count > 0)
        {
            _reader.Advance(count);
        }
    }

    #endregion

    #region Container read

    /// <summary>
    /// Starts reading a list. The list type is determined by reading the type stamp.
    /// Metadata is stored in the current frame.
    /// </summary>
    public void StartReadList()
    {
        var listType = BeginReadType();
        var attributes = _reader.ReadByte();
        var length = ReadInt32();

        // Update the stack frame with list information
        var frame = GetCurrentFrame();
        frame.Attributes = attributes;
        frame.ListLength = length;
        if (listType.IsAtomList())
            frame.AtomTypeStamp = listType.Neg();

        _stack.Pop();
        _stack.Push(frame);
    }

    /// <summary>
    /// Ends reading a list.
    /// </summary>
    public void EndReadList()
    {
        EndReadType();
    }

    /// <summary>
    /// Starts reading a dictionary.
    /// </summary>
    public void StartReadDictionary()
    {
        BeginReadType();
    }

    /// <summary>
    /// Ends reading a dictionary.
    /// </summary>
    public void EndReadDictionary()
    {
        EndReadType();
    }

    /// <summary>
    /// Starts reading a table.
    /// Metadata is stored in the current frame.
    /// </summary>
    public void StartReadTable()
    {
        BeginReadType();
        var attributes = _reader.ReadByte();
        var dictType = ReadTypeStamp();
        if (dictType != KType.Dictionary)
        {
            throw new InvalidOperationException($"Expected Dictionary type stamp in table, but got {dictType}.");
        }

        // Update the stack frame with table information
        var frame = GetCurrentFrame();
        frame.Attributes = attributes;
        _stack.Pop();
        _stack.Push(frame);
    }

    /// <summary>
    /// Ends reading a table.
    /// </summary>
    public void EndReadTable()
    {
        EndReadType();
    }

    #endregion

    #region Atom read

    /// <summary>
    /// Reads an unmanaged value from the buffer.
    /// </summary>
    /// <typeparam name="T">The type of the unmanaged value.</typeparam>
    /// <returns>The value read from the buffer.</returns>
    public unsafe T Read<T>() where T : unmanaged
    {
        int size = sizeof(T);
        Span<byte> buffer = stackalloc byte[size];
        if (!_reader.TryCopyTo(buffer))
        {
            throw new InvalidOperationException($"Cannot read {typeof(T).Name}: not enough bytes in buffer.");
        }
        _reader.Advance(size);

        return MemoryMarshal.Read<T>(buffer);
    }

    /// <summary>
    /// Reads a 32-bit integer from the buffer.
    /// </summary>
    /// <returns>The 32-bit integer value.</returns>
    public int ReadInt32()
    {
        var result = Read<int>();
        return IsLittleEndian == BitConverter.IsLittleEndian
            ? result : BinaryPrimitives.ReverseEndianness(result);
    }

    /// <summary>
    /// Reads a single-precision floating-point value from the buffer.
    /// </summary>
    /// <returns>The single-precision floating-point value.</returns>
    public float ReadSingle()
    {
        if (IsLittleEndian == BitConverter.IsLittleEndian)
        {
            return Read<float>();
        }
        else
        {
            var intValue = Read<int>();
            var reversedInt = BinaryPrimitives.ReverseEndianness(intValue);
            return BitConverter.Int32BitsToSingle(reversedInt);
        }
    }

    /// <summary>
    /// Reads a 16-bit integer from the buffer.
    /// </summary>
    /// <returns>The 16-bit integer value.</returns>
    public short ReadInt16()
    {
        var result = Read<short>();
        return IsLittleEndian == BitConverter.IsLittleEndian
            ? result : BinaryPrimitives.ReverseEndianness(result);
    }

    /// <summary>
    /// Reads a 64-bit integer from the buffer.
    /// </summary>
    /// <returns>The 64-bit integer value.</returns>
    public long ReadInt64()
    {
        var result = Read<long>();
        return IsLittleEndian == BitConverter.IsLittleEndian
            ? result : BinaryPrimitives.ReverseEndianness(result);
    }

    /// <summary>
    /// Reads a double-precision floating-point value from the buffer.
    /// </summary>
    /// <returns>The double-precision floating-point value.</returns>
    public double ReadDouble()
    {
        if (IsLittleEndian == BitConverter.IsLittleEndian)
        {
            return Read<double>();
        }
        else
        {
            var longValue = Read<long>();
            var reversedLong = BinaryPrimitives.ReverseEndianness(longValue);
            return BitConverter.Int64BitsToDouble(reversedLong);
        }
    }

    public bool ReadBoolean()
    {
        BeginReadAtom();
        var value = _reader.ReadBool();
        return value;
    }

    public byte ReadByte()
    {
        BeginReadAtom();
        return _reader.ReadByte();
    }

    /// <summary>
    /// Reads a GUID value.
    /// </summary>
    /// <returns>The GUID value.</returns>
    public Guid ReadGuid()
    {
        BeginReadAtom();
        var result = Read<Guid>();
        if (IsLittleEndian)
        {
            SerializationHelper.FlipGuidTop3Parts(ref result);
        }
        return result;
    }

    /// <summary>
    /// Reads a KShort value.
    /// </summary>
    /// <returns>The KShort value.</returns>
    public KShort ReadShort()
    {
        BeginReadAtom();
        var value = new KShort(ReadInt16());
        return value;
    }

    /// <summary>
    /// Reads a KInt value.
    /// </summary>
    /// <returns>The KInt value.</returns>
    public KInt ReadInt()
    {
        BeginReadAtom();
        var value = new KInt(ReadInt32());
        return value;
    }

    /// <summary>
    /// Reads a KLong value.
    /// </summary>
    /// <returns>The KLong value.</returns>
    public KLong ReadLong()
    {
        BeginReadAtom();
        var value = new KLong(ReadInt64());
        return value;
    }

    /// <summary>
    /// Reads a KReal value.
    /// </summary>
    /// <returns>The KReal value.</returns>
    public KReal ReadReal()
    {
        BeginReadAtom();
        var value = new KReal(ReadSingle());
        return value;
    }

    /// <summary>
    /// Reads a KFloat value.
    /// </summary>
    /// <returns>The KFloat value.</returns>
    public KFloat ReadFloat()
    {
        BeginReadAtom();
        var value = new KFloat(ReadDouble());
        return value;
    }

    /// <summary>
    /// Reads a KChar value.
    /// </summary>
    /// <returns>The KChar value.</returns>
    public KChar ReadChar()
    {
        BeginReadAtom();
        var value = new KChar((sbyte)_reader.ReadByte());
        return value;
    }

    /// <summary>
    /// Reads a symbol value.
    /// </summary>
    /// <returns>The symbol value as a string.</returns>
    public string ReadSymbol()
    {
        BeginReadAtom();
        var value = ReadNullTerminatedString();
        return value;
    }

    // ReadXXX methods for KDB Temporal types

    /// <summary>
    /// Reads a KTimestamp value.
    /// </summary>
    /// <returns>The KTimestamp value.</returns>
    public KTimestamp ReadTimestamp()
    {
        BeginReadAtom();
        var value = new KTimestamp(ReadInt64());
        return value;
    }

    /// <summary>
    /// Reads a KDate value.
    /// </summary>
    /// <returns>The KDate value.</returns>
    public KDate ReadDate()
    {
        BeginReadAtom();
        var value = new KDate(ReadInt32());
        return value;
    }

    /// <summary>
    /// Reads a KTime value.
    /// </summary>
    /// <returns>The KTime value.</returns>
    public KTime ReadTime()
    {
        BeginReadAtom();
        var value = new KTime(ReadInt32());
        return value;
    }

    /// <summary>
    /// Reads a KDateTime value.
    /// </summary>
    /// <returns>The KDateTime value.</returns>
    public KDateTime ReadDateTime()
    {
        BeginReadAtom();
        var value = new KDateTime(ReadDouble());
        return value;
    }

    /// <summary>
    /// Reads a KTimeSpan value.
    /// </summary>
    /// <returns>The KTimeSpan value.</returns>
    public KTimeSpan ReadTimeSpan()
    {
        BeginReadAtom();
        var value = new KTimeSpan(ReadInt64());
        return value;
    }

    /// <summary>
    /// Reads a KMonth value.
    /// </summary>
    /// <returns>The KMonth value.</returns>
    public KMonth ReadMonth()
    {
        BeginReadAtom();
        var value = new KMonth(ReadInt32());
        return value;
    }

    /// <summary>
    /// Reads a KMinute value.
    /// </summary>
    /// <returns>The KMinute value.</returns>
    public KMinute ReadMinute()
    {
        BeginReadAtom();
        var value = new KMinute(ReadInt32());
        return value;
    }

    /// <summary>
    /// Reads a KSecond value.
    /// </summary>
    /// <returns>The KSecond value.</returns>
    public KSecond ReadSecond()
    {
        BeginReadAtom();
        var value = new KSecond(ReadInt32());
        return value;
    }

    #endregion

    #region Other read

    public UnaryPrimitive ReadUnaryPrimitive()
    {
        BeginReadType();
        var value = Read<UnaryPrimitive>();
        EndReadType();
        return value;
    }
    #endregion

    #region Buffer read

    /// <summary>
    /// Reads a span of bytes from the buffer.
    /// </summary>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>The bytes read from the buffer.</returns>
    public ReadOnlySpan<byte> ReadBytes(int length)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative.");
        }

        if (length == 0)
        {
            return ReadOnlySpan<byte>.Empty;
        }

        // For small lengths, use stack allocation
        if (length <= 1024)
        {
            Span<byte> buffer = stackalloc byte[length];
            if (!_reader.TryCopyTo(buffer))
            {
                throw new InvalidOperationException($"Cannot read {length} bytes: not enough bytes in buffer.");
            }
            _reader.Advance(length);
            return buffer.ToArray(); // Convert to array to return from method
        }
        else
        {
            // For larger lengths, allocate on heap
            byte[] buffer = new byte[length];
            if (!_reader.TryCopyTo(buffer))
            {
                throw new InvalidOperationException($"Cannot read {length} bytes: not enough bytes in buffer.");
            }
            _reader.Advance(length);
            return buffer;
        }
    }

    /// <summary>
    /// Reads a null-terminated string from the buffer using the specified encoding.
    /// </summary>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The string read from the buffer.</returns>
    public string ReadNullTerminatedString(Encoding encoding)
    {
        var bytes = GetNullTerminatedBytes();
        // omit the null terminator
        return encoding.GetString(bytes[..^1]);
    }

    /// <summary>
    /// Reads a null-terminated string from the buffer using the default text encoding.
    /// </summary>
    /// <returns>The string read from the buffer.</returns>
    public string ReadNullTerminatedString()
    {
        return ReadNullTerminatedString(TextEncoding);
    }

    /// <summary>
    /// Reads a string of the specified byte length from the buffer using the specified encoding.
    /// </summary>
    /// <param name="byteLength">The number of bytes to read.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The string read from the buffer.</returns>
    public string ReadString(int byteLength, Encoding encoding)
    {
        var bytes = ReadBytes(byteLength);
        return encoding.GetString(bytes);
    }

    /// <summary>
    /// Reads a string of the specified byte length from the buffer using the default text encoding.
    /// </summary>
    /// <param name="byteLength">The number of bytes to read.</param>
    /// <returns>The string read from the buffer.</returns>
    public string ReadString(int byteLength)
    {
        return ReadString(byteLength, TextEncoding);
    }

    /// <summary>
    /// Gets null-terminated bytes from the buffer.
    /// </summary>
    /// <returns>The bytes up to the null terminator.</returns>
    public ReadOnlySpan<byte> GetNullTerminatedBytes()
    {

        var index = _reader.UnreadSpan.IndexOf((byte)0);
        var result = index >= 0
            ? _reader.UnreadSpan.Slice(0, index + 1)
            : throw new NotImplementedException("Out of bounds reading for null-terminated string not implemented.");
        _reader.Advance(result.Length);
        return result;
    }

    #endregion

}
