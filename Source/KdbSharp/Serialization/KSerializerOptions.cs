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
using KdbSharp.Serialization.Converters;
using KdbSharp.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KdbSharp.Serialization;


public enum NullHandleStrategy
{
    Default,
    OnlyUnitAsNull,
}
public partial class KSerializerOptions
{
    public void RegisterBuildInDefaultConverters()
    {

        RegisterDefaultConverter<bool>(KType.Boolean, new KBooleanConverter());
        RegisterDefaultConverter<Guid>(KType.Guid, new KGuidConverter());
        RegisterDefaultConverter<byte>(KType.Byte, new KByteConverter());
        RegisterDefaultConverter<short>(KType.Short, new KShortConverter<short>());
        RegisterDefaultConverter<int>(KType.Int, new KIntConverter<int>());
        RegisterDefaultConverter<long>(KType.Long, new KLongConverter<long>());
        RegisterDefaultConverter<float>(KType.Real, new KRealConverter<float>());
        RegisterDefaultConverter<double>(KType.Float, new KFloatConverter<double>());
        RegisterDefaultConverter<char>(KType.Char, new KCharConverter<char>());
        RegisterDefaultConverter<string>(KType.Symbol, new KSymbolConverter());

        // DateTime=Timestamp
        RegisterDefaultConverter<DateTime>(KType.Timestamp, new KDateTimeConverter<DateTime>());
        // Month
        RegisterDefaultConverter<DateTime>(KType.Month, new KMonthConverter<DateTime>());
        // Date
        RegisterDefaultConverter<DateTime>(KType.Date, new KDateConverter<DateTime>());
        // DateTime
        RegisterDefaultConverter<DateTime>(KType.DateTime, new KDateTimeConverter<DateTime>());
        // Timespan
        RegisterDefaultConverter<TimeSpan>(KType.TimeSpan, new KTimeSpanConverter<TimeSpan>());
        // Minute
        RegisterDefaultConverter<TimeSpan>(KType.Minute, new KMinuteConverter<TimeSpan>());
        // Second
        RegisterDefaultConverter<TimeSpan>(KType.Second, new KSecondConverter<TimeSpan>());
        // Time
        RegisterDefaultConverter<TimeSpan>(KType.Time, new KTimeConverter<TimeSpan>());

        // Complex
        RegisterDefaultConverter<object[]>(KType.GeneralList, new KGenericListConverter());
        // Dictionary
        RegisterDefaultConverter<KSimpleDictionary>(KType.Dictionary, new KDictionaryConverter<KSimpleDictionary>());
        RegisterDefaultConverter<KTable>(KType.Table, new KTableConverter());
        // Unit
        RegisterDefaultConverter<KUnit>(KType.Unit, new KUnitConverter());
        // Error
        RegisterDefaultConverter<KdbException>(KType.Error, new KdbExceptionConverter());

        // Atom List
        RegisterDefaultConverter<bool[]>(KType.BooleanList, new KArrayConverter<bool>(KType.BooleanList, new KBooleanConverter()));
        // Guid
        RegisterDefaultConverter<Guid[]>(KType.GuidList, new KArrayConverter<Guid>(KType.GuidList, new KGuidConverter()));
        // Byte
        RegisterDefaultConverter<byte[]>(KType.ByteList, new KArrayConverter<byte>(KType.ByteList, new KByteConverter()));
        // Short
        RegisterDefaultConverter<short[]>(KType.ShortList, new KArrayConverter<short>(KType.ShortList, new KShortConverter<short>()));
        // Int
        RegisterDefaultConverter<int[]>(KType.IntList, new KArrayConverter<int>(KType.IntList, new KIntConverter<int>()));
        // Long
        RegisterDefaultConverter<long[]>(KType.LongList, new KArrayConverter<long>(KType.LongList, new KLongConverter<long>()));
        // Real
        RegisterDefaultConverter<float[]>(KType.RealList, new KArrayConverter<float>(KType.RealList, new KRealConverter<float>()));
        // Float
        RegisterDefaultConverter<double[]>(KType.FloatList, new KArrayConverter<double>(KType.FloatList, new KFloatConverter<double>()));
        // Char
        RegisterDefaultConverter<char[]>(KType.CharList, new KArrayConverter<char>(KType.CharList, new KCharConverter<char>()));
        // Symbol
        RegisterDefaultConverter<string[]>(KType.SymbolList, new KArrayConverter<string>(KType.SymbolList, new KSymbolConverter()));
        // Timestamp
        RegisterDefaultConverter<DateTime[]>(KType.TimestampList, new KArrayConverter<DateTime>(KType.TimestampList, new KDateTimeConverter<DateTime>()));
        // Month
        RegisterDefaultConverter<DateTime[]>(KType.MonthList, new KArrayConverter<DateTime>(KType.MonthList, new KMonthConverter<DateTime>()));
        // Date
        RegisterDefaultConverter<DateTime[]>(KType.DateList, new KArrayConverter<DateTime>(KType.DateList, new KDateConverter<DateTime>()));
        // DateTime
        RegisterDefaultConverter<DateTime[]>(KType.DateTimeList, new KArrayConverter<DateTime>(KType.DateTimeList, new KDateTimeConverter<DateTime>()));
        // TimeSpan
        RegisterDefaultConverter<TimeSpan[]>(KType.TimeSpanList, new KArrayConverter<TimeSpan>(KType.TimeSpanList, new KTimeSpanConverter<TimeSpan>()));
        // Minute
        RegisterDefaultConverter<TimeSpan[]>(KType.MinuteList, new KArrayConverter<TimeSpan>(KType.MinuteList, new KMinuteConverter<TimeSpan>()));
        // Second
        RegisterDefaultConverter<TimeSpan[]>(KType.SecondList, new KArrayConverter<TimeSpan>(KType.SecondList, new KSecondConverter<TimeSpan>()));
        // Time
        RegisterDefaultConverter<TimeSpan[]>(KType.TimeList, new KArrayConverter<TimeSpan>(KType.TimeList, new KTimeConverter<TimeSpan>()));

    }
    public void RegisterBuildInConverters()
    {
        // 18 atom converters
        RegisterConverter(new KBooleanConverter());
        RegisterConverter(new KGuidConverter());
        RegisterConverter(new KByteConverter());
        RegisterConverter(new KShortConverterFactory());
        RegisterConverter(new KIntConverterFactory());
        RegisterConverter(new KLongConverterFactory());
        RegisterConverter(new KRealConverterFactory());
        RegisterConverter(new KFloatConverterFactory());
        RegisterConverter(new KCharConverterFactory());
        RegisterConverter(new KSymbolConverter());
        RegisterConverter(new KTimestampeConverterFactory());
        RegisterConverter(new KMonthConverterFactory());
        RegisterConverter(new KDateConverterFactory());
        RegisterConverter(new KDateTimeConverterFactory());
        RegisterConverter(new KTimeSpanConverterFactory());
        RegisterConverter(new KMinuteConverterFactory());
        RegisterConverter(new KSecondConverterFactory());
        RegisterConverter(new KTimeConverterFactory());

        RegisterConverter(new KGenericListConverter());
        RegisterConverter(new KDictionaryConverter<KSimpleDictionary>());
        RegisterConverter(new KDictionaryConverter<KKeyedTable>());
        RegisterConverter(new KTableConverter());
        RegisterConverter(new KUnitConverter());
        RegisterConverter(new KdbExceptionConverter());
        RegisterConverter(new KAtomListConverterFactory());

        RegisterConverter(new StringConverter());
        RegisterConverter(new ValueTupleConverterFactory());
    }
}
/// <summary>
/// Options for KdbSerializer.
/// </summary>
public partial class KSerializerOptions
{
    //public bool IsLittleEndian { get; set; } = true;
    /// <summary>
    /// Used to read CharList, Symbol as string.
    /// </summary>
    public Encoding TextEncoding { get; set; } = Encoding.UTF8;
    public KTypeInfo ObjectTypeInfo { get; internal set; }
    IKTypeInfoResolver TypeInfoResolver = new DefaultKTypeInfoResolver();

    public KSerializerOptions()
    {
        ObjectTypeInfo = new KTypeInfo<object>(this);
        RegisterBuildInDefaultConverters();
        RegisterBuildInConverters();
    }

    public KTypeInfo GetTypeInfo(Type inputType)
    {
        Debug.Assert(inputType != null);
        return inputType == typeof(object)
            ? ObjectTypeInfo
            : GetTypeInfoForRootType(inputType);
    }

    private Dictionary<Type, KTypeInfo> _typeInfo = new();

    internal KTypeInfo GetTypeInfoForRootType(Type type)
    {
        if (_typeInfo.TryGetValue(type, out var typeInfo))
        {
            return typeInfo;
        }
        else
        {
            var newTypeInfo = GetTypeInfoNoCaching(type);
            _typeInfo.Add(type, newTypeInfo);
            return newTypeInfo;
        }
    }
    internal KTypeInfo GetTypeInfoNoCaching(Type type)
    {
        return TypeInfoResolver.GetKTypeInfo(type, this)
            ?? throw new NotSupportedException($"Type {type} is not supported.");
    }

    // TODO: get convter by type info
    #region Get converter by type info

    public KTypeConverter? GetConverterByTypeInfo(Type type)
    {
        GetTypeInfo(type).TryGetConverter(out var converter);
        return converter;
    }
    public KTypeConverter<T>? GetConverterByTypeInfo<T>()
    {
        GetTypeInfo(typeof(T)).TryGetConverter<T>(out var converter);
        return converter;
    }
    public KTypeConverter<T>? GetConverterByTypeInfo<T>(KType t)
    {
        GetTypeInfo(typeof(T)).TryGetConverter<T>(t, out var converter);
        return converter;
    }
    #endregion

    #region Converter management

    private List<KTypeConverter> Converters { get; } = new();
    private List<(Func<Type, bool> canConvert, KTypeConverter converter)> DefaultWriteConverters { get; } = new();
    private List<(Func<KType, bool> canConvert, KTypeConverter converter)> DefaultReadConverters { get; } = new();
    public NullHandleStrategy NullHandleStategy { get; internal set; }

    public void RegisterDefaultWriteConverter(Func<Type, bool> canConvert, KTypeConverter converter)
    {
        DefaultWriteConverters.Add((canConvert, converter));
    }
    public void RegisterDefaultReadConverter(Func<KType, bool> canConvert, KTypeConverter converter)
    {
        DefaultReadConverters.Add((canConvert, converter));
    }
    public void RegisterDefaultConverter<T>(KType kt, KTypeConverter converter)
    {
        RegisterDefaultReadConverter(x => x == kt, converter);
        RegisterDefaultWriteConverter(x => x == typeof(T), converter);
    }
    public void RegisterConverter(KTypeConverter converter)
    {
        Converters.Add(converter);
    }
    public KTypeConverter? GetConverter(Type type)
    {
        foreach (var (canConvert, converter) in DefaultWriteConverters)
        {
            if (canConvert(type))
            {
                return converter;
            }
        }
        return null;
    }
    public KTypeConverter? GetConverter(KType type)
    {
        foreach (var (canConvert, converter) in DefaultReadConverters)
        {
            if (canConvert(type))
            {
                return converter;
            }
        }
        return null;
    }
    public KTypeConverter? GetConverter(Type type, KType kType)
    {
        foreach (var converter in Converters)
        {
            if (converter.CanConvert(type, kType))
            {
                if (converter is KTypeConverterFactory factory)
                {
                    return factory.GetConverter(type, kType, this);
                }
                return converter;
            }
        }
        return null;
    }
    #endregion

    // Impl default converters, Register and Get by canConvert Func


    // Write
    // When using KTypeInfo<object>, 意味着要使用 value.GetType() 类型的默认 Converter
    // 如果是基础类型，直接使用该基础类注册的默认 Converter；（你需要为所有的基础类型注册默认 Converter，不然写入 int 时系统不知道写 KLong 还是 KInt）
    // 如果是 实体类，则是生成一个 该类型的 ObjectConverter，由 RegisterDefaultConverterFor 返回，不用预先注册。
    // 更新：应该实现个拓展性更强的注册默认 Converter 的方法，以支持 Dictionary<,> 实体类的序列化，不可能为每个这种开放的类型都注册一个 Converter
    // 考虑 Factory 模式，根据类型生成 Converter
}
