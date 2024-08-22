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
using KdbSharp.Types;
using System.Text;

namespace KdbSharp.Serialization;

public class KWriterOptions
{
    public Encoding TextEncoding { get; set; } = Encoding.UTF8;
    public byte ProtocolVersion { get; set; } = KConstant.ClientProtocolVersion;
}

public class KWriter
{
    internal readonly KWriteBuffer _buffer;
    public struct WriteStackFrame
    {
        public KType TypeStamp;
        public byte? Attributes;
        public int? ListLength;
        public KType? AtomTypeStamp;
    }

    private readonly Stack<WriteStackFrame> _stack = new Stack<WriteStackFrame>();
    private WriteStackFrame CurrentFrame => _stack.Count > 0 ? _stack.Peek() :
        throw new InvalidOperationException("No type is being read.");
    public KType TypeStamp => CurrentFrame.TypeStamp;
    public byte? Attributes => CurrentFrame.Attributes;
    public int? ListLength => CurrentFrame.ListLength;
    public KType? AtomTypeStamp => CurrentFrame.AtomTypeStamp;
    public bool IsList => TypeStamp.IsList();
    public bool IsAtom => TypeStamp.IsAtom();
    public bool IsAtomList => AtomTypeStamp is not null;

    public KWriteBuffer Buffer => _buffer;

    public KWriterOptions Options { get; }

    public KWriter(KWriteBuffer buffer, KWriterOptions? options = null)
    {
        options ??= new KWriterOptions();
        _buffer = buffer;
        Options = options;
        _protocolVersion = options.ProtocolVersion;
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
    private bool TryWriteTypeStamp(KType kType)
    {
        if (_stack.Count > 0 && IsAtomList)
        {
            // When writing an atom list, the only valid type is the its atom type and the type stamp should not be written.
            if (AtomTypeStamp != kType)
            {
                throw new InvalidOperationException($"Invalid write type {kType} in atom list {TypeStamp}. expected {AtomTypeStamp}.");
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

    private readonly byte _protocolVersion;
    public void WriteTypeStamp(KType kType)
    {
        // Protocol validation
        ThrowIfUsingKTypePreSupportedProtocolVersion(KType.Timestamp, 1);
        ThrowIfUsingKTypePreSupportedProtocolVersion(KType.TimeSpan, 1);
        ThrowIfUsingKTypePreSupportedProtocolVersion(KType.Guid, 3);

        void ThrowIfUsingKTypePreSupportedProtocolVersion(KType target, byte minimalSupportedProtocolVersion)
        {
            var beingChecked = kType.IsAtomList() ? kType.GetUnderlyingType() : kType;
            if (beingChecked == target && _protocolVersion < minimalSupportedProtocolVersion)
            {
                throw new NotSupportedException($"KType {target} is not supported in protocol version {_protocolVersion}, minimal supported protocol version is {minimalSupportedProtocolVersion}.");
            }
        }
        _buffer.WriteByte((byte)kType);
    }


    public void WriteBoolean(bool value)
    {
        BeginWriteType(KType.Boolean);
        _buffer.WriteBool(value);
        EndWriteType();
    }

    public void WriteGuid(Guid value)
    {
        BeginWriteType(KType.Guid);
        var span = value.AsSpan();
        if (BitConverter.IsLittleEndian)
        {
            SerializationHelper.FlipGuidTop3Parts(ref value);
            _buffer.WriteBytes(span);
        }
        else
        {
            _buffer.WriteBytes(span);
        }
        EndWriteType();
    }

    public void WriteByte(byte value)
    {
        BeginWriteType(KType.Byte);
        _buffer.WriteByte(value);
        EndWriteType();
    }

    public void WriteShort(KShort value)
    {
        BeginWriteType(KType.Short);
        _buffer.WriteInt16(value.Value);
        EndWriteType();
    }

    public void WriteInt(KInt value)
    {
        BeginWriteType(KType.Int);
        _buffer.WriteInt32(value.Value);
        EndWriteType();
    }

    public void WriteLong(KLong value)
    {
        BeginWriteType(KType.Long);
        _buffer.WriteInt64(value.Value);
        EndWriteType();
    }

    public void WriteReal(KReal value)
    {
        BeginWriteType(KType.Real);
        _buffer.WriteSingle(value.Value);
        EndWriteType();
    }

    public void WriteFloat(KFloat value)
    {
        BeginWriteType(KType.Float);
        _buffer.WriteDouble(value.Value);
        EndWriteType();
    }

    public void WriteChar(KChar value)
    {
        BeginWriteType(KType.Char);
        _buffer.WriteByte((byte)value.Value);
        EndWriteType();
    }

    public void WriteSymbol(ReadOnlySpan<char> value, Encoding? encoding = null)
    {
        encoding ??= Options.TextEncoding;
        BeginWriteType(KType.Symbol);
        _buffer.WriteNullTerminatedString(value, encoding);
        EndWriteType();
    }

    public void WriteTimestamp(KTimestamp value)
    {
        BeginWriteType(KType.Timestamp);
        _buffer.WriteInt64(value.Value);
        EndWriteType();
    }

    public void WriteMonth(KMonth value)
    {
        BeginWriteType(KType.Month);
        _buffer.WriteInt32(value.Value);
        EndWriteType();
    }

    public void WriteDate(KDate value)
    {
        BeginWriteType(KType.Date);
        _buffer.WriteInt32(value.Value);
        EndWriteType();
    }

    public void WriteDateTime(KDateTime value)
    {
        BeginWriteType(KType.DateTime);
        _buffer.WriteDouble(value.Value);
        EndWriteType();
    }

    public void WriteTimeSpan(KTimeSpan value)
    {
        BeginWriteType(KType.TimeSpan);
        _buffer.WriteInt64(value.Value);
        EndWriteType();
    }

    public void WriteMinute(KMinute value)
    {
        BeginWriteType(KType.Minute);
        _buffer.WriteInt32(value.Value);
        EndWriteType();
    }

    public void WriteSecond(KSecond value)
    {
        BeginWriteType(KType.Second);
        _buffer.WriteInt32(value.Value);
        EndWriteType();
    }

    public void WriteTime(KTime value)
    {
        BeginWriteType(KType.Time);
        _buffer.WriteInt32(value.Value);
        EndWriteType();
    }

    public void WriteStartList(KType listType, int length, byte attribute = 0)
    {
        BeginWriteType(listType);
        _buffer.WriteByte(attribute);
        _buffer.WriteInt32(length);
        // Refill the stack frame with the array type and length.
        var frame = CurrentFrame;
        frame.Attributes = attribute;
        frame.ListLength = length;
        if (listType.IsAtomList())
            frame.AtomTypeStamp = listType.Neg();
        _stack.Pop();
        _stack.Push(frame);
    }

    public void WriteEndList()
    {
        EndWriteType();
    }

    public void WriteStartDictionary()
    {
        BeginWriteType(KType.Dictionary);
    }

    public void WriteEndDictionary()
    {
        EndWriteType();
    }

    public void WriteStartTable(byte attributes = 0)
    {
        BeginWriteType(KType.Table);
        _buffer.WriteByte(attributes);
        WriteTypeStamp(KType.Dictionary);
        // Refill the stack frame with the array type and length.
        var frame = CurrentFrame;
        frame.Attributes = attributes;
        _stack.Pop();
        _stack.Push(frame);
    }

    public void WriteEndTable()
    {
        EndWriteType();
    }

    public void WriteCharList(string value, Encoding? encoding = null, byte attributes = 0)
    {
        encoding ??= Options.TextEncoding;
        var bytes = encoding.GetBytes(value);
        WriteStartList(KType.CharList, bytes.Length, attributes);
        _buffer.WriteBytes(bytes);
        WriteEndList();
    }

    internal void WriteUnit()
    {
        BeginWriteType(KType.Unit);
        _buffer.WriteByte(0);
        EndWriteType();
    }
}

