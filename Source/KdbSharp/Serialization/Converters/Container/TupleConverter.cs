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
using System.Runtime.CompilerServices;

namespace KdbSharp.Serialization.Converters;

// TODO: Add more tuple types.
public class ValueTupleConverter<T> : KTypeConverter<T> where T : ITuple
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.GeneralList;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        var len = reader.ListLength ?? throw new MessageSerializationException("Array length is null.");
        var res = (ITuple)default(T)!;
        var fields = typeof(T).GetFields();
        for (int i = 0; i < len; i++)
        {
            var field = fields[i];
            var elemTypeInfo = options.GetTypeInfo(field.FieldType);
            var elem = elemTypeInfo.DeserializeAsObject(reader)!;
            fields[i].SetValue(res, elem);
        }
        return (T)res;
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        writer.WriteStartList(KType.GeneralList, value.Length);
        var fields = typeof(T).GetFields();
        for (int i = 0; i < value.Length; i++)
        {
            var field = fields[i];
            var elem = value[i];
            var elemTypeInfo = options.GetTypeInfo(field.FieldType);
            elemTypeInfo.SerializeAsObject(writer, elem);
        }
        writer.WriteEndList();
    }
}

// ValueTupleConverterFactory
public class ValueTupleConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t.Name.StartsWith("ValueTuple`") && kt == KType.GeneralList;
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        var type = typeof(ValueTupleConverter<>).MakeGenericType(t);
        return (KTypeConverter)Activator.CreateInstance(type)!;
    }
}

//public class TupleConverter<T> : KTypeConverter<T> where T : ITuple
//{
//    public override bool CanConvert(Type t, KType kt)
//    {
//        return t == TypeToConvert && kt == KType.GeneralList;
//    }

//    public override T Read(KReader reader, KSerializerOptions options)
//    {
//        var arr = KSerializer.Deserialize<object[]>(reader, options);
//        var tuple = (T)Activator.CreateInstance(typeof(T), arr)!;
//        return tuple;
//    }

//    public override void Write(KWriter writer, T value, KSerializerOptions options)
//    {
//        var tuple = (ITuple)value;
//        var array = new object?[tuple.Length];
//        for (int i = 0; i < tuple.Length; i++)
//        {
//            array[i] = tuple[i];
//        }
//        KSerializer.Serialize(writer, array, options);
//    }
//}
