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
/// Static helper methods for writing binary data to buffers.
/// </summary>
internal static class BufferWriteHelper
{
    public static void WriteByte(IBufferWriter<byte> writer, byte value)
    {
        var span = writer.GetSpan(1);
        span[0] = value;
        writer.Advance(1);
    }

    public static void WriteBytes(IBufferWriter<byte> writer, ReadOnlySpan<byte> bytes)
    {
        var span = writer.GetSpan(bytes.Length);
        bytes.CopyTo(span);
        writer.Advance(bytes.Length);
    }

    public static void WriteBool(IBufferWriter<byte> writer, bool value)
    {
        WriteByte(writer, value ? (byte)1 : (byte)0);
    }

    public static void WriteInt16(IBufferWriter<byte> writer, short value)
    {
        var span = writer.GetSpan(2);
        BinaryPrimitives.WriteInt16LittleEndian(span, value);
        writer.Advance(2);
    }

    public static void WriteInt32(IBufferWriter<byte> writer, int value)
    {
        var span = writer.GetSpan(4);
        BinaryPrimitives.WriteInt32LittleEndian(span, value);
        writer.Advance(4);
    }

    public static void WriteInt64(IBufferWriter<byte> writer, long value)
    {
        var span = writer.GetSpan(8);
        BinaryPrimitives.WriteInt64LittleEndian(span, value);
        writer.Advance(8);
    }

    public static void WriteSingle(IBufferWriter<byte> writer, float value)
    {
        var span = writer.GetSpan(4);
        BinaryPrimitives.WriteSingleLittleEndian(span, value);
        writer.Advance(4);
    }

    public static void WriteDouble(IBufferWriter<byte> writer, double value)
    {
        var span = writer.GetSpan(8);
        BinaryPrimitives.WriteDoubleLittleEndian(span, value);
        writer.Advance(8);
    }

    public static void WriteNullTerminatedString(IBufferWriter<byte> writer, ReadOnlySpan<char> value, Encoding encoding)
    {
        var byteCount = encoding.GetMaxByteCount(value.Length);
        var span = writer.GetSpan(byteCount + 1);
        var written = encoding.GetBytes(value, span);
        span[written] = 0; // null terminator
        writer.Advance(written + 1);
    }
}

/// <summary>
/// A struct-based serialization writer for KDB data types that replaces the KWriter API.
/// </summary>
public struct KSerializationWriter
{
    private readonly IBufferWriter<byte> _writer;
    private readonly Stack<WriteStackFrame> _stack;

    public struct WriteStackFrame
    {
        public KType TypeStamp;
        public byte? Attributes;
        public int? ListLength;
        public KType? AtomTypeStamp;
    }

    /// <summary>
    /// Gets a value indicating whether the system architecture is little-endian.
    /// </summary>
    public bool IsLittleEndian => BitConverter.IsLittleEndian;

    /// <summary>
    /// Gets or sets the text encoding used for string serialization.
    /// </summary>
    public Encoding TextEncoding { get; set; }

    /// <summary>
    /// Gets or sets the protocol version.
    /// </summary>
    public byte ProtocolVersion { get; set; }

    /// <summary>
    /// Gets or sets the cancellation token for this serialization operation.
    /// </summary>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Initializes a new instance of the KSerializationWriter struct.
    /// </summary>
    /// <param name="writer">The buffer writer to write to.</param>
    public KSerializationWriter(IBufferWriter<byte> writer)
    {
        _writer = writer;
        _stack = new Stack<WriteStackFrame>();
        TextEncoding = Encoding.UTF8;
        ProtocolVersion = KConstant.ClientProtocolVersion;
        CancellationToken = default;
    }

    /// <summary>
    /// Initializes a new instance of the KSerializationWriter struct with the same settings as this one,
    /// but with its own buffer writer.
    /// </summary>
    /// <param name="writer">The writer to use for the new instance.</param>
    /// <returns>The new writer.</returns>
    public KSerializationWriter Clone(IBufferWriter<byte> writer) => new KSerializationWriter(writer)
    {
        TextEncoding = this.TextEncoding,
        ProtocolVersion = this.ProtocolVersion,
        CancellationToken = this.CancellationToken,
    };

    /// <summary>
    /// Ensures everything previously written has been flushed to the underlying IBufferWriter.
    /// </summary>
    public void Flush()
    {
        // IBufferWriter doesn't have a Flush method, but we can ensure all data is committed
        // by calling GetSpan(0) which forces any pending writes to be committed
        _writer.GetSpan(0);
    }

    public WriteStackFrame GetCurrentFrame()
    {
        return _stack.Count > 0 ? _stack.Peek() :
            throw new InvalidOperationException("No type is being written.");
    }

    public void BeginWriteType(KType type)
    {
        TryWriteTypeStamp(type);
    }

    public void EndWriteType()
    {
        if (_stack.Count == 0)
        {
            throw new InvalidOperationException("No type is being written.");
        }
        _stack.Pop();
    }

    public bool TryWriteTypeStamp(KType kType)
    {
        if (_stack.Count > 0)
        {
            var currentFrame = GetCurrentFrame();
            var isAtomList = currentFrame.AtomTypeStamp is not null;

            if (isAtomList)
            {
                // When writing an atom list, the only valid type is the its atom type and the type stamp should not be written.
                if (currentFrame.AtomTypeStamp != kType)
                {
                    throw new InvalidOperationException($"Invalid write type {kType} in atom list {currentFrame.TypeStamp}. expected {currentFrame.AtomTypeStamp}.");
                }
                return false;
            }
        }

        WriteTypeStamp(kType);
        _stack.Push(new WriteStackFrame { TypeStamp = kType });
        return true;
    }

    public void WriteTypeStamp(KType kType)
    {
        // Protocol validation
        var protocolVersion = ProtocolVersion;
        ThrowIfUsingKTypePreSupportedProtocolVersion(kType, KType.Timestamp, 1, protocolVersion);
        ThrowIfUsingKTypePreSupportedProtocolVersion(kType, KType.TimeSpan, 1, protocolVersion);
        ThrowIfUsingKTypePreSupportedProtocolVersion(kType, KType.Guid, 3, protocolVersion);

        BufferWriteHelper.WriteByte(_writer, (byte)kType);
    }

    public static void ThrowIfUsingKTypePreSupportedProtocolVersion(KType kType, KType target, byte minimalSupportedProtocolVersion, byte currentProtocolVersion)
    {
        var beingChecked = kType.IsAtomList() ? kType.GetUnderlyingType() : kType;
        if (beingChecked == target && currentProtocolVersion < minimalSupportedProtocolVersion)
        {
            throw new NotSupportedException($"KType {target} is not supported in protocol version {currentProtocolVersion}, minimal supported protocol version is {minimalSupportedProtocolVersion}.");
        }
    }

    // WriteXXX methods for KDB Atom types

    /// <summary>
    /// Writes a boolean value.
    /// </summary>
    /// <param name="value">The boolean value to write.</param>
    public void WriteBoolean(bool value)
    {
        BeginWriteType(KType.Boolean);
        BufferWriteHelper.WriteBool(_writer, value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a GUID value.
    /// </summary>
    /// <param name="value">The GUID value to write.</param>
    public void WriteGuid(Guid value)
    {
        BeginWriteType(KType.Guid);
        if (BitConverter.IsLittleEndian)
        {
            SerializationHelper.FlipGuidTop3Parts(ref value);
        }
        var span = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1));
        BufferWriteHelper.WriteBytes(_writer, span);
        EndWriteType();
    }

    /// <summary>
    /// Writes a byte value.
    /// </summary>
    /// <param name="value">The byte value to write.</param>
    public void WriteByte(byte value)
    {
        BeginWriteType(KType.Byte);
        BufferWriteHelper.WriteByte(_writer, value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KShort value.
    /// </summary>
    /// <param name="value">The KShort value to write.</param>
    public void WriteShort(KShort value)
    {
        BeginWriteType(KType.Short);
        BufferWriteHelper.WriteInt16(_writer, value.Value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KInt value.
    /// </summary>
    /// <param name="value">The KInt value to write.</param>
    public void WriteInt(KInt value)
    {
        BeginWriteType(KType.Int);
        BufferWriteHelper.WriteInt32(_writer, value.Value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KLong value.
    /// </summary>
    /// <param name="value">The KLong value to write.</param>
    public void WriteLong(KLong value)
    {
        BeginWriteType(KType.Long);
        BufferWriteHelper.WriteInt64(_writer, value.Value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KReal value.
    /// </summary>
    /// <param name="value">The KReal value to write.</param>
    public void WriteReal(KReal value)
    {
        BeginWriteType(KType.Real);
        BufferWriteHelper.WriteSingle(_writer, value.Value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KFloat value.
    /// </summary>
    /// <param name="value">The KFloat value to write.</param>
    public void WriteFloat(KFloat value)
    {
        BeginWriteType(KType.Float);
        BufferWriteHelper.WriteDouble(_writer, value.Value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KChar value.
    /// </summary>
    /// <param name="value">The KChar value to write.</param>
    public void WriteChar(KChar value)
    {
        BeginWriteType(KType.Char);
        BufferWriteHelper.WriteByte(_writer, (byte)value.Value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a symbol value.
    /// </summary>
    /// <param name="value">The symbol value to write.</param>
    public void WriteSymbol(ReadOnlySpan<char> value)
    {
        BeginWriteType(KType.Symbol);
        BufferWriteHelper.WriteNullTerminatedString(_writer, value, TextEncoding);
        EndWriteType();
    }

    /// <summary>
    /// Writes a symbol value with specified encoding.
    /// </summary>
    /// <param name="value">The symbol value to write.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void WriteSymbol(ReadOnlySpan<char> value, Encoding encoding)
    {
        BeginWriteType(KType.Symbol);
        BufferWriteHelper.WriteNullTerminatedString(_writer, value, encoding);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KTimestamp value.
    /// </summary>
    /// <param name="value">The KTimestamp value to write.</param>
    public void WriteTimestamp(KTimestamp value)
    {
        BeginWriteType(KType.Timestamp);
        BufferWriteHelper.WriteInt64(_writer, value.Value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KMonth value.
    /// </summary>
    /// <param name="value">The KMonth value to write.</param>
    public void WriteMonth(KMonth value)
    {
        BeginWriteType(KType.Month);
        BufferWriteHelper.WriteInt32(_writer, value.Value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KDate value.
    /// </summary>
    /// <param name="value">The KDate value to write.</param>
    public void WriteDate(KDate value)
    {
        BeginWriteType(KType.Date);
        BufferWriteHelper.WriteInt32(_writer, value.Value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KDateTime value.
    /// </summary>
    /// <param name="value">The KDateTime value to write.</param>
    public void WriteDateTime(KDateTime value)
    {
        BeginWriteType(KType.DateTime);
        BufferWriteHelper.WriteDouble(_writer, value.Value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KTimeSpan value.
    /// </summary>
    /// <param name="value">The KTimeSpan value to write.</param>
    public void WriteTimeSpan(KTimeSpan value)
    {
        BeginWriteType(KType.TimeSpan);
        BufferWriteHelper.WriteInt64(_writer, value.Value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KMinute value.
    /// </summary>
    /// <param name="value">The KMinute value to write.</param>
    public void WriteMinute(KMinute value)
    {
        BeginWriteType(KType.Minute);
        BufferWriteHelper.WriteInt32(_writer, value.Value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KSecond value.
    /// </summary>
    /// <param name="value">The KSecond value to write.</param>
    public void WriteSecond(KSecond value)
    {
        BeginWriteType(KType.Second);
        BufferWriteHelper.WriteInt32(_writer, value.Value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a KTime value.
    /// </summary>
    /// <param name="value">The KTime value to write.</param>
    public void WriteTime(KTime value)
    {
        BeginWriteType(KType.Time);
        BufferWriteHelper.WriteInt32(_writer, value.Value);
        EndWriteType();
    }

    // StartWriteXXX and EndWriteXXX methods for KDB Collection types

    /// <summary>
    /// Starts writing a list with the specified type, length, and attributes.
    /// </summary>
    /// <param name="listType">The type of the list.</param>
    /// <param name="length">The length of the list.</param>
    /// <param name="attributes">The attributes of the list (default: 0).</param>
    public void StartWriteList(KType listType, int length, byte attributes = 0)
    {
        BeginWriteType(listType);
        BufferWriteHelper.WriteByte(_writer, attributes);
        BufferWriteHelper.WriteInt32(_writer, length);
        // Refill the stack frame with the array type and length.
        var frame = GetCurrentFrame();
        frame.Attributes = attributes;
        frame.ListLength = length;
        if (listType.IsAtomList())
            frame.AtomTypeStamp = listType.Neg();
        _stack.Pop();
        _stack.Push(frame);
    }

    /// <summary>
    /// Ends writing a list.
    /// </summary>
    public void EndWriteList()
    {
        EndWriteType();
    }

    /// <summary>
    /// Starts writing a dictionary.
    /// </summary>
    public void StartWriteDictionary()
    {
        BeginWriteType(KType.Dictionary);
    }

    /// <summary>
    /// Ends writing a dictionary.
    /// </summary>
    public void EndWriteDictionary()
    {
        EndWriteType();
    }

    /// <summary>
    /// Starts writing a table with the specified attributes.
    /// </summary>
    /// <param name="attributes">The attributes of the table (default: 0).</param>
    public void StartWriteTable(byte attributes = 0)
    {
        BeginWriteType(KType.Table);
        BufferWriteHelper.WriteByte(_writer, attributes);
        WriteTypeStamp(KType.Dictionary);
        // Refill the stack frame with the array type and length.
        var frame = GetCurrentFrame();
        frame.Attributes = attributes;
        _stack.Pop();
        _stack.Push(frame);
    }

    /// <summary>
    /// Ends writing a table.
    /// </summary>
    public void EndWriteTable()
    {
        EndWriteType();
    }

    /// <summary>
    /// Writes a character list (string) with the specified encoding and attributes.
    /// </summary>
    /// <param name="value">The string value to write.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <param name="attributes">The attributes of the character list (default: 0).</param>
    public void WriteCharList(string value, Encoding encoding, byte attributes = 0)
    {
        var bytes = encoding.GetBytes(value);
        StartWriteList(KType.CharList, bytes.Length, attributes);
        BufferWriteHelper.WriteBytes(_writer, bytes);
        EndWriteList();
    }

    /// <summary>
    /// Writes a character list (string) using the default text encoding and attributes.
    /// </summary>
    /// <param name="value">The string value to write.</param>
    /// <param name="attributes">The attributes of the character list (default: 0).</param>
    public void WriteCharList(string value, byte attributes = 0)
    {
        WriteCharList(value, TextEncoding, attributes);
    }

    public void WriteUnaryPrimitive(UnaryPrimitive value)
    {
        BeginWriteType(KType.UnaryPrimitive);
        BufferWriteHelper.WriteByte(_writer, (byte)value);
        EndWriteType();
    }

    /// <summary>
    /// Writes a unit value.
    /// </summary>
    public void WriteUnit()
    {
        BeginWriteType(KType.UnaryPrimitive);
        BufferWriteHelper.WriteByte(_writer, 0);
        EndWriteType();
    }

    // Make it extension method to allow writing strings with specific encoding
    internal void WriteString(string username, Encoding textEncoding) => throw new NotImplementedException();
    internal void Write<T>(T value) where T: unmanaged => throw new NotImplementedException();
    internal void WriteInt32(int value) => throw new NotImplementedException();
}
