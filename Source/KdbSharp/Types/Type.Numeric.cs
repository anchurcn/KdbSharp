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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KdbSharp.Types;

public readonly struct KLong : INullable, IEquatable<KLong>
{
    private readonly long _value;

    public static readonly KLong Zero = new KLong();
    public static readonly KLong Infinity = new KLong(long.MaxValue);
    public static readonly KLong NegativeInfinity = new KLong(-Infinity._value);
    public static readonly KLong Null = new KLong(long.MinValue);

    public KLong()
    {
        _value = 0;
    }

    public KLong(long value)
    {
        _value = value;
    }

    public bool IsNull => _value == long.MinValue;

    public long Value => _value;

    public bool Equals(KLong other) => _value == other._value;

    public override string ToString()
    {
        if (_value == Null._value)
        {
            return TypeNullString.Long;
        }
        else if (_value == Infinity._value)
        {
            return "0Wj";
        }
        else if (_value == -Infinity._value)
        {
            return "-0Wj";
        }
        else
        {
            return _value.ToString();
        }
    }
}

public readonly struct KInt : INullable, IEquatable<KInt>
{
    private readonly int _value;

    public static readonly KInt Zero = new KInt();
    public static readonly KInt Infinity = new KInt(int.MaxValue);
    public static readonly KInt NegativeInfinity = new KInt(-Infinity._value);
    public static readonly KInt Null = new KInt(int.MinValue);

    public KInt(int value)
    {
        _value = value;
    }

    public bool IsNull => _value == int.MinValue;

    public int Value => _value;

    public bool Equals(KInt other) => _value == other._value;

    public override string ToString()
    {
        if (_value == Null._value)
        {
            return TypeNullString.Int;
        }
        else if (_value == Infinity._value)
        {
            return "0Wi";
        }
        else if (_value == -Infinity._value)
        {
            return "-0Wi";
        }
        else
        {
            return _value.ToString();
        }
    }
}

public readonly struct KShort : INullable, IEquatable<KShort>
{
    private readonly short _value;

    public static readonly KShort Zero = new KShort();
    public static readonly KShort Infinity = new KShort(short.MaxValue);
    public static readonly KShort NegativeInfinity = new KShort((short)-Infinity._value);
    public static readonly KShort Null = new KShort(short.MinValue);

    public KShort(short value)
    {
        _value = value;
    }

    public bool IsNull => _value == short.MinValue;

    public short Value => _value;

    public bool Equals(KShort other) => _value == other._value;

    public override string ToString()
    {
        if (_value == Null._value)
        {
            return TypeNullString.Short;
        }
        else if (_value == Infinity._value)
        {
            return "0Wh";
        }
        else if (_value == -Infinity._value)
        {
            return "-0Wh";
        }
        else
        {
            return _value.ToString();
        }
    }
}
// Floating point types
public readonly struct KReal : INullable, IEquatable<KReal>
{
    private readonly float _value;

    public static readonly KReal Zero = new KReal();
    public static readonly KReal Infinity = new KReal(float.PositiveInfinity);
    public static readonly KReal NegativeInfinity = new KReal(float.NegativeInfinity);
    public static readonly KReal Null = new KReal(float.NaN);

    public KReal(float value)
    {
        _value = value;
    }

    public bool IsNull => float.IsNaN(_value);

    public float Value => _value;

    public bool Equals(KReal other) => _value == other._value;

    public override string ToString()
    {
        if (float.IsNaN(_value))
        {
            return "0Ne";
        }
        else if (_value == Infinity._value)
        {
            return "0w";
        }
        else if (_value == -Infinity._value)
        {
            return "-0w";
        }
        else
        {
            return _value.ToString();
        }
    }
}

public readonly struct KFloat : INullable, IEquatable<KFloat>
{
    private readonly double _value;

    public static readonly KFloat Zero = new KFloat();
    public static readonly KFloat Infinity = new KFloat(double.PositiveInfinity);
    public static readonly KFloat NegativeInfinity = new KFloat(double.NegativeInfinity);
    public static readonly KFloat Null = new KFloat(double.NaN);

    public KFloat(double value)
    {
        _value = value;
    }

    public bool IsNull => double.IsNaN(_value);

    public double Value => _value;

    public bool Equals(KFloat other) => _value == other._value;

    public override string ToString()
    {
        if (double.IsNaN(_value))
        {
            return "0n";
        }
        else if (_value == Infinity._value)
        {
            return "0w";
        }
        else if (_value == -Infinity._value)
        {
            return "-0w";
        }
        else
        {
            return _value.ToString();
        }
    }
}
