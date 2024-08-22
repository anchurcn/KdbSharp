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
namespace KdbSharp.Types;

public enum KType: sbyte
{
    GeneralList = 0,
    Boolean = -1,
    BooleanList = 1,
    Guid = -2,
    GuidList = 2,
    Byte = -4,
    ByteList = 4,
    Short = -5,
    ShortList = 5,
    Int = -6,
    IntList = 6,
    Long = -7,
    LongList = 7,
    Real = -8,
    RealList = 8,
    Float = -9,
    FloatList = 9,
    Char = -10,
    CharList = 10,
    Symbol = -11,
    SymbolList = 11,
    Timestamp = -12,
    TimestampList = 12,
    Month = -13,
    MonthList = 13,
    Date = -14,
    DateList = 14,
    DateTime = -15,
    DateTimeList = 15,
    TimeSpan = -16,
    TimeSpanList = 16,
    Minute = -17,
    MinuteList = 17,
    Second = -18,
    SecondList = 18,
    Time = -19,
    TimeList = 19,
    AnyMap = 77,
    NestedSymEnum = 97,
    Table = 98,
    Dictionary = 99,
    Lambda = 100,
    Unit = 101,
    Operator = 102,
    Iterator = 103,
    Projection = 104,
    Composition = 105,
    Iterator106 = 106,
    Iterator107 = 107,
    Iterator108 = 108,
    Iterator109 = 109,
    Iterator110 = 110,
    Iterator111 = 111,
    DynamicLoad = 112,
    Error = -128,
}

public sealed class KUnit
{
    private KUnit()
    {
    }
    public static readonly KUnit Value = new KUnit();
}
public interface INullable
{
    bool IsNull { get; }
}
public interface IIndexable
{
    public object this[int index] { get; }
}


internal static class TypeNullString
{
    public const string Long = "0Nj";
    public const string Short = "0Nh";
    public const string Int = "0Ni";
}

public static class KTypeHelper
{
    public static bool IsList(this KType type)
    {
        return type == KType.GeneralList || IsAtomList(type);
    }

    public static KType Neg(this KType type)
    {
        return (KType)(-(int)type);
    }

    public static bool IsAtom(this KType type)
    {
        return type < KType.GeneralList && type >= KType.Time && type != (KType)((sbyte)-3);
    }

    public static bool IsAtomList(this KType kt) => kt > 0 && Neg(kt).IsAtom();

    public static KType GetUnderlyingType(this KType type)
    {
        if (!type.IsAtomList())
        {
            throw new InvalidOperationException("This method is only valid for atom list types.");
        }
        return type.Neg();
    }
}
