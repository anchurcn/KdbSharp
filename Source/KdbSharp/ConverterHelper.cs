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
using System.Runtime.CompilerServices;

namespace KdbSharp;

public class ConverterHelper
{
    // implements a generic number converter that checks overflow
    // usage: ConvertNum<long, int>(long.Maxvalue) // throws OverflowException
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDest ConvertInteger<TSrc, TDest>(TSrc value) where TSrc : struct
    {
        checked
        {
            var srcType = typeof(TSrc);
            var destType = typeof(TDest);
            if (srcType == destType)
            {
                return (TDest)(object)value;
            }
            if (srcType == typeof(long))
            {
                if (destType == typeof(ulong))
                {
                    return (TDest)(object)(ulong)(long)(object)value;
                }
                else if (destType == typeof(int))
                {
                    return (TDest)(object)(int)(long)(object)value;
                }
                else if (destType == typeof(uint))
                {
                    return (TDest)(object)(uint)(long)(object)value;
                }
                else if (destType == typeof(short))
                {
                    return (TDest)(object)(short)(long)(object)value;
                }
                else if (destType == typeof(ushort))
                {
                    return (TDest)(object)(ushort)(long)(object)value;
                }
                else if (destType == typeof(byte))
                {
                    return (TDest)(object)(byte)(long)(object)value;
                }
                else if (destType == typeof(sbyte))
                {
                    return (TDest)(object)(sbyte)(long)(object)value;
                }
                else
                {
                    throw new NotSupportedException($"Type {destType} is not supported.");
                }
            }
            else if (srcType == typeof(ulong))
            {
                if (destType == typeof(long))
                {
                    return (TDest)(object)(long)(ulong)(object)value;
                }
                else if (destType == typeof(int))
                {
                    return (TDest)(object)(int)(ulong)(object)value;
                }
                else if (destType == typeof(uint))
                {
                    return (TDest)(object)(uint)(ulong)(object)value;
                }
                else if (destType == typeof(short))
                {
                    return (TDest)(object)(short)(ulong)(object)value;
                }
                else if (destType == typeof(ushort))
                {
                    return (TDest)(object)(ushort)(ulong)(object)value;
                }
                else if (destType == typeof(byte))
                {
                    return (TDest)(object)(byte)(ulong)(object)value;
                }
                else if (destType == typeof(sbyte))
                {
                    return (TDest)(object)(sbyte)(ulong)(object)value;
                }
                else
                {
                    throw new NotSupportedException($"Type {destType} is not supported.");
                }
            }
            else if (srcType == typeof(int))
            {
                if (destType == typeof(long))
                {
                    return (TDest)(object)(long)(int)(object)value;
                }
                else if (destType == typeof(ulong))
                {
                    return (TDest)(object)(ulong)(int)(object)value;
                }
                else if (destType == typeof(uint))
                {
                    return (TDest)(object)(uint)(int)(object)value;
                }
                else if (destType == typeof(short))
                {
                    return (TDest)(object)(short)(int)(object)value;
                }
                else if (destType == typeof(ushort))
                {
                    return (TDest)(object)(ushort)(int)(object)value;
                }
                else if (destType == typeof(byte))
                {
                    return (TDest)(object)(byte)(int)(object)value;
                }
                else if (destType == typeof(sbyte))
                {
                    return (TDest)(object)(sbyte)(int)(object)value;
                }
                else
                {
                    throw new NotSupportedException($"Type {destType} is not supported.");
                }
            }
            else if (srcType == typeof(uint))
            {
                if (destType == typeof(long))
                {
                    return (TDest)(object)(long)(uint)(object)value;
                }
                else if (destType == typeof(ulong))
                {
                    return (TDest)(object)(ulong)(uint)(object)value;
                }
                else if (destType == typeof(int))
                {
                    return (TDest)(object)(int)(uint)(object)value;
                }
                else if (destType == typeof(short))
                {
                    return (TDest)(object)(short)(uint)(object)value;
                }
                else if (destType == typeof(ushort))
                {
                    return (TDest)(object)(ushort)(uint)(object)value;
                }
                else if (destType == typeof(byte))
                {
                    return (TDest)(object)(byte)(uint)(object)value;
                }
                else if (destType == typeof(sbyte))
                {
                    return (TDest)(object)(sbyte)(uint)(object)value;
                }
                else
                {
                    throw new NotSupportedException($"Type {destType} is not supported.");
                }
            }
            else if (srcType == typeof(short))
            {
                if (destType == typeof(long))
                {
                    return (TDest)(object)(long)(short)(object)value;
                }
                else if (destType == typeof(ulong))
                {
                    return (TDest)(object)(ulong)(short)(object)value;
                }
                else if (destType == typeof(int))
                {
                    return (TDest)(object)(int)(short)(object)value;
                }
                else if (destType == typeof(uint))
                {
                    return (TDest)(object)(uint)(short)(object)value;
                }
                else if (destType == typeof(ushort))
                {
                    return (TDest)(object)(ushort)(short)(object)value;
                }
                else if (destType == typeof(byte))
                {
                    return (TDest)(object)(byte)(short)(object)value;
                }
                else if (destType == typeof(sbyte))
                {
                    return (TDest)(object)(sbyte)(short)(object)value;
                }
                else
                {
                    throw new NotSupportedException($"Type {destType} is not supported.");
                }
            }
            else if (srcType == typeof(ushort))
            {
                if (destType == typeof(long))
                {
                    return (TDest)(object)(long)(ushort)(object)value;
                }
                else if (destType == typeof(ulong))
                {
                    return (TDest)(object)(ulong)(ushort)(object)value;
                }
                else if (destType == typeof(int))
                {
                    return (TDest)(object)(int)(ushort)(object)value;
                }
                else if (destType == typeof(uint))
                {
                    return (TDest)(object)(uint)(ushort)(object)value;
                }
                else if (destType == typeof(short))
                {
                    return (TDest)(object)(short)(ushort)(object)value;
                }
                else if (destType == typeof(byte))
                {
                    return (TDest)(object)(byte)(ushort)(object)value;
                }
                else if (destType == typeof(sbyte))
                {
                    return (TDest)(object)(sbyte)(ushort)(object)value;
                }
                else
                {
                    throw new NotSupportedException($"Type {destType} is not supported.");
                }
            }
            else if (srcType == typeof(byte))
            {
                if (destType == typeof(long))
                {
                    return (TDest)(object)(long)(byte)(object)value;
                }
                else if (destType == typeof(ulong))
                {
                    return (TDest)(object)(ulong)(byte)(object)value;
                }
                else if (destType == typeof(int))
                {
                    return (TDest)(object)(int)(byte)(object)value;
                }
                else if (destType == typeof(uint))
                {
                    return (TDest)(object)(uint)(byte)(object)value;
                }
                else if (destType == typeof(short))
                {
                    return (TDest)(object)(short)(byte)(object)value;
                }
                else if (destType == typeof(ushort))
                {
                    return (TDest)(object)(ushort)(byte)(object)value;
                }
                else if (destType == typeof(sbyte))
                {
                    return (TDest)(object)(sbyte)(byte)(object)value;
                }
                else
                {
                    throw new NotSupportedException($"Type {destType} is not supported.");
                }
            }
            else if (srcType == typeof(sbyte))
            {
                if (destType == typeof(long))
                {
                    return (TDest)(object)(long)(sbyte)(object)value;
                }
                else if (destType == typeof(ulong))
                {
                    return (TDest)(object)(ulong)(sbyte)(object)value;
                }
                else if (destType == typeof(int))
                {
                    return (TDest)(object)(int)(sbyte)(object)value;
                }
                else if (destType == typeof(uint))
                {
                    return (TDest)(object)(uint)(sbyte)(object)value;
                }
                else if (destType == typeof(short))
                {
                    return (TDest)(object)(short)(sbyte)(object)value;
                }
                else if (destType == typeof(ushort))
                {
                    return (TDest)(object)(ushort)(sbyte)(object)value;
                }
                else if (destType == typeof(byte))
                {
                    return (TDest)(object)(byte)(sbyte)(object)value;
                }
                else
                {
                    throw new NotSupportedException($"Type {destType} is not supported.");
                }
            }
            else
            {
                throw new NotSupportedException($"Type {srcType} is not supported.");
            }
        }
    }

    public static U ConvertToNullableInteger<T, U>(T value)
    {
        return ConvertToNullableIntegerCache<T, U>.Convert(value);
    }

    internal static T2 ConvertFloatingPoint<T1, T2>(T1 value) where T1 : struct
    {
        checked
        {
            var srcType = typeof(T1);
            var destType = typeof(T2);
            if (srcType == destType)
            {
                return (T2)(object)value;
            }
            if (srcType == typeof(float))
            {
                if (destType == typeof(double))
                {
                    return (T2)(object)(double)(float)(object)value;
                }
                else if (destType == typeof(decimal))
                {
                    return (T2)(object)(decimal)(float)(object)value;
                }
                else
                {
                    throw new NotSupportedException($"Type {destType} is not supported.");
                }
            }
            else if (srcType == typeof(double))
            {
                if (destType == typeof(float))
                {
                    return (T2)(object)(float)(double)(object)value;
                }
                else if (destType == typeof(decimal))
                {
                    return (T2)(object)(decimal)(double)(object)value;
                }
                else
                {
                    throw new NotSupportedException($"Type {destType} is not supported.");
                }
            }
            else if (srcType == typeof(decimal))
            {
                if (destType == typeof(float))
                {
                    return (T2)(object)(float)(decimal)(object)value;
                }
                else if (destType == typeof(double))
                {
                    return (T2)(object)(double)(decimal)(object)value;
                }
                else
                {
                    throw new NotSupportedException($"Type {destType} is not supported.");
                }
            }
            else
            {
                throw new NotSupportedException($"Type {srcType} is not supported.");
            }
        }
    }

    internal static T2 ConvertToNullableFloatingPoint<T1, T2>(T1 value)
    {
        return ConvertToNullableFloatingPointCache<T1, T2>.Convert(value);
    }

    static class ConvertToNullableIntegerCache<T, U>
    {
        static Func<T, U> s_func;
        static ConvertToNullableIntegerCache()
        {
            var typeofU = typeof(U);
            if (typeofU.IsNullable() && typeofU.TryGetNullableUnderlying(out var underlying) && underlying.IsInteger())
            {
                var method = typeof(NullableIntConverter<,>).MakeGenericType(typeof(T), underlying).GetMethod("Convert")!;
                s_func = (Func<T, U>)Delegate.CreateDelegate(typeof(Func<T, U>), method);
            }
            else
            {
                s_func = null!;
            }
        }
        public static U Convert(T value) => s_func is not null ? s_func(value) : throw new NotSupportedException();
    }
    static class NullableIntConverter<TIntSrc, TIntDst> where TIntSrc : unmanaged where TIntDst : unmanaged
    {
        public static TIntDst? Convert(TIntSrc value)
        {
            var convertedValue = ConvertInteger<TIntSrc, TIntDst>(value);
            return new TIntDst?(convertedValue);
        }
    }

    static class NullableFloatingPointConverter<TFloatSrc, TFloatDst> where TFloatSrc : unmanaged where TFloatDst : unmanaged
    {
        public static TFloatDst? Convert(TFloatSrc value)
        {
            var convertedValue = ConvertFloatingPoint<TFloatSrc, TFloatDst>(value);
            return new TFloatDst?(convertedValue);
        }
    }

    static class ConvertToNullableFloatingPointCache<T, U>
    {
        static Func<T, U> s_func;
        static ConvertToNullableFloatingPointCache()
        {
            var typeofU = typeof(U);
            if (typeofU.IsNullable() && typeofU.TryGetNullableUnderlying(out var underlying) && underlying.IsFloatingPoint())
            {
                var method = typeof(NullableFloatingPointConverter<,>).MakeGenericType(typeof(T), underlying).GetMethod("Convert")!;
                s_func = (Func<T, U>)Delegate.CreateDelegate(typeof(Func<T, U>), method);
            }
            else
            {
                s_func = null!;
            }
        }
        public static U Convert(T value) => s_func is not null ? s_func(value) : throw new NotSupportedException();
    }
}
