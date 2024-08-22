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

public class KRealConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        if(kt == KType.Real)
        {
            if (t == typeof(KReal) || t.IsFloatingPoint())
            {
                return true;
            }
            else if (t.TryGetNullableUnderlying(out var type))
            {
                return type.IsFloatingPoint();
            }
        }
        return false;
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        return (KTypeConverter)(Activator.CreateInstance(typeof(KRealConverter<>).MakeGenericType(t))
                       ?? throw new InvalidOperationException($"Failed to create converter for {t}"));
    }
}

public class KRealConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Real;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        var rawValue = reader.ReadReal();
        var value = rawValue.Value;
        if (TypeToConvert == typeof(KReal))
        {
            return (T)(object)rawValue;
        }
        else if (TypeToConvert.TryGetNullableUnderlying(out var ut) && ut.IsFloatingPoint())
        {
            if (rawValue.IsNull)
            {
                return default!;
            }
            else
            {
                return ConverterHelper.ConvertToNullableFloatingPoint<float, T>(value)!;
            }
        }
        else if (TypeToConvert.IsFloatingPoint())
        {
            return (T)(object)ConverterHelper.ConvertFloatingPoint<float, T>(value)!;
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (value is KReal kreal)
        {
            writer.WriteReal(kreal);
        }
        else if (value is float f)
        {
            writer.WriteReal(new KReal(f));
        }
        else if (TypeToConvert == typeof(float?))
        {
            if (value is null)
            {
                writer.WriteReal(KReal.Null);
            }
            else
            {
                writer.WriteReal(new KReal((float)(object)value));
            }
        }
        else
        {
            throw new NotSupportedException();
        }
    }
}

// Float
public class KFloatConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        if (kt == KType.Float)
        {
            if (t == typeof(KFloat) || t.IsFloatingPoint())
            {
                return true;
            }
            else if (t.TryGetNullableUnderlying(out var type))
            {
                return type.IsFloatingPoint();
            }
        }
        return false;
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        return (KTypeConverter)(Activator.CreateInstance(typeof(KFloatConverter<>).MakeGenericType(t))
                                  ?? throw new InvalidOperationException($"Failed to create converter for {t}"));
    }
}

public class KFloatConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Float;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        var rawValue = reader.ReadFloat();
        var value = rawValue.Value;
        if (TypeToConvert == typeof(KFloat))
        {
            return (T)(object)rawValue;
        }
        else if (TypeToConvert.TryGetNullableUnderlying(out var ut) && ut.IsFloatingPoint())
        {
            if (rawValue.IsNull)
            {
                return default!;
            }
            else
            {
                return ConverterHelper.ConvertToNullableFloatingPoint<double, T>(value)!;
            }
        }
        else if (TypeToConvert.IsFloatingPoint())
        {
            return (T)(object)ConverterHelper.ConvertFloatingPoint<double, T>(value)!;
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (value is KFloat kfloat)
        {
            writer.WriteFloat(kfloat);
        }
        else if (value is double d)
        {
            writer.WriteFloat(new KFloat(d));
        }
        else if (TypeToConvert == typeof(double?))
        {
            if (value is null)
            {
                writer.WriteFloat(KFloat.Null);
            }
            else
            {
                writer.WriteFloat(new KFloat((double)(object)value));
            }
        }
        else
        {
            throw new NotSupportedException();
        }
    }
}




public class KShortConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        if (kt == KType.Short)
        {
            if (t == typeof(KShort) || t.IsInteger())
            {
                return true;
            }
            else if (t.TryGetNullableUnderlying(out var type))
            {
                return type.IsInteger();
            }
        }
        return false;
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        return (KTypeConverter)(Activator.CreateInstance(typeof(KShortConverter<>).MakeGenericType(t))
                       ?? throw new InvalidOperationException($"Failed to create converter for {t}"));
    }
}

public class KShortConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Short;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        var rawValue = reader.ReadShort();
        var value = rawValue.Value;
        if (TypeToConvert == typeof(KShort))
        {
            return (T)(object)rawValue;
        }
        else if (TypeToConvert.TryGetNullableUnderlying(out var ut) && ut.IsInteger())
        {
            if (rawValue.IsNull)
            {
                return default!;
            }
            else
            {
                return ConverterHelper.ConvertToNullableInteger<short, T>(value)!;
            }
        }
        else if (TypeToConvert.IsInteger())
        {
            return (T)(object)ConverterHelper.ConvertInteger<short, T>(value)!;
        }

        throw new NotSupportedException();
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (value is KShort kshort)
        {
            writer.WriteShort(kshort);
        }
        else if (value is short s)
        {
            writer.WriteShort(new KShort(s));
        }
        else if (TypeToConvert == typeof(short?))
        {
            if (value is null)
            {
                writer.WriteShort(KShort.Null);
            }
            else
            {
                writer.WriteShort(new KShort((short)(object)value));
            }
        }
        else
        {
            throw new NotSupportedException();
        }
    }
}

// Int
public class KIntConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        if (kt == KType.Int)
        {
            if (t == typeof(KInt) || t.IsInteger())
            {
                return true;
            }
            else if (t.TryGetNullableUnderlying(out var type))
            {
                return type.IsInteger();
            }
        }
        return false;
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        return (KTypeConverter)(Activator.CreateInstance(typeof(KIntConverter<>).MakeGenericType(t))
                                  ?? throw new InvalidOperationException($"Failed to create converter for {t}"));
    }
}

public class KIntConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Int;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        var rawValue = reader.ReadInt();
        var value = rawValue.Value;
        if (TypeToConvert == typeof(KInt))
        {
            return (T)(object)rawValue;
        }
        else if (TypeToConvert.TryGetNullableUnderlying(out var ut) && ut.IsInteger())
        {
            if (rawValue.IsNull)
            {
                return default!;
            }
            else
            {
                return ConverterHelper.ConvertToNullableInteger<int, T>(value)!;
            }
        }
        else if (TypeToConvert.IsInteger())
        {
            return (T)(object)ConverterHelper.ConvertInteger<int, T>(value)!;
        }

        throw new NotSupportedException();
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (value is KInt kint)
        {
            writer.WriteInt(kint);
        }
        else if (value is int i)
        {
            writer.WriteInt(new KInt(i));
        }
        else if (TypeToConvert == typeof(int?))
        {
            if (value is null)
            {
                writer.WriteInt(KInt.Null);
            }
            else
            {
                writer.WriteInt(new KInt((int)(object)value));
            }
        }
        else
        {
            throw new NotSupportedException();
        }
    }
}

// Long

public class KLongConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        if (kt == KType.Long)
        {
            if (t == typeof(KLong) || t.IsInteger())
            {
                return true;
            }
            else if (t.TryGetNullableUnderlying(out var type))
            {
                return type.IsInteger();
            }
        }
        return false;
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        return (KTypeConverter)(Activator.CreateInstance(typeof(KLongConverter<>).MakeGenericType(t))
                                             ?? throw new InvalidOperationException($"Failed to create converter for {t}"));
    }
}

public class KLongConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Long;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        var rawValue = reader.ReadLong();
        var value = rawValue.Value;
        if (TypeToConvert == typeof(KLong))
        {
            return (T)(object)rawValue;
        }
        else if (TypeToConvert.TryGetNullableUnderlying(out var ut) && ut.IsInteger())
        {
            if (rawValue.IsNull)
            {
                return default!;
            }
            else
            {
                return ConverterHelper.ConvertToNullableInteger<long, T>(value)!;
            }
        }
        else if (TypeToConvert.IsInteger())
        {
            return (T)(object)ConverterHelper.ConvertInteger<long, T>(value)!;
        }

        throw new NotSupportedException();
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (value is KLong klong)
        {
            writer.WriteLong(klong);
        }
        else if (value is long l)
        {
            writer.WriteLong(new KLong(l));
        }
        else if (TypeToConvert == typeof(long?))
        {
            if (value is null)
            {
                writer.WriteLong(KLong.Null);
            }
            else
            {
                writer.WriteLong(new KLong((long)(object)value));
            }
        }
        else
        {
            throw new NotSupportedException();
        }
    }
}
