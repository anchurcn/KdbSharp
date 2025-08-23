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
using System.Collections.Generic;

namespace KdbSharp.Serialization.Converters;

public class KDictionaryConverter<T> : KTypeConverter<T> // where T: KSimpleDictionary or KKeyedTable
{
    public override KType? TypeToWriteTo => KType.Dictionary;

    public override bool CanConvert(Type t, KType kt)
    {
        return t == typeof(T) && kt == KType.Dictionary;
    }

    public override T Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        reader.StartReadDictionary();
        var keys = KSerializer.Deserialize<object>(ref reader, options);
        var values = KSerializer.Deserialize<object>(ref reader, options);
        reader.EndReadDictionary();
        if (keys is Array keyArray && values is Array valueArray)
        {
            return (T)(object)new KSimpleDictionary(keyArray, valueArray);
        }
        else if (keys is KTable keyTable && values is KTable valueTable2)
        {
            return (T)(object)new KKeyedTable(keyTable, valueTable2);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    public override void Write(ref KSerializationWriter writer, T value, KSerializerOptions options)
    {
        if (value is KSimpleDictionary simpleDict)
        {
            writer.StartWriteDictionary();

            // Determine the KType for keys and values based on the actual array types
            var keysKType = GetKTypeForArray(simpleDict.Keys);
            var valuesKType = GetKTypeForArray(simpleDict.Values);

            KSerializer.Serialize(ref writer, simpleDict.Keys, keysKType, options);
            KSerializer.Serialize(ref writer, simpleDict.Values, valuesKType, options);
            writer.EndWriteDictionary();
        }
        else if (value is KKeyedTable keyedTable)
        {
            writer.StartWriteDictionary();

            // For KKeyedTable, Keys and Values are KTable objects, not arrays
            KSerializer.Serialize(ref writer, keyedTable.Keys, KType.Table, options);
            KSerializer.Serialize(ref writer, keyedTable.Values, KType.Table, options);
            writer.EndWriteDictionary();
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    private static KType GetKTypeForArray(Array array)
    {
        var elementType = array.GetType().GetElementType()!;

        // Map CLR types to KDB array types
        // For basic CLR arrays, we use GeneralList to let the serializer handle the conversion
        return elementType switch
        {
            Type t when t == typeof(bool) => KType.BooleanList,
            Type t when t == typeof(byte) => KType.ByteList,
            Type t when t == typeof(short) => KType.ShortList,
            Type t when t == typeof(int) => KType.IntList,
            Type t when t == typeof(long) => KType.LongList,
            Type t when t == typeof(float) => KType.RealList,
            Type t when t == typeof(double) => KType.FloatList,
            Type t when t == typeof(char) => KType.CharList,
            Type t when t == typeof(string) => KType.SymbolList, // Use SymbolList for string arrays
            Type t when t == typeof(DateTime) => KType.DateTimeList,
            Type t when t == typeof(TimeSpan) => KType.TimeSpanList,
            Type t when t == typeof(Guid) => KType.GuidList,
            _ => KType.GeneralList // Fallback to general list for complex types
        };
    }
}

/// <summary>
/// Converter factory for generic Dictionary&lt;TKey, TValue&gt; types
/// </summary>
public class KGenericDictionaryConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        if (kt != KType.Dictionary) return false;

        return t.IsGenericType &&
               t.GetGenericTypeDefinition() == typeof(Dictionary<,>);
    }

    public override KTypeConverter GetConverter(Type t, KType? kt, KSerializerOptions options)
    {
        var genericArgs = t.GetGenericArguments();
        var keyType = genericArgs[0];
        var valueType = genericArgs[1];

        var converterType = typeof(KGenericDictionaryConverter<,>).MakeGenericType(keyType, valueType);
        return (KTypeConverter)(Activator.CreateInstance(converterType)
                               ?? throw new InvalidOperationException($"Failed to create converter for {t}"));
    }
}

/// <summary>
/// Converter for generic Dictionary&lt;TKey, TValue&gt; to/from KDictionary
/// </summary>
public class KGenericDictionaryConverter<TKey, TValue> : KTypeConverter<Dictionary<TKey, TValue>>
    where TKey : notnull
{
    public override KType? TypeToWriteTo => KType.Dictionary;

    public override bool CanConvert(Type t, KType kt)
    {
        return t == typeof(Dictionary<TKey, TValue>) && kt == KType.Dictionary;
    }

    public override Dictionary<TKey, TValue> Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        reader.StartReadDictionary();
        var keys = KSerializer.Deserialize<object>(ref reader, options);
        var values = KSerializer.Deserialize<object>(ref reader, options);
        reader.EndReadDictionary();

        if (keys is not Array keyArray || values is not Array valueArray)
        {
            throw new InvalidOperationException("Expected array keys and values for Dictionary conversion");
        }

        if (keyArray.Length != valueArray.Length)
        {
            throw new InvalidOperationException("Key and value arrays must have the same length");
        }

        var dictionary = new Dictionary<TKey, TValue>(keyArray.Length);

        for (int i = 0; i < keyArray.Length; i++)
        {
            var key = ConvertKey(keyArray.GetValue(i));
            var value = ConvertValue(valueArray.GetValue(i));
            dictionary[key] = value;
        }

        return dictionary;
    }

    public override void Write(ref KSerializationWriter writer, Dictionary<TKey, TValue> value, KSerializerOptions options)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var keyArray = CreateKeyArray(value);
        var valueArray = CreateValueArray(value);

        writer.StartWriteDictionary();

        // Determine the appropriate KType for the arrays
        var keyKType = GetKTypeForArray(typeof(TKey));
        var valueKType = GetKTypeForArray(typeof(TValue));

        KSerializer.Serialize(ref writer, keyArray, keyKType, options);
        KSerializer.Serialize(ref writer, valueArray, valueKType, options);
        writer.EndWriteDictionary();
    }

    private TKey ConvertKey(object? keyObj)
    {
        if (keyObj == null)
        {
            throw new InvalidOperationException("Dictionary keys cannot be null");
        }

        if (keyObj is TKey directKey)
        {
            return directKey;
        }

        // Handle type conversion for common cases
        if (typeof(TKey) == typeof(string) && keyObj is not string)
        {
            return (TKey)(object)keyObj.ToString()!;
        }

        try
        {
            return (TKey)Convert.ChangeType(keyObj, typeof(TKey));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Cannot convert key of type {keyObj.GetType()} to {typeof(TKey)}", ex);
        }
    }

    private TValue ConvertValue(object? valueObj)
    {
        if (valueObj == null)
        {
            if (default(TValue) != null)
            {
                throw new InvalidOperationException($"Cannot convert null to non-nullable type {typeof(TValue)}");
            }
            return default(TValue)!;
        }

        if (valueObj is TValue directValue)
        {
            return directValue;
        }

        // Handle type conversion for common cases
        if (typeof(TValue) == typeof(string) && valueObj is not string)
        {
            return (TValue)(object)valueObj.ToString()!;
        }

        try
        {
            return (TValue)Convert.ChangeType(valueObj, typeof(TValue));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Cannot convert value of type {valueObj.GetType()} to {typeof(TValue)}", ex);
        }
    }

    private TKey[] CreateKeyArray(Dictionary<TKey, TValue> dictionary)
    {
        return dictionary.Keys.ToArray();
    }

    private TValue[] CreateValueArray(Dictionary<TKey, TValue> dictionary)
    {
        return dictionary.Values.ToArray();
    }

    private static KType GetKTypeForArray(Type elementType)
    {
        // Map CLR types to KDB array types
        return elementType switch
        {
            Type t when t == typeof(bool) => KType.BooleanList,
            Type t when t == typeof(byte) => KType.ByteList,
            Type t when t == typeof(short) => KType.ShortList,
            Type t when t == typeof(int) => KType.IntList,
            Type t when t == typeof(long) => KType.LongList,
            Type t when t == typeof(float) => KType.RealList,
            Type t when t == typeof(double) => KType.FloatList,
            Type t when t == typeof(char) => KType.CharList,
            Type t when t == typeof(string) => KType.SymbolList,
            Type t when t == typeof(DateTime) => KType.DateTimeList,
            Type t when t == typeof(TimeSpan) => KType.TimeSpanList,
            Type t when t == typeof(Guid) => KType.GuidList,
            _ => KType.GeneralList // Fallback to general list for complex types
        };
    }
}
