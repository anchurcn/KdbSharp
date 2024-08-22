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

namespace KdbSharp.Serialization.Converters;

public class KDictionaryConverter<T> : KTypeConverter<T> // where T: KSimpleDictionary or KKeyedTable
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == typeof(T) && kt == KType.Dictionary;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        if (reader.TypeStamp != KType.Dictionary)
        {
            throw new InvalidOperationException();
        }
        var keys = KSerializer.Deserialize<object>(reader, options);
        var values = KSerializer.Deserialize<object>(reader, options);
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

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (value is KSimpleDictionary simpleDict)
        {
            writer.WriteStartDictionary();
            KSerializer.Serialize(writer, simpleDict.Keys, options);
            KSerializer.Serialize(writer, simpleDict.Values, options);
            writer.WriteEndDictionary();
        }
        else if (value is KKeyedTable keyedTable)
        {
            writer.WriteStartDictionary();
            KSerializer.Serialize(writer, keyedTable.Keys, options);
            KSerializer.Serialize(writer, keyedTable.Values, options);
            writer.WriteEndDictionary();
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}
