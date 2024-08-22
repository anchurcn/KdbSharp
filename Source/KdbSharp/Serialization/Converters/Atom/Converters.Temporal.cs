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


// KTimestampeConverterFactory
public class KTimestampeConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        return kt == KType.Timestamp
            && (t == typeof(KTimestamp) || t.IsTypeOfOrNullable<DateTime>());
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        var converterType = typeof(KTimestampConverter<>).MakeGenericType(t);
        return (KTypeConverter)Activator.CreateInstance(converterType)!;
    }
}

public class KTimestampConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Timestamp;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(DateTime))
        {
            return (T)(object)reader.ReadTimestamp().ToDateTime();
        }
        else if (TypeToConvert == typeof(DateTime?))
        {
            var res = reader.ReadTimestamp();
            return (T)(object)(res.IsNull ? default(DateTime?) : res.ToDateTime())!;
        }
        else if (TypeToConvert == typeof(KTimestamp))
        {
            return (T)(object)reader.ReadTimestamp();
        }
        throw new InvalidOperationException();
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(DateTime))
        {
            writer.WriteTimestamp(KTimestamp.FromDateTime(((DateTime)(object)value!)));
        }
        else if (TypeToConvert == typeof(DateTime?))
        {
            var v = (DateTime?)(object)value!;
            writer.WriteTimestamp(v.HasValue ? KTimestamp.FromDateTime(v.Value) : KTimestamp.Null);
        }
        else if (TypeToConvert == typeof(KTimestamp))
        {
            writer.WriteTimestamp((KTimestamp)(object)value!);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}

// KMonthConverter
public class KMonthConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        return kt == KType.Month
            && (t == typeof(KMonth) || t.IsTypeOfOrNullable<DateTime>());
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        var converterType = typeof(KMonthConverter<>).MakeGenericType(t);
        return (KTypeConverter)Activator.CreateInstance(converterType)!;
    }
}

public class KMonthConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Month;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(DateTime))
        {
            return (T)(object)reader.ReadMonth().ToDateTime();
        }
        else if (TypeToConvert == typeof(DateTime?))
        {
            var res = reader.ReadMonth();
            return (T)(object)(res.IsNull ? default(DateTime?) : res.ToDateTime())!;
        }
        else if (TypeToConvert == typeof(KMonth))
        {
            return (T)(object)reader.ReadMonth();
        }
        throw new InvalidOperationException();
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(DateTime))
        {
            writer.WriteMonth(KMonth.FromDateTime(((DateTime)(object)value!)));
        }
        else if (TypeToConvert == typeof(DateTime?))
        {
            var v = (DateTime?)(object)value!;
            writer.WriteMonth(v.HasValue ? KMonth.FromDateTime(v.Value) : KMonth.Null);
        }
        else if (TypeToConvert == typeof(KMonth))
        {
            writer.WriteMonth((KMonth)(object)value!);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}

// KDateConverter
public class KDateConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        return kt == KType.Date
            && (t == typeof(KDate) || t.IsTypeOfOrNullable<DateTime>());
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        var converterType = typeof(KDateConverter<>).MakeGenericType(t);
        return (KTypeConverter)Activator.CreateInstance(converterType)!;
    }
}

public class KDateConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Date;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(DateTime))
        {
            return (T)(object)reader.ReadDate().ToDateTime();
        }
        else if (TypeToConvert == typeof(DateTime?))
        {
            var res = reader.ReadDate();
            return (T)(object)(res.IsNull ? default(DateTime?) : res.ToDateTime())!;
        }
        else if (TypeToConvert == typeof(KDate))
        {
            return (T)(object)reader.ReadDate();
        }
        throw new InvalidOperationException();
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(DateTime))
        {
            writer.WriteDate(KDate.FromDateTime(((DateTime)(object)value!)));
        }
        else if (TypeToConvert == typeof(DateTime?))
        {
            var v = (DateTime?)(object)value!;
            writer.WriteDate(v.HasValue ? KDate.FromDateTime(v.Value) : KDate.Null);
        }
        else if (TypeToConvert == typeof(KDate))
        {
            writer.WriteDate((KDate)(object)value!);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}

// KDateTimeConverter
public class KDateTimeConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        return kt == KType.DateTime
            && (t == typeof(KDateTime) || t.IsTypeOfOrNullable<DateTime>());
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        var converterType = typeof(KDateTimeConverter<>).MakeGenericType(t);
        return (KTypeConverter)Activator.CreateInstance(converterType)!;
    }
}

public class KDateTimeConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.DateTime;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(DateTime))
        {
            return (T)(object)reader.ReadDateTime().ToDateTime();
        }
        else if (TypeToConvert == typeof(DateTime?))
        {
            var res = reader.ReadDateTime();
            return (T)(object)(res.IsNull ? default(DateTime?) : res.ToDateTime())!;
        }
        else if (TypeToConvert == typeof(KDateTime))
        {
            return (T)(object)reader.ReadDateTime();
        }
        throw new InvalidOperationException();
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(DateTime))
        {
            writer.WriteDateTime(KDateTime.FromDateTime(((DateTime)(object)value!)));
        }
        else if (TypeToConvert == typeof(DateTime?))
        {
            var v = (DateTime?)(object)value!;
            writer.WriteDateTime(v.HasValue ? KDateTime.FromDateTime(v.Value) : KDateTime.Null);
        }
        else if (TypeToConvert == typeof(KDateTime))
        {
            writer.WriteDateTime((KDateTime)(object)value!);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}

// KTimeSpanConverter
public class KTimeSpanConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        return kt == KType.TimeSpan
            && (t == typeof(KTimeSpan) || t.IsTypeOfOrNullable<TimeSpan>());
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        var converterType = typeof(KTimeSpanConverter<>).MakeGenericType(t);
        return (KTypeConverter)Activator.CreateInstance(converterType)!;
    }
}

public class KTimeSpanConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.TimeSpan;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(TimeSpan))
        {
            return (T)(object)reader.ReadTimeSpan().ToTimeSpan();
        }
        else if (TypeToConvert == typeof(TimeSpan?))
        {
            var res = reader.ReadTimeSpan();
            return (T)(object)(res.IsNull ? default(TimeSpan?) : res.ToTimeSpan())!;
        }
        else if (TypeToConvert == typeof(KTimeSpan))
        {
            return (T)(object)reader.ReadTimeSpan();
        }
        throw new InvalidOperationException();
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(TimeSpan))
        {
            writer.WriteTimeSpan(KTimeSpan.FromTimeSpan(((TimeSpan)(object)value!)));
        }
        else if (TypeToConvert == typeof(TimeSpan?))
        {
            var v = (TimeSpan?)(object)value!;
            writer.WriteTimeSpan(v.HasValue ? KTimeSpan.FromTimeSpan(v.Value) : KTimeSpan.Null);
        }
        else if (TypeToConvert == typeof(KTimeSpan))
        {
            writer.WriteTimeSpan((KTimeSpan)(object)value!);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}

// KMinuteConverter
public class KMinuteConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        return kt == KType.Minute
            && (t == typeof(KMinute) || t.IsTypeOfOrNullable<KTimeSpan>());
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        var converterType = typeof(KMinuteConverter<>).MakeGenericType(t);
        return (KTypeConverter)Activator.CreateInstance(converterType)!;
    }
}

public class KMinuteConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Minute;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(TimeSpan))
        {
            return (T)(object)reader.ReadMinute().ToTimeSpan();
        }
        else if (TypeToConvert == typeof(TimeSpan?))
        {
            var res = reader.ReadMinute();
            return (T)(object)(res.IsNull ? default(TimeSpan?) : res.ToTimeSpan())!;
        }
        else if (TypeToConvert == typeof(KMinute))
        {
            return (T)(object)reader.ReadMinute();
        }
        throw new InvalidOperationException();
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(TimeSpan))
        {
            writer.WriteMinute(KMinute.FromTimeSpan(((TimeSpan)(object)value!)));
        }
        else if (TypeToConvert == typeof(TimeSpan?))
        {
            var v = (TimeSpan?)(object)value!;
            writer.WriteMinute(v.HasValue ? KMinute.FromTimeSpan(v.Value) : KMinute.Null);
        }
        else if (TypeToConvert == typeof(KMinute))
        {
            writer.WriteMinute((KMinute)(object)value!);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}

// KSecondConverter

public class KSecondConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        return kt == KType.Second
            && (t == typeof(KSecond) || t.IsTypeOfOrNullable<KTimeSpan>());
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        var converterType = typeof(KSecondConverter<>).MakeGenericType(t);
        return (KTypeConverter)Activator.CreateInstance(converterType)!;
    }
}

public class KSecondConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Second;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(TimeSpan))
        {
            return (T)(object)reader.ReadSecond().ToTimeSpan();
        }
        else if (TypeToConvert == typeof(TimeSpan?))
        {
            var res = reader.ReadSecond();
            return (T)(object)(res.IsNull ? default(TimeSpan?) : res.ToTimeSpan())!;
        }
        else if (TypeToConvert == typeof(KSecond))
        {
            return (T)(object)reader.ReadSecond();
        }
        throw new InvalidOperationException();
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(TimeSpan))
        {
            writer.WriteSecond(KSecond.FromTimeSpan(((TimeSpan)(object)value!)));
        }
        else if (TypeToConvert == typeof(TimeSpan?))
        {
            var v = (TimeSpan?)(object)value!;
            writer.WriteSecond(v.HasValue ? KSecond.FromTimeSpan(v.Value) : KSecond.Null);
        }
        else if (TypeToConvert == typeof(KSecond))
        {
            writer.WriteSecond((KSecond)(object)value!);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}

// KTimeConverter
public class KTimeConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt)
    {
        return kt == KType.Time
            && (t == typeof(KTime) || t.IsTypeOfOrNullable<KTimeSpan>());
    }

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        var converterType = typeof(KTimeConverter<>).MakeGenericType(t);
        return (KTypeConverter)Activator.CreateInstance(converterType)!;
    }
}

public class KTimeConverter<T> : KTypeConverter<T>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return t == TypeToConvert && kt == KType.Time;
    }

    public override T Read(KReader reader, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(TimeSpan))
        {
            return (T)(object)reader.ReadTime().ToTimeSpan();
        }
        else if (TypeToConvert == typeof(TimeSpan?))
        {
            var res = reader.ReadTime();
            return (T)(object)(res.IsNull ? default(TimeSpan?) : res.ToTimeSpan())!;
        }
        else if (TypeToConvert == typeof(KTime))
        {
            return (T)(object)reader.ReadTime();
        }
        throw new InvalidOperationException();
    }

    public override void Write(KWriter writer, T value, KSerializerOptions options)
    {
        if (TypeToConvert == typeof(TimeSpan))
        {
            writer.WriteTime(KTime.FromTimeSpan(((TimeSpan)(object)value!)));
        }
        else if (TypeToConvert == typeof(TimeSpan?))
        {
            var v = (TimeSpan?)(object)value!;
            writer.WriteTime(v.HasValue ? KTime.FromTimeSpan(v.Value) : KTime.Null);
        }
        else if (TypeToConvert == typeof(KTime))
        {
            writer.WriteTime((KTime)(object)value!);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}
