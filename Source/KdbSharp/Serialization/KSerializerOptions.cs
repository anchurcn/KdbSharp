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
using System.Runtime.CompilerServices;
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

        RegisterDefaultConverter<DateTime>(KType.Timestamp, new KDateTimeConverter<DateTime>());
        RegisterDefaultConverter<DateTime>(KType.Month, new KMonthConverter<DateTime>());
        RegisterDefaultConverter<DateTime>(KType.Date, new KDateConverter<DateTime>());
        RegisterDefaultConverter<DateTime>(KType.DateTime, new KDateTimeConverter<DateTime>());
        RegisterDefaultConverter<TimeSpan>(KType.TimeSpan, new KTimeSpanConverter<TimeSpan>());
        RegisterDefaultConverter<TimeSpan>(KType.Minute, new KMinuteConverter<TimeSpan>());
        RegisterDefaultConverter<TimeSpan>(KType.Second, new KSecondConverter<TimeSpan>());
        RegisterDefaultConverter<TimeSpan>(KType.Time, new KTimeConverter<TimeSpan>());

        RegisterDefaultConverter<object[]>(KType.GeneralList, new KGenericListConverter());
        RegisterDefaultConverter<KSimpleDictionary>(KType.Dictionary, new KDictionaryConverter<object>());
        RegisterDefaultConverter<KTable>(KType.Table, new KTableConverter());
        RegisterDefaultConverter<KUnit>(KType.UnaryPrimitive, new KUnitConverter());
        RegisterDefaultConverter<KdbException>(KType.Error, new KdbExceptionConverter());

        RegisterDefaultConverter<bool[]>(KType.BooleanList, new KArrayConverter<bool>(KType.BooleanList, new KBooleanConverter()));
        RegisterDefaultConverter<Guid[]>(KType.GuidList, new KArrayConverter<Guid>(KType.GuidList, new KGuidConverter()));
        RegisterDefaultConverter<byte[]>(KType.ByteList, new KArrayConverter<byte>(KType.ByteList, new KByteConverter()));
        RegisterDefaultConverter<short[]>(KType.ShortList, new KArrayConverter<short>(KType.ShortList, new KShortConverter<short>()));
        RegisterDefaultConverter<int[]>(KType.IntList, new KArrayConverter<int>(KType.IntList, new KIntConverter<int>()));
        RegisterDefaultConverter<long[]>(KType.LongList, new KArrayConverter<long>(KType.LongList, new KLongConverter<long>()));
        RegisterDefaultConverter<float[]>(KType.RealList, new KArrayConverter<float>(KType.RealList, new KRealConverter<float>()));
        RegisterDefaultConverter<double[]>(KType.FloatList, new KArrayConverter<double>(KType.FloatList, new KFloatConverter<double>()));
        RegisterDefaultConverter<char[]>(KType.CharList, new CharArrayConverter());
        RegisterDefaultConverter<string[]>(KType.SymbolList, new KArrayConverter<string>(KType.SymbolList, new KSymbolConverter()));
        RegisterDefaultConverter<DateTime[]>(KType.TimestampList, new KArrayConverter<DateTime>(KType.TimestampList, new KDateTimeConverter<DateTime>()));
        RegisterDefaultConverter<DateTime[]>(KType.MonthList, new KArrayConverter<DateTime>(KType.MonthList, new KMonthConverter<DateTime>()));
        RegisterDefaultConverter<DateTime[]>(KType.DateList, new KArrayConverter<DateTime>(KType.DateList, new KDateConverter<DateTime>()));
        RegisterDefaultConverter<DateTime[]>(KType.DateTimeList, new KArrayConverter<DateTime>(KType.DateTimeList, new KDateTimeConverter<DateTime>()));
        RegisterDefaultConverter<TimeSpan[]>(KType.TimeSpanList, new KArrayConverter<TimeSpan>(KType.TimeSpanList, new KTimeSpanConverter<TimeSpan>()));
        RegisterDefaultConverter<TimeSpan[]>(KType.MinuteList, new KArrayConverter<TimeSpan>(KType.MinuteList, new KMinuteConverter<TimeSpan>()));
        RegisterDefaultConverter<TimeSpan[]>(KType.SecondList, new KArrayConverter<TimeSpan>(KType.SecondList, new KSecondConverter<TimeSpan>()));
        RegisterDefaultConverter<TimeSpan[]>(KType.TimeList, new KArrayConverter<TimeSpan>(KType.TimeList, new KTimeConverter<TimeSpan>()));
        RegisterDefaultWriteConverter(t => typeof(ITuple).IsAssignableFrom(t), new ValueTupleConverterFactory());

    }
    public void RegisterBuildInConverters()
    {
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
        RegisterConverter(new KDictionaryConverter<object>());
        RegisterConverter(new KTableConverter());
        RegisterConverter(new KUnitConverter());
        RegisterConverter(new KdbExceptionConverter());
        RegisterConverter(new CharArrayConverter()); // Conversion between .NET char array and KDB char list is not element by element.
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
        ObjectTypeInfo = new ObjectTypeInfo(this);
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
                if (converter is KTypeConverterFactory factory)
                {
                    return factory.GetConverter(type, null, this);
                }
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
                if (converter is KTypeConverterFactory factory)
                {
                    return factory.GetConverter(null!, type, this);
                }

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
