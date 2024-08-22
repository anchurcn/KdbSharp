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
using KdbSharp.Extensions;
using KdbSharp.Types;

namespace KdbSharp.Serialization.Converters;


// KBooleanConverter
public class KBooleanConverter : KTypeConverter<bool>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Boolean;
    }
    public override bool Read(KReader reader, KSerializerOptions options)
    {
        return reader.ReadBoolean();
    }
    public override void Write(KWriter writer, bool value, KSerializerOptions options)
    {
        writer.WriteBoolean(value);
    }
}

// KGuidConverter
public class KGuidConverter : KTypeConverter<Guid>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Guid;
    }
    public override Guid Read(KReader reader, KSerializerOptions options)
    {
        return reader.ReadGuid();
    }
    public override void Write(KWriter writer, Guid value, KSerializerOptions options)
    {
        writer.WriteGuid(value);
    }
}

// KByteConverter
public class KByteConverter : KTypeConverter<byte>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Byte;
    }
    public override byte Read(KReader reader, KSerializerOptions options)
    {
        return reader.ReadByte();
    }
    public override void Write(KWriter writer, byte value, KSerializerOptions options)
    {
        writer.WriteByte(value);
    }
}

// KCharConverter
public class KCharConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        return kt == KType.Char
            && (t == typeof(char) || t == typeof(KChar));
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        if (t == typeof(char))
        {
            return new KCharConverter<char>();
        }
        else if (t == typeof(KChar))
        {
            return new KCharConverter<KChar>();
        }
        throw new InvalidOperationException();
    }
}

public class KCharConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Char;
    }
    public override T Read(KReader reader, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(char))
        {
            return (T)(object)(char)reader.ReadChar().Value;
        }
        else if (TypeToConvert == typeof(KChar))
        {
            return (T)(object)reader.ReadChar();
        }
        throw new InvalidOperationException();
    }
    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(char))
        {
            if ((char)(object)value! > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Char value must be less than 256.");
            }
            writer.WriteChar(new KChar((sbyte)(object)value!));
        }
        else if (TypeToConvert == typeof(KChar))
        {
            writer.WriteChar((KChar)(object)value!);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}

// KSymbolConverter
public class KSymbolConverter : KTypeConverter<string>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Symbol;
    }

    public override string Read(KReader reader, KSerializerOptions options)
    {
        return reader.ReadSymbol();
    }

    public override void Write(KWriter writer, string value, KSerializerOptions options)
    {
        writer.WriteSymbol(value);
    }
}
