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
using System.Diagnostics.CodeAnalysis;

namespace KdbSharp.Serialization;


public abstract class KTypeConverter
{
    public abstract Type? TypeToConvert { get; }

    public abstract bool CanConvert(Type t, KType kt);
    // Reads
    public abstract object? ReadAsObject(KReader reader, KSerializerOptions options);
    // Writes
    public abstract void WriteAsObject(KWriter writer, object? value, KSerializerOptions options);

    // [return: NotNullIfNotNull(nameof(value))]
    internal static T? UnboxOnWrite<T>(object? value)
    {
        if (default(T) is not null && value is null)
        {
            throw new ArgumentNullException(nameof(value), $"Cannot serialize null value for non-nullable type {typeof(T)}");
        }
        return (T?)value;
    }
}

public abstract class KTypeConverterFactory : KTypeConverter
{
    public abstract KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options);

    public sealed override Type? TypeToConvert => null;

    public sealed override object? ReadAsObject(KReader reader, KSerializerOptions options)
    {
        throw new InvalidOperationException("We should never get here.");
    }
    public sealed override void WriteAsObject(KWriter writer, object? value, KSerializerOptions options)
    {
        throw new InvalidOperationException("We should never get here.");
    }
}

public abstract class KTypeConverter<T> : KTypeConverter
{
    public override Type TypeToConvert => typeof(T);
    public override object? ReadAsObject(KReader reader, KSerializerOptions options)
        => Read(reader, options);
    public override void WriteAsObject(KWriter writer, object? value, KSerializerOptions options)
    {
        Write(writer, UnboxOnWrite<T>(value)!, options);
    }
    // abstract members
    public abstract T Read(KReader reader, KSerializerOptions options);
    public abstract void Write(KWriter writer, T value, KSerializerOptions options);
}


public static class TypeConverterHelper
{
    public static bool CompatibleWith<T>(this Type t)
    {
        throw new NotImplementedException();
    }
}
