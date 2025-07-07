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

public class KAtomListConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        if (KTypeHelper.IsAtomList(kt))
        {
            if (t.IsArray)
            {
                var et = t.GetElementType();
                return true;
                // Maybe we should make a element CanConvert check here.
                // But API design is not support this.
                // Options.CanConvert(et, KTypeHelper.GetElementType(kt))
            }
            else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return false;
                // Not implemented
            }
            // Throw if t is Stack<T>, round trip for Stack<T> is not supported.
            else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Stack<>))
            {
                return false;
                throw new NotSupportedException("Round trip for Stack<T> is not supported.");
            }
        }
        return false;
    }

    public override KTypeConverter GetConverter(Type t, KType? kt, KSerializerOptions options)
    {
        // Create KArrayConverter
        if (t.IsArray)
        {
            var et = t.GetElementType()!;
            var elemConverter = options.GetConverter(et, kt!.Value.GetUnderlyingType());
            return (KTypeConverter)Activator.CreateInstance(typeof(KArrayConverter<>).MakeGenericType(et), kt, elemConverter)!;
        }
        else
        {
            throw new NotSupportedException();
        }
    }
}

public class KArrayConverter<TElem> : KTypeConverter<TElem[]>
{
    private readonly KType _kt;
    private readonly KTypeConverter<TElem>? _elemConverter = null;

    public override KType? TypeToWriteTo => _kt;

    public KArrayConverter(KType kt, KTypeConverter<TElem>? elemConverter)
    {
        _kt = kt;
        _elemConverter = elemConverter;
    }
    public override bool CanConvert(Type t, KType kt)
    {
        return KTypeHelper.IsAtomList(kt) && t.IsArray;
    }

    public override TElem[] Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        if (_elemConverter is null)
        {
            throw new InvalidOperationException();
        }
        reader.StartReadList();
        var count = reader.ListLength ?? throw new InvalidOperationException();

        var arr = new TElem[count];
        for (int i = 0; i < count; i++)
        {
            arr[i] = _elemConverter.Read(ref reader, options);
        }
        reader.EndReadList();
        return arr;
    }

    public override void Write(ref KSerializationWriter writer, TElem[] value, KSerializerOptions options)
    {
        if(_elemConverter is null)
        {
            throw new InvalidOperationException();
        }

        writer.StartWriteList(_kt, value.Length);
        foreach (var item in value)
        {
            _elemConverter.Write(ref writer, item, options);
        }
        writer.EndWriteList();
    }
}

// TODO: Not implemented
public class KAtomListConverter<TElem,T> : KTypeConverter<T>
{
    private readonly KType _kt;
    private readonly KTypeConverter<TElem>? _elemConverter = null;

    public override KType? TypeToWriteTo => _kt;

    public KAtomListConverter(KType kt, KTypeConverter<TElem>? elemConverter)
    {
        _kt = kt;
        _elemConverter = elemConverter;
    }
    public override bool CanConvert(Type t, KType kt)
    {
        return KTypeHelper.IsAtomList(kt) && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>);
    }

    public override T Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        if (_elemConverter is null)
        {
            throw new InvalidOperationException();
        }

        var count = reader.ListLength ?? throw new InvalidOperationException();

        var list = (IList<TElem>)Activator.CreateInstance<T>()!;
        for (int i = 0; i < count; i++)
        {
            list.Add(_elemConverter.Read(ref reader, options));
        }
        return (T)list;
    }

    public void Write(ref KSerializationWriter writer, IList<TElem> value, KSerializerOptions options)
    {
        if(_elemConverter is null)
        {
            throw new InvalidOperationException();
        }

        writer.StartWriteList(_kt, value.Count);
        foreach (var item in value)
        {
            _elemConverter.Write(ref writer, item, options);
        }
        writer.EndWriteList();
    }

    public override void Write(ref KSerializationWriter writer, T value, KSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
