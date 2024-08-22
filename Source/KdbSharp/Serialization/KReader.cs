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
using System.Diagnostics;
using System.Text;

namespace KdbSharp.Serialization;

public class KReaderOptions
{
    public bool IsLittleEndian { get; set; }
    public Encoding TextEncoding { get; set; } = Encoding.UTF8;
}

public class KReader
{
    public struct ReadStackFrame
    {
        public KType TypeStamp;
        public byte? Attributes;
        public int? ListLength;
        public KType? AtomTypeStamp;
        public KType? LastNestedType;
    }

    private readonly KReadBuffer _buffer;
    internal KReadBuffer Buffer => _buffer;
    public KReaderOptions Options { get; }

    private readonly Stack<ReadStackFrame> _stack = new Stack<ReadStackFrame>();
    private ReadStackFrame CurrentFrame => _stack.Count > 0 ? _stack.Peek() :
        throw new InvalidOperationException("No type is being read.");
    public KType TypeStamp => CurrentFrame.TypeStamp;
    public byte? Attributes => CurrentFrame.Attributes;
    public int? ListLength => CurrentFrame.ListLength;
    /// <summary>
    /// Not null when reads a atom list and the value is neg TypeStamp.
    /// </summary>
    public KType? AtomTypeStamp => CurrentFrame.AtomTypeStamp;
    public bool IsList => TypeStamp.IsList();
    public bool IsAtom => TypeStamp.IsAtom();
    public bool IsAtomList => AtomTypeStamp is not null;

    public KType? LastNestedType => CurrentFrame.LastNestedType;

    // Encoding, Endianess
    // AtomConverter validation: var current = (AtomTypeStamp ?? TypeStamp) == KType.Int ? accept : throw new NotSupport(); 
    internal KReader(KReadBuffer buffer, KReaderOptions options)
    {
        _buffer = buffer;
        Options = options;
    }
    internal KTypeConverter<T> GetLastNestedTypeConverter<T>()
    {
        throw new NotImplementedException();
    }
    public KType BeginReadType()
    {
        var readFrame = new ReadStackFrame();
        readFrame.TypeStamp = _buffer.Read<KType>();
        if(readFrame.TypeStamp.IsList())
        {
            readFrame.Attributes = _buffer.ReadByte();
            readFrame.ListLength = _buffer.ReadInt32();
            if(readFrame.TypeStamp.IsAtomList())
            {
                readFrame.AtomTypeStamp = TypeStamp.Neg();
            }
        }
        else if(readFrame.TypeStamp == KType.Table)
        {
            readFrame.Attributes = _buffer.ReadByte();
            var dicT = _buffer.Read<KType>();
            Debug.Assert(dicT == KType.Dictionary);
        }
        _stack.Push(readFrame);
        return TypeStamp;
    }
    public void EndReadType()
    {
        if (_stack.Count == 0)
        {
            throw new InvalidOperationException("No type is being read.");
        }
        var last = _stack.Pop();
        if (_stack.Count > 0)
        {
            var frame = _stack.Pop();
            frame.LastNestedType = last.TypeStamp;
            _stack.Push(frame);
        }
    }

    #region Other read methods

    public string ReadSymbol(Encoding? encoding = null)
    {
        return _buffer.ReadNullTerminatedString(encoding ?? Options.TextEncoding);
    }

    public ReadOnlySpan<byte> ReadSymbolAsByteSpan()
    {
        return _buffer.GetNullTerminatedBytes();
    }
    public ReadOnlySpan<KChar> ReadSymbolAsKCharSpan()
    {
        return _buffer.GetNullTerminatedBytes().Cast<byte, KChar>();
    }

    #endregion

    #region Read atom

    public bool ReadBoolean()
    {
        return _buffer.ReadBool();
    }
    public Guid ReadGuid()
    {
        var result = Guid.Empty;
        if (BitConverter.IsLittleEndian)
        {
            result = _buffer.Read<Guid>();
            SerializationHelper.FlipGuidTop3Parts(ref result);
        }
        else
        {
            result = _buffer.Read<Guid>();
        }
        return result;
    }

    public byte ReadByte()
    {
        return _buffer.ReadByte();
    }
    
    public KShort ReadShort()
    {
        return new KShort(ReadInt16());
    }
    public KInt ReadInt()
    {
        return new KInt(ReadInt32());
    }
    public KLong ReadLong()
    {
        return new KLong(ReadInt64());
    }
    public KReal ReadReal()
    {
        return new KReal(ReadSingle());
    }
    public KFloat ReadFloat()
    {
        return new KFloat(ReadDouble());
    }
    public KChar ReadChar()
    {
        return new KChar((sbyte)_buffer.ReadByte());
    }

    public KTimestamp ReadTimestamp()
    {
        return new KTimestamp(ReadInt64());
    }
    public KMonth ReadMonth()
    {
        return new KMonth(ReadInt32());
    }
    public KDate ReadDate()
    {
        return new KDate(ReadInt32());
    }
    public KDateTime ReadDateTime()
    {
        return new KDateTime(ReadDouble());
    }
    public KTimeSpan ReadTimeSpan()
    {
        return new KTimeSpan(ReadInt64());
    }
    public KMinute ReadMinute()
    {
        return new KMinute(ReadInt32());
    }
    public KSecond ReadSecond()
    {
        return new KSecond(ReadInt32());
    }
    public KTime ReadTime()
    {
        return new KTime(ReadInt32());
    }
    #endregion

    #region Read integers methods

    private short ReadInt16()
    {
        return _buffer.ReadInt16();
    }
    private int ReadInt32()
    {
        return _buffer.ReadInt32();
    }
    private long ReadInt64()
    {
        return _buffer.ReadInt64();
    }
    #endregion

    #region Read floating point methods

    private float ReadSingle()
    {
        return _buffer.ReadSingle();
    }
    private double ReadDouble()
    {
        return _buffer.ReadDouble();
    }
    #endregion
}
