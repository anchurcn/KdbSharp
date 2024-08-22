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

namespace KdbSharp.Extensions;

public static class ReflectionExtensions
{
    public static bool IsNullable(this Type type)
    {
        return type.TryGetNullableUnderlying(out _);
    }
    public static bool IsNullableOf(this Type type, Type underlying)
    {
        return type.TryGetNullableUnderlying(out var u) && u == underlying;
    }
    public static bool TryGetNullableUnderlying(this Type type, out Type underlying)
    {
        underlying = Nullable.GetUnderlyingType(type)!;
        return underlying is not null;
    }
    public static object? GetDefaultValue(this Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        return null;
    }
    public static bool IsNumeric(this Type type)
    {
        return type.IsInteger() || type.IsFloatingPoint();
    }
    public static bool IsFloatingPoint(this Type type)
    {
        return type == typeof(float) || type == typeof(double) || type == typeof(decimal);
    }
    public static bool IsSignedInteger(this Type type)
    {
        return type == typeof(sbyte) || type == typeof(short) || type == typeof(int) || type == typeof(long);
    }
    public static bool IsUnsignedInteger(this Type type)
    {
        return type == typeof(byte) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong);
    }
    public static bool IsInteger(this Type type)
    {
        return type.IsSignedInteger() || type.IsUnsignedInteger();
    }

    public static bool IsTypeOfOrNullable<T>(this Type type)
    {
        return type == typeof(T) || type.IsNullableOf(typeof(T));
    }
}
