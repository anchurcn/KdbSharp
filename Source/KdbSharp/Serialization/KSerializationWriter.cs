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
    /// Gets a value indicating whether the system architecture is little-endian.
    /// </summary>
    public bool IsLittleEndian => BitConverter.IsLittleEndian;

    public IBufferWriter<byte> BufferWriter => _writer;


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

    private WriteStackFrame GetCurrentFrame()
    {
        return _stack.Count > 0 ? _stack.Peek() :
            throw new InvalidOperationException("No type is being written.");
    }

    private bool IsWriting(out WriteStackFrame frame)
    {
        if (_stack.Count == 0)
        {
            frame = default;
            return false;
        }
        frame = _stack.Peek();
        return true;
    }

    public bool BeginWriteType(KType type)
    {
        return TryWriteTypeStamp(type);
    }

    public void EndWriteType()
    {
        if (_stack.Count == 0)
        {
            throw new InvalidOperationException("No type is being written.");
        }

        _stack.Pop();
    }

    private bool TryWriteTypeStamp(KType kType)
    {
        if (IsWriting(out var frame) && frame.TypeStamp.IsAtomList())
        {
            // When writing an atom list, the only valid type is the its atom type and the type stamp should not be written.
            if (frame.AtomTypeStamp != kType)
            {
                throw new InvalidOperationException($"Invalid write type {kType} in atom list {frame.TypeStamp}. expected {frame.AtomTypeStamp}.");
            }

            return false;
        }
        else
        {
            WriteTypeStamp(kType);
            _stack.Push(new WriteStackFrame { TypeStamp = kType });
            return true;
        }
    }

    private void WriteTypeStamp(KType kType)
    {
        static void ThrowIfUsingKTypePreSupportedProtocolVersion(KType kType, KType target, byte minimalSupportedProtocolVersion, byte currentProtocolVersion)
        {
            var beingChecked = kType.IsAtomList() ? kType.GetUnderlyingType() : kType;
            if (beingChecked == target && currentProtocolVersion < minimalSupportedProtocolVersion)
            {
                throw new NotSupportedException($"KType {target} is not supported in protocol version {currentProtocolVersion}, minimal supported protocol version is {minimalSupportedProtocolVersion}.");
            }
        }

        // Protocol validation
        var protocolVersion = ProtocolVersion;
        ThrowIfUsingKTypePreSupportedProtocolVersion(kType, KType.Timestamp, 1, protocolVersion);
        ThrowIfUsingKTypePreSupportedProtocolVersion(kType, KType.TimeSpan, 1, protocolVersion);
        ThrowIfUsingKTypePreSupportedProtocolVersion(kType, KType.Guid, 3, protocolVersion);

        _writer.WriteByte((byte)kType);
    }

    #region Atom write

    /// <summary>
    /// Writes a boolean value.
    /// </summary>
    /// <param name="value">The boolean value to write.</param>
    public void WriteBoolean(bool value)
    {
        var needPop = BeginWriteType(KType.Boolean);
        _writer.WriteBool(value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a GUID value.
    /// </summary>
    /// <param name="value">The GUID value to write.</param>
    public void WriteGuid(Guid value)
    {
        var needPop = BeginWriteType(KType.Guid);
        if (BitConverter.IsLittleEndian)
        {
            SerializationHelper.FlipGuidTop3Parts(ref value);
        }

        var span = value.AsSpan();
        _writer.WriteBytes(span);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a byte value.
    /// </summary>
    /// <param name="value">The byte value to write.</param>
    public void WriteByte(byte value)
    {
        var needPop = BeginWriteType(KType.Byte);
        _writer.WriteByte(value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KShort value.
    /// </summary>
    /// <param name="value">The KShort value to write.</param>
    public void WriteShort(KShort value)
    {
        var needPop = BeginWriteType(KType.Short);
        _writer.WriteInt16(value.Value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KInt value.
    /// </summary>
    /// <param name="value">The KInt value to write.</param>
    public void WriteInt(KInt value)
    {
        var needPop = BeginWriteType(KType.Int);
        _writer.WriteInt32(value.Value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KLong value.
    /// </summary>
    /// <param name="value">The KLong value to write.</param>
    public void WriteLong(KLong value)
    {
        var needPop = BeginWriteType(KType.Long);
        _writer.WriteInt64(value.Value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KReal value.
    /// </summary>
    /// <param name="value">The KReal value to write.</param>
    public void WriteReal(KReal value)
    {
        var needPop = BeginWriteType(KType.Real);
        _writer.WriteSingle(value.Value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KFloat value.
    /// </summary>
    /// <param name="value">The KFloat value to write.</param>
    public void WriteFloat(KFloat value)
    {
        var needPop = BeginWriteType(KType.Float);
        _writer.WriteDouble(value.Value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KChar value.
    /// </summary>
    /// <param name="value">The KChar value to write.</param>
    public void WriteChar(KChar value)
    {
        var needPop = BeginWriteType(KType.Char);
        _writer.WriteByte((byte)value.Value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a symbol value with specified encoding.
    /// </summary>
    /// <param name="value">The symbol value to write.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void WriteSymbol(ReadOnlySpan<char> value, Encoding? encoding = null)
    {
        var needPop = BeginWriteType(KType.Symbol);
        _writer.WriteNullTerminatedString(value, encoding ?? TextEncoding);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KTimestamp value.
    /// </summary>
    /// <param name="value">The KTimestamp value to write.</param>
    public void WriteTimestamp(KTimestamp value)
    {
        var needPop = BeginWriteType(KType.Timestamp);
        _writer.WriteInt64(value.Value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KMonth value.
    /// </summary>
    /// <param name="value">The KMonth value to write.</param>
    public void WriteMonth(KMonth value)
    {
        var needPop = BeginWriteType(KType.Month);
        _writer.WriteInt32(value.Value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KDate value.
    /// </summary>
    /// <param name="value">The KDate value to write.</param>
    public void WriteDate(KDate value)
    {
        var needPop = BeginWriteType(KType.Date);
        _writer.WriteInt32(value.Value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KDateTime value.
    /// </summary>
    /// <param name="value">The KDateTime value to write.</param>
    public void WriteDateTime(KDateTime value)
    {
        var needPop = BeginWriteType(KType.DateTime);
        _writer.WriteDouble(value.Value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KTimeSpan value.
    /// </summary>
    /// <param name="value">The KTimeSpan value to write.</param>
    public void WriteTimeSpan(KTimeSpan value)
    {
        var needPop = BeginWriteType(KType.TimeSpan);
        _writer.WriteInt64(value.Value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KMinute value.
    /// </summary>
    /// <param name="value">The KMinute value to write.</param>
    public void WriteMinute(KMinute value)
    {
        var needPop = BeginWriteType(KType.Minute);
        _writer.WriteInt32(value.Value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KSecond value.
    /// </summary>
    /// <param name="value">The KSecond value to write.</param>
    public void WriteSecond(KSecond value)
    {
        var needPop = BeginWriteType(KType.Second);
        _writer.WriteInt32(value.Value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a KTime value.
    /// </summary>
    /// <param name="value">The KTime value to write.</param>
    public void WriteTime(KTime value)
    {
        var needPop = BeginWriteType(KType.Time);
        _writer.WriteInt32(value.Value);
        if (needPop)
            EndWriteType();
    }

    #endregion

    #region Container write

    /// <summary>
    /// Starts writing a list with the specified type, length, and attributes.
    /// </summary>
    /// <param name="listType">The type of the list.</param>
    /// <param name="length">The length of the list.</param>
    /// <param name="attributes">The attributes of the list (default: 0).</param>
    public void StartWriteList(KType listType, int length, byte attributes = 0)
    {
        // TODO: throw if listType is not a list type
        if (!listType.IsList())
        {
            throw new ArgumentException($"Invalid list type: {listType}. Expected a list type.", nameof(listType));
        }

        BeginWriteType(listType);
        _writer.WriteByte(attributes);
        _writer.WriteInt32(length);

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
        _writer.WriteByte(attributes);
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

    #endregion

    #region Other write

    /// <summary>
    /// Writes a character list (string) with the specified encoding and attributes.
    /// </summary>
    /// <param name="value">The string value to write.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <param name="attributes">The attributes of the character list (default: 0).</param>
    public void WriteCharList(ReadOnlySpan<char> value, Encoding? encoding = null, byte attributes = 0)
    {
        encoding ??= TextEncoding;
        var bytesCount = encoding.GetByteCount(value);
        StartWriteList(KType.CharList, bytesCount, attributes);
        encoding.GetBytes(value, _writer);
        EndWriteList();
    }

    /// <summary>
    /// Writes a unary primitive value.
    /// </summary>
    /// <param name="value">The unary primitive value to write.</param>
    public void WriteUnaryPrimitive(UnaryPrimitive value)
    {
        var needPop = BeginWriteType(KType.UnaryPrimitive);
        _writer.WriteByte((byte)value);
        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a unit value.
    /// </summary>
    public void WriteUnit()
    {
        WriteUnaryPrimitive(UnaryPrimitive.Unit);
    }

    /// <summary>
    /// Writes a lambda function.
    /// </summary>
    /// <param name="lambda">The lambda function to write.</param>
    public void WriteLambda(KLambda lambda)
    {
        var needPop = BeginWriteType(KType.Lambda);

        // Write context (empty symbol for now, following qSharp pattern)
        _writer.WriteNullTerminatedString(lambda.Context ?? "", TextEncoding);

        // Write expression as character array
        var expressionChars = lambda.Expression.ToCharArray();
        StartWriteList(KType.CharList, expressionChars.Length);
        foreach (var ch in expressionChars)
        {
            _writer.WriteByte((byte)ch);
        }

        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a projection function.
    /// </summary>
    /// <param name="projection">The projection function to write.</param>
    public void WriteProjection(KProjection projection)
    {
        var needPop = BeginWriteType(KType.Projection);

        // Write the number of parameters
        _writer.WriteInt32(projection.Count);

        // Write each parameter as an object
        foreach (var parameter in projection)
        {
            // TODO: This should be implemented by the higher-level serialization system
            throw new NotImplementedException("WriteObject should be implemented by the serialization system");
        }

        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a composition function.
    /// </summary>
    /// <param name="composition">The composition function to write.</param>
    public void WriteComposition(KComposition composition)
    {
        var needPop = BeginWriteType(KType.Composition);

        // Write the number of functions
        _writer.WriteInt32(composition.Count);

        // Write each function as an object
        foreach (var function in composition)
        {
            // TODO: This should be implemented by the higher-level serialization system
            throw new NotImplementedException("WriteObject should be implemented by the serialization system");
        }

        if (needPop)
            EndWriteType();
    }

    /// <summary>
    /// Writes a generic function.
    /// </summary>
    /// <param name="function">The function to write.</param>
    public void WriteFunction(KFunction function)
    {
        switch (function)
        {
            case KLambda lambda:
                WriteLambda(lambda);
                break;
            case KProjection projection:
                WriteProjection(projection);
                break;
            case KComposition composition:
                WriteComposition(composition);
                break;
            default:
                // Generic function - just write the type code and a placeholder byte
                var needPop = BeginWriteType(function.FunctionType);
                _writer.WriteByte(0); // Placeholder byte for primitive functions
                if (needPop)
                    EndWriteType();
                break;
        }
    }

    #endregion
}
