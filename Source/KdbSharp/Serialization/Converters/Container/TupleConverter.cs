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
    public override KType? TypeToWriteTo => KType.GeneralList;

    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToReadBack && kt == KType.GeneralList;
    }

    public override T Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        reader.StartReadList();
        var len = reader.ListLength ?? throw new KSerializationException("Array length is null.");
        var array = new object?[len];

        for (int i = 0; i < len; i++)
        {
            array[i] = KSerializer.Deserialize<object>(ref reader, options);
        }
        reader.EndReadList();

        // Create ValueTuple using reflection
        var tuple = (T)Activator.CreateInstance(typeof(T), array)!;
        return tuple;
    }

    public override void Write(ref KSerializationWriter writer, T value, KSerializerOptions options)
    {
        writer.StartWriteList(KType.GeneralList, value.Length);
        var fields = typeof(T).GetFields();
        for (int i = 0; i < value.Length; i++)
        {
            var item = value[i];
            KSerializer.Serialize(ref writer, item, fields[i].FieldType, options);
        }
        writer.EndWriteList();
    }
}

// ValueTupleConverterFactory
public class ValueTupleConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t.Name.StartsWith("ValueTuple`") && kt == KType.GeneralList;
    }

    public override KTypeConverter GetConverter(Type t, KType? kt, KSerializerOptions options)
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
