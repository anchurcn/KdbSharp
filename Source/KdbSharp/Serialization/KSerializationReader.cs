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
    public struct ReadStackFrame
    {
        public KType TypeStamp;
        public byte? Attributes;
        public int? ListLength;
        public KType? AtomTypeStamp;
        public KType? LastNestedType;
    }

    private SequenceReader<byte> _reader;
    private readonly Stack<ReadStackFrame> _stack;

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
    /// Gets the next type stamp that will be read without consuming it.
    /// </summary>
    public KType NextTypeStamp => GetNextTypeStamp();

    private KType GetNextTypeStamp()
    {
        return IsNestedRead(out var frame) && frame.TypeStamp.IsAtomList() ?
            frame.AtomTypeStamp.GetValueOrDefault() : PeekFromReader(_reader);

        static KType PeekFromReader(SequenceReader<byte> reader)
        {
            if (!reader.TryPeek(out byte typeByte))
            {
                throw new InvalidOperationException("Cannot peek type stamp: end of buffer reached.");
            }
            return (KType)typeByte;
        }
    }

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

    public KType? LastNestedType => GetCurrentFrame().LastNestedType;

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

    public ReadStackFrame GetCurrentFrame()
    {
        if (!IsNestedRead(out ReadStackFrame currentFrame))
        {
            throw new InvalidOperationException("Is not reading any type currently.");
        }
        return currentFrame;
    }

    private readonly bool IsNestedRead(out ReadStackFrame currentFrame)
    {
        if (_stack.Count > 0)
        {
            currentFrame = _stack.Peek();
            return true;
        }
        currentFrame = default;
        return false;
    }

    #region Container read

    private void BeginReadContainerType(ReadStackFrame frame)
    {
        _stack.Push(frame);
    }

    private readonly void EndReadContainerType()
    {
        var last = _stack.Pop();
        if (IsNestedRead(out var current))
        {
            current.LastNestedType = last.TypeStamp;
            _stack.Pop();
            _stack.Push(current);
        }
    }

    /// <summary>
    /// Starts reading a list. The list type is determined by reading the type stamp.
    /// Metadata is stored in the current frame.
    /// </summary>
    public void StartReadList()
    {
        var listType = ReadTypeStamp();
        var attributes = Read<byte>();
        var length = ReadInt32();

        var frame = new ReadStackFrame()
        {
            TypeStamp = listType,
            Attributes = attributes,
            ListLength = length,
            AtomTypeStamp = listType.IsAtomList() ? listType.Neg() : null,
            LastNestedType = null,
        };

        BeginReadContainerType(frame);
    }

    /// <summary>
    /// Ends reading a list.
    /// </summary>
    public void EndReadList()
    {
        EndReadContainerType();
    }

    /// <summary>
    /// Starts reading a dictionary.
    /// </summary>
    public void StartReadDictionary()
    {
        var type = ReadTypeStamp();
        var frame = new ReadStackFrame()
        {
            TypeStamp = type,
            Attributes = null,
            ListLength = null,
            AtomTypeStamp = null,
            LastNestedType = null,
        };
        BeginReadContainerType(frame);
    }

    /// <summary>
    /// Ends reading a dictionary.
    /// </summary>
    public void EndReadDictionary()
    {
        EndReadContainerType();
    }

    /// <summary>
    /// Starts reading a table.
    /// Metadata is stored in the current frame.
    /// </summary>
    public void StartReadTable()
    {
        var type = ReadTypeStamp();
        var attr = Read<byte>();
        var dictType = ReadTypeStamp();
        if (dictType != KType.Dictionary)
        {
            throw new InvalidOperationException($"Expected Dictionary type stamp in table, but got {dictType}.");
        }

        var frame = new ReadStackFrame()
        {
            TypeStamp = type,
            Attributes = attr,
            ListLength = null,
            AtomTypeStamp = null,
            LastNestedType = null,
        };

        BeginReadContainerType(frame);
    }

    /// <summary>
    /// Ends reading a table.
    /// </summary>
    public void EndReadTable()
    {
        EndReadContainerType();
    }

    #endregion

    #region Atom read

    /// <summary>
    /// Begins reading an atom. If not reading an atom list, reads the type stamp.
    /// </summary>
    private KType BeginReadAtom()
    {
        if (IsNestedRead(out var frame) && frame.TypeStamp.IsAtomList())
        {
            return frame.AtomTypeStamp.GetValueOrDefault();
        }
        return ReadTypeStamp();
    }

    private void EndReadAtom(KType lastAtom)
    {
        if (IsNestedRead(out var current))
        {
            current.LastNestedType = lastAtom;
            _stack.Pop();
            _stack.Push(current);
        }
    }

    public bool ReadBoolean()
    {
        var type = BeginReadAtom();
        var value = Read<byte>() != 0;
        EndReadAtom(type);
        return value;
    }

    public byte ReadByte()
    {
        var type = BeginReadAtom();
        var value = _reader.ReadByte();
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a GUID value.
    /// </summary>
    /// <returns>The GUID value.</returns>
    public Guid ReadGuid()
    {
        var type = BeginReadAtom();
        var result = Read<Guid>();
        if (IsLittleEndian)
        {
            SerializationHelper.FlipGuidTop3Parts(ref result);
        }
        EndReadAtom(type);
        return result;
    }

    /// <summary>
    /// Reads a KShort value.
    /// </summary>
    /// <returns>The KShort value.</returns>
    public KShort ReadShort()
    {
        var type = BeginReadAtom();
        var value = new KShort(ReadInt16());
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a KInt value.
    /// </summary>
    /// <returns>The KInt value.</returns>
    public KInt ReadInt()
    {
        var type = BeginReadAtom();
        var value = new KInt(ReadInt32());
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a KLong value.
    /// </summary>
    /// <returns>The KLong value.</returns>
    public KLong ReadLong()
    {
        var type = BeginReadAtom();
        var value = new KLong(ReadInt64());
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a KReal value.
    /// </summary>
    /// <returns>The KReal value.</returns>
    public KReal ReadReal()
    {
        var type = BeginReadAtom();
        var value = new KReal(ReadSingle());
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a KFloat value.
    /// </summary>
    /// <returns>The KFloat value.</returns>
    public KFloat ReadFloat()
    {
        var type = BeginReadAtom();
        var value = new KFloat(ReadDouble());
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a KChar value.
    /// </summary>
    /// <returns>The KChar value.</returns>
    public KChar ReadChar()
    {
        var type = BeginReadAtom();
        var value = new KChar((sbyte)_reader.ReadByte());
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a symbol value.
    /// </summary>
    /// <returns>The symbol value as a string.</returns>
    public string ReadSymbol()
    {
        var type = BeginReadAtom();
        var value = ReadNullTerminatedString();
        EndReadAtom(type);
        return value;
    }

    // ReadXXX methods for KDB Temporal types

    /// <summary>
    /// Reads a KTimestamp value.
    /// </summary>
    /// <returns>The KTimestamp value.</returns>
    public KTimestamp ReadTimestamp()
    {
        var type = BeginReadAtom();
        var value = new KTimestamp(ReadInt64());
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a KDate value.
    /// </summary>
    /// <returns>The KDate value.</returns>
    public KDate ReadDate()
    {
        var type = BeginReadAtom();
        var value = new KDate(ReadInt32());
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a KTime value.
    /// </summary>
    /// <returns>The KTime value.</returns>
    public KTime ReadTime()
    {
        var type = BeginReadAtom();
        var value = new KTime(ReadInt32());
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a KDateTime value.
    /// </summary>
    /// <returns>The KDateTime value.</returns>
    public KDateTime ReadDateTime()
    {
        var type = BeginReadAtom();
        var value = new KDateTime(ReadDouble());
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a KTimeSpan value.
    /// </summary>
    /// <returns>The KTimeSpan value.</returns>
    public KTimeSpan ReadTimeSpan()
    {
        var type = BeginReadAtom();
        var value = new KTimeSpan(ReadInt64());
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a KMonth value.
    /// </summary>
    /// <returns>The KMonth value.</returns>
    public KMonth ReadMonth()
    {
        var type = BeginReadAtom();
        var value = new KMonth(ReadInt32());
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a KMinute value.
    /// </summary>
    /// <returns>The KMinute value.</returns>
    public KMinute ReadMinute()
    {
        var type = BeginReadAtom();
        var value = new KMinute(ReadInt32());
        EndReadAtom(type);
        return value;
    }

    /// <summary>
    /// Reads a KSecond value.
    /// </summary>
    /// <returns>The KSecond value.</returns>
    public KSecond ReadSecond()
    {
        var type = BeginReadAtom();
        var value = new KSecond(ReadInt32());
        EndReadAtom(type);
        return value;
    }

    #endregion

    #region Other Ktype read

    public void BeginReadType()
    {
        var type = ReadTypeStamp();
        var frame = new ReadStackFrame()
        {
            TypeStamp = type,
            Attributes = null,
            ListLength = null,
            AtomTypeStamp = null,
            LastNestedType = null,
        };
        _stack.Push(frame);
    }

    public void EndReadType()
    {
        var endedFrame = _stack.Pop();
        if (IsNestedRead(out var current))
        {
            current.LastNestedType = endedFrame.TypeStamp;
            _stack.Pop();
            _stack.Push(current);
        }
    }

    public UnaryPrimitive ReadUnaryPrimitive()
    {
        BeginReadType();
        var value = Read<UnaryPrimitive>();
        EndReadType();
        return value;
    }
    #endregion

    #region Read helper

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

        byte[] buffer = new byte[length];
        if (!_reader.TryCopyTo(buffer))
        {
            throw new InvalidOperationException($"Cannot read {length} bytes: not enough bytes in buffer.");
        }
        _reader.Advance(length);
        return buffer;
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
