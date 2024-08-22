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
namespace KdbSharp.Types;

public static class TemporalHelper
{
    public static DateTime MillenniumY2K { get; } = new DateTime(2000, 1, 1);
    public static TimeSpan Midnight { get; } = default;
}
public readonly struct KTimestamp : IEquatable<KTimestamp>, IComparable<KTimestamp>, IComparable, INullable
{

    private readonly long _value;

    public KTimestamp(long value)
    {
        _value = value;
    }

    public static readonly KTimestamp Zero = new KTimestamp();
    public static readonly KTimestamp Infinity = new KTimestamp(long.MaxValue);
    public static readonly KTimestamp NegativeInfinity = new KTimestamp(-Infinity._value);
    public static readonly KTimestamp Null = new KTimestamp(long.MinValue);

    public readonly bool IsNull => Equals(Null);

    public readonly long Value => _value;

    #region Equality

    public bool Equals(KTimestamp other)
    {
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        return obj is KTimestamp ts && Equals(ts);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static bool Equals(KTimestamp left, KTimestamp right)
    {
        return left.Equals(right);
    }

    public static bool operator ==(KTimestamp left, KTimestamp right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(KTimestamp left, KTimestamp right)
    {
        return !(left == right);
    }
    #endregion

    #region Comparison

    public int CompareTo(KTimestamp other)
    {
        return _value.CompareTo(other._value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is KTimestamp ts)
        {
            return CompareTo(ts);
        }
        else
        {
            throw new ArgumentException("Object is not a KTimestamp.");
        }
    }

    public static int Compare(KTimestamp left, KTimestamp right)
    {
        return left.CompareTo(right);
    }

    public static bool operator >(KTimestamp left, KTimestamp right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <(KTimestamp left, KTimestamp right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >=(KTimestamp left, KTimestamp right)
    {
        return left.CompareTo(right) >= 0;
    }

    public static bool operator <=(KTimestamp left, KTimestamp right)
    {
        return left.CompareTo(right) <= 0;
    }
    #endregion

    public override string ToString()
    {
        if (Equals(Null))
        {
            return "0Np";
        }
        else if (Equals(Infinity))
        {
            return "0Wp";
        }
        else if (Equals(NegativeInfinity))
        {
            return "-0Wp";
        }
        else
        {
            var dt = new DateTime(TemporalHelper.MillenniumY2K.Ticks + _value / 100);
            var nanosecondDigits = _value % 100;
            return dt.ToString("yyyy.MM.dd'D'HH:mm:ss.fffffff") + nanosecondDigits.ToString("D2");
        }
    }

    public DateTime ToDateTime()
    {
        return TemporalHelper.MillenniumY2K.AddTicks(_value / 100);
    }

    public static KTimestamp FromDateTime(DateTime dt)
    {
        return new KTimestamp((dt.Ticks - TemporalHelper.MillenniumY2K.Ticks) * 100);
    }
}

public readonly struct KMonth : IEquatable<KMonth>, IComparable<KMonth>, IComparable, INullable
{

    private readonly int _value;

    public KMonth(int value)
    {
        _value = value;
    }

    public static readonly KMonth Zero = new KMonth();
    public static readonly KMonth Infinity = new KMonth(int.MaxValue);
    public static readonly KMonth NegativeInfinity = new KMonth(-Infinity._value);
    public static readonly KMonth Null = new KMonth(int.MinValue);

    public int Value => _value;

    public bool IsNull => Equals(Null);

    #region Equality

    public bool Equals(KMonth other) => _value == other._value;

    public override bool Equals(object? obj) => obj is KMonth m && Equals(m);

    public override int GetHashCode() => _value.GetHashCode();

    public static bool Equals(KMonth left, KMonth right) => left.Equals(right);

    public static bool operator ==(KMonth left, KMonth right) => left.Equals(right);

    public static bool operator !=(KMonth left, KMonth right) => !(left == right);

    #endregion

    #region Comparison

    public int CompareTo(KMonth other) => _value.CompareTo(other._value);

    public int CompareTo(object? obj)
    {
        if (obj is KMonth m)
        {
            return CompareTo(m);
        }
        else
        {
            throw new ArgumentException("Object is not a KMonth.");
        }
    }

    public static int Compare(KMonth left, KMonth right) => left.CompareTo(right);

    public static int CompareTo(KMonth left, KMonth right) => left.CompareTo(right);

    public static bool operator >(KMonth left, KMonth right) => left.CompareTo(right) > 0;

    public static bool operator <(KMonth left, KMonth right) => left.CompareTo(right) < 0;

    public static bool operator >=(KMonth left, KMonth right) => left.CompareTo(right) >= 0;

    public static bool operator <=(KMonth left, KMonth right) => left.CompareTo(right) <= 0;

    #endregion

    public override string ToString()
    {
        if (Equals(Null))
        {
            return "0Nm";
        }
        else if (Equals(Infinity))
        {
            return "0Wm";
        }
        else if (Equals(NegativeInfinity))
        {
            return "-0Wm";
        }
        else
        {
            var dt = ToDateTime();
            return dt.ToString("yyyy.MM") + "m";
        }
    }

    public DateTime ToDateTime()
    {
        return TemporalHelper.MillenniumY2K.AddMonths(_value);
    }

    public static KMonth FromDateTime(DateTime dt)
    {
        return new KMonth((dt.Year - TemporalHelper.MillenniumY2K.Year) * 12 + dt.Month);
    }
}

public readonly struct KDate : IEquatable<KDate>, IComparable<KDate>, IComparable, INullable
{
    private readonly int _value;

    public KDate(int value)
    {
        _value = value;
    }

    public static readonly KDate Zero = new KDate();
    public static readonly KDate Infinity = new KDate(int.MaxValue);
    public static readonly KDate NegativeInfinity = new KDate(-Infinity._value);
    public static readonly KDate Null = new KDate(int.MinValue);

    public int Value => _value;

    public bool IsNull => Equals(Null);

    #region Equality

    public bool Equals(KDate other) => _value == other._value;

    public override bool Equals(object? obj) => obj is KDate d && Equals(d);

    public override int GetHashCode() => _value.GetHashCode();

    public static bool Equals(KDate left, KDate right) => left.Equals(right);

    public static bool operator ==(KDate left, KDate right) => left.Equals(right);

    public static bool operator !=(KDate left, KDate right) => !(left == right);

    #endregion

    #region Comparison

    public int CompareTo(KDate other) => _value.CompareTo(other._value);

    public int CompareTo(object? obj)
    {
        if (obj is KDate d)
        {
            return CompareTo(d);
        }
        else
        {
            throw new ArgumentException("Object is not a KDate.");
        }
    }

    public static int Compare(KDate left, KDate right) => left.CompareTo(right);

    public static int CompareTo(KDate left, KDate right) => left.CompareTo(right);

    public static bool operator >(KDate left, KDate right) => left.CompareTo(right) > 0;

    public static bool operator <(KDate left, KDate right) => left.CompareTo(right) < 0;

    public static bool operator >=(KDate left, KDate right) => left.CompareTo(right) >= 0;

    public static bool operator <=(KDate left, KDate right) => left.CompareTo(right) <= 0;

    #endregion

    public override string ToString()
    {
        if (Equals(Null))
        {
            return "0Nd";
        }
        else if (Equals(Infinity))
        {
            return "0Wd";
        }
        else if (Equals(NegativeInfinity))
        {
            return "-0Wd";
        }
        else
        {
            var dt = ToDateTime();
            return dt.ToString("yyyy.MM.dd");
        }
    }

    public DateTime ToDateTime()
    {
        return TemporalHelper.MillenniumY2K.AddDays(_value);
    }

    public static KDate FromDateTime(DateTime dt)
    {
        return new KDate((int)(dt - TemporalHelper.MillenniumY2K).TotalDays);
    }

}

/// <summary>
/// Represents the number of days since the millennium year 2000.
/// </summary>
public readonly struct KDateTime : IEquatable<KDateTime>, IComparable<KDateTime>, IComparable, INullable
{
    private readonly double _value;

    public KDateTime(double value)
    {
        _value = value;
    }

    public static readonly KDateTime Zero = new KDateTime();
    public static readonly KDateTime Infinity = new KDateTime(double.PositiveInfinity);
    public static readonly KDateTime NegativeInfinity = new KDateTime(double.NegativeInfinity);
    public static readonly KDateTime Null = new KDateTime(double.NaN);

    public double Value => _value;

    public bool IsNull => Equals(Null);

    #region Equality

    public bool Equals(KDateTime other) => _value == other._value;

    public override bool Equals(object? obj) => obj is KDateTime dt && Equals(dt);

    public override int GetHashCode() => _value.GetHashCode();

    public static bool Equals(KDateTime left, KDateTime right) => left.Equals(right);

    public static bool operator ==(KDateTime left, KDateTime right) => left.Equals(right);

    public static bool operator !=(KDateTime left, KDateTime right) => !(left == right);

    #endregion

    #region Comparison

    public int CompareTo(KDateTime other) => _value.CompareTo(other._value);

    public int CompareTo(object? obj)
    {
        if (obj is KDateTime dt)
        {
            return CompareTo(dt);
        }
        else
        {
            throw new ArgumentException("Object is not a KDateTime.");
        }
    }

    public static int Compare(KDateTime left, KDateTime right) => left.CompareTo(right);

    public static int CompareTo(KDateTime left, KDateTime right) => left.CompareTo(right);

    public static bool operator >(KDateTime left, KDateTime right) => left.CompareTo(right) > 0;

    public static bool operator <(KDateTime left, KDateTime right) => left.CompareTo(right) < 0;

    public static bool operator >=(KDateTime left, KDateTime right) => left.CompareTo(right) >= 0;

    public static bool operator <=(KDateTime left, KDateTime right) => left.CompareTo(right) <= 0;

    #endregion

    public override string ToString()
    {
        if (Equals(Null))
        {
            return "0Nz";
        }
        else if (Equals(Infinity))
        {
            return "0Wz";
        }
        else if (Equals(NegativeInfinity))
        {
            return "-0Wz";
        }
        else
        {
            var dt = ToDateTime();
            return dt.ToString("yyyy.MM.dd'T'HH:mm:ss.fff");
        }
    }

    public DateTime ToDateTime()
    {
        return TemporalHelper.MillenniumY2K.AddDays(_value);
    }

    public static KDateTime FromDateTime(DateTime dt)
    {
        return new KDateTime((dt - TemporalHelper.MillenniumY2K).TotalDays);
    }
}
/// <summary>
/// represents the number of nanoseconds since midnight.
/// </summary>
public readonly struct KTimeSpan : IEquatable<KTimeSpan>, IComparable<KTimeSpan>, IComparable, INullable
{
    private readonly long _value;

    public KTimeSpan(long value)
    {
        _value = value;
    }

    public static readonly KTimeSpan Zero = new KTimeSpan();
    public static readonly KTimeSpan Infinity = new KTimeSpan(long.MaxValue);
    public static readonly KTimeSpan NegativeInfinity = new KTimeSpan(-Infinity._value);
    public static readonly KTimeSpan Null = new KTimeSpan(long.MinValue);

    /// <summary>
    /// Gets the value of the timespan in ticks (100 nanoseconds).
    /// </summary>
    public readonly long Value => _value;

    public readonly bool IsNull => Equals(Null);

    #region Equality

    public bool Equals(KTimeSpan other)
    {
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        return obj is KTimeSpan ts && Equals(ts);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static bool Equals(KTimeSpan left, KTimeSpan right)
    {
        return left.Equals(right);
    }

    public static bool operator ==(KTimeSpan left, KTimeSpan right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(KTimeSpan left, KTimeSpan right)
    {
        return !(left == right);
    }
    #endregion

    #region Comparison

    public int CompareTo(KTimeSpan other)
    {
        return _value.CompareTo(other._value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is KTimeSpan ts)
        {
            return CompareTo(ts);
        }
        else
        {
            throw new ArgumentException("Object is not a KdbTimeSpan.");
        }
    }

    public static int Compare(KTimeSpan left, KTimeSpan right)
    {
        return left.CompareTo(right);
    }

    public static bool operator >(KTimeSpan left, KTimeSpan right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <(KTimeSpan left, KTimeSpan right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >=(KTimeSpan left, KTimeSpan right)
    {
        return left.CompareTo(right) >= 0;
    }

    public static bool operator <=(KTimeSpan left, KTimeSpan right)
    {
        return left.CompareTo(right) <= 0;
    }
    #endregion

    #region Arithmetic

    /// <summary>
    /// Returns a new KdbTimeSpan object that represents the timespan added to the current timespan.
    /// </summary>
    public KTimeSpan Add(KTimeSpan ts)
    {
        return new KTimeSpan(_value + ts._value);
    }
    /// <summary>
    /// Returns a new KdbTimeSpan object that represents the timespan subtracted from the current timespan.
    /// </summary>
    public KTimeSpan Subtract(KTimeSpan ts)
    {
        return new KTimeSpan(_value - ts._value);
    }

    public static KTimeSpan operator +(KTimeSpan left, KTimeSpan right)
    {
        return left.Add(right);
    }

    public static KTimeSpan operator -(KTimeSpan left, KTimeSpan right)
    {
        return left.Subtract(right);
    }
    #endregion

    /// <summary>
    /// Returns a string representation of the timespan in KDB's time literal format.
    /// For example: 12:34:56.789
    /// </summary>
    public override string ToString()
    {
        if (Equals(Null))
        {
            return "0Nn";
        }
        else if (Equals(Infinity))
        {
            return "0Wn";
        }
        else if (Equals(NegativeInfinity))
        {
            return "-0Wn";
        }
        else
        {
            TimeSpan ts = ToTimeSpan();
            var nanosecondDigits = _value % 100;
            return ts.ToString("hh':'mm':'ss'.'fffffff") + nanosecondDigits.ToString("D2");
        }
    }

    public TimeSpan ToTimeSpan()
    {
        return TimeSpan.FromTicks(_value / 100);
    }

    public KTimeSpan FromDateTime(DateTime dt)
    {
        return new KTimeSpan(dt.TimeOfDay.Ticks * 100);
    }

    public static KTimeSpan FromTimeSpan(TimeSpan ts)
    {
        return new KTimeSpan(ts.Ticks * 100);
    }
}

public readonly struct KMinute : IEquatable<KMinute>, IComparable<KMinute>, IComparable, INullable
{
    private readonly int _value;

    public KMinute(int value)
    {
        _value = value;
    }

    public static readonly KMinute Zero = new KMinute();
    public static readonly KMinute Infinity = new KMinute(int.MaxValue);
    public static readonly KMinute NegativeInfinity = new KMinute(-Infinity._value);
    public static readonly KMinute Null = new KMinute(int.MinValue);

    public int Value => _value;

    public bool IsNull => Equals(Null);

    #region Equality

    public bool Equals(KMinute other) => _value == other._value;

    public override bool Equals(object? obj) => obj is KMinute m && Equals(m);

    public override int GetHashCode() => _value.GetHashCode();

    public static bool Equals(KMinute left, KMinute right) => left.Equals(right);

    public static bool operator ==(KMinute left, KMinute right) => left.Equals(right);

    public static bool operator !=(KMinute left, KMinute right) => !(left == right);

    #endregion

    #region Comparison

    public int CompareTo(KMinute other) => _value.CompareTo(other._value);

    public int CompareTo(object? obj)
    {
        if (obj is KMinute m)
        {
            return CompareTo(m);
        }
        else
        {
            throw new ArgumentException("Object is not a KMinute.");
        }
    }

    public static int Compare(KMinute left, KMinute right) => left.CompareTo(right);

    public static int CompareTo(KMinute left, KMinute right) => left.CompareTo(right);

    public static bool operator >(KMinute left, KMinute right) => left.CompareTo(right) > 0;

    public static bool operator <(KMinute left, KMinute right) => left.CompareTo(right) < 0;

    public static bool operator >=(KMinute left, KMinute right) => left.CompareTo(right) >= 0;

    public static bool operator <=(KMinute left, KMinute right) => left.CompareTo(right) <= 0;

    #endregion

    public override string ToString()
    {
        if (Equals(Null))
        {
            return "0Nu";
        }
        else if (Equals(Infinity))
        {
            return "0Wu";
        }
        else if (Equals(NegativeInfinity))
        {
            return "-0Wu";
        }
        else
        {
            var ts = ToTimeSpan();
            return ts.ToString("hh':'mm");
        }
    }

    public TimeSpan ToTimeSpan()
    {
        return TimeSpan.FromMinutes(_value);
    }

    public static KMinute FromTimeSpan(TimeSpan ts)
    {
        return new KMinute((int)ts.TotalMinutes);
    }
}

public readonly struct KSecond : IEquatable<KSecond>, IComparable<KSecond>, IComparable, INullable
{
    private readonly int _value;

    public KSecond(int value)
    {
        _value = value;
    }

    public static readonly KSecond Zero = new KSecond();
    public static readonly KSecond Infinity = new KSecond(int.MaxValue);
    public static readonly KSecond NegativeInfinity = new KSecond(-Infinity._value);
    public static readonly KSecond Null = new KSecond(int.MinValue);

    public int Value => _value;

    public bool IsNull => Equals(Null);

    #region Equality

    public bool Equals(KSecond other) => _value == other._value;

    public override bool Equals(object? obj) => obj is KSecond s && Equals(s);

    public override int GetHashCode() => _value.GetHashCode();

    public static bool Equals(KSecond left, KSecond right) => left.Equals(right);

    public static bool operator ==(KSecond left, KSecond right) => left.Equals(right);

    public static bool operator !=(KSecond left, KSecond right) => !(left == right);

    #endregion

    #region Comparison

    public int CompareTo(KSecond other) => _value.CompareTo(other._value);

    public int CompareTo(object? obj)
    {
        if (obj is KSecond s)
        {
            return CompareTo(s);
        }
        else
        {
            throw new ArgumentException("Object is not a KSecond.");
        }
    }

    public static int Compare(KSecond left, KSecond right) => left.CompareTo(right);

    public static int CompareTo(KSecond left, KSecond right) => left.CompareTo(right);

    public static bool operator >(KSecond left, KSecond right) => left.CompareTo(right) > 0;

    public static bool operator <(KSecond left, KSecond right) => left.CompareTo(right) < 0;

    public static bool operator >=(KSecond left, KSecond right) => left.CompareTo(right) >= 0;

    public static bool operator <=(KSecond left, KSecond right) => left.CompareTo(right) <= 0;

    #endregion

    public override string ToString()
    {
        if (Equals(Null))
        {
            return "0Nv";
        }
        else if (Equals(Infinity))
        {
            return "0Wv";
        }
        else if (Equals(NegativeInfinity))
        {
            return "-0Wv";
        }
        else
        {
            var ts = ToTimeSpan();
            return ts.ToString("hh':'mm':'ss");
        }
    }

    public TimeSpan ToTimeSpan()
    {
        return TimeSpan.FromSeconds(_value);
    }

    public static KSecond FromTimeSpan(TimeSpan ts)
    {
        return new KSecond((int)ts.TotalSeconds);
    }
}

public readonly struct KTime : IEquatable<KTime>, IComparable<KTime>, IComparable, INullable
{
    private readonly int _value;

    public KTime(int value)
    {
        _value = value;
    }

    public static readonly KTime Zero = new KTime();
    public static readonly KTime Infinity = new KTime(int.MaxValue);
    public static readonly KTime NegativeInfinity = new KTime(-Infinity._value);
    public static readonly KTime Null = new KTime(int.MinValue);

    public int Value => _value;

    public bool IsNull => Equals(Null);

    #region Equality

    public bool Equals(KTime other) => _value == other._value;

    public override bool Equals(object? obj) => obj is KTime t && Equals(t);

    public override int GetHashCode() => _value.GetHashCode();

    public static bool Equals(KTime left, KTime right) => left.Equals(right);

    public static bool operator ==(KTime left, KTime right) => left.Equals(right);

    public static bool operator !=(KTime left, KTime right) => !(left == right);

    #endregion

    #region Comparison

    public int CompareTo(KTime other) => _value.CompareTo(other._value);

    public int CompareTo(object? obj)
    {
        if (obj is KTime t)
        {
            return CompareTo(t);
        }
        else
        {
            throw new ArgumentException("Object is not a KTime.");
        }
    }

    public static int Compare(KTime left, KTime right) => left.CompareTo(right);

    public static int CompareTo(KTime left, KTime right) => left.CompareTo(right);

    public static bool operator >(KTime left, KTime right) => left.CompareTo(right) > 0;

    public static bool operator <(KTime left, KTime right) => left.CompareTo(right) < 0;

    public static bool operator >=(KTime left, KTime right) => left.CompareTo(right) >= 0;

    public static bool operator <=(KTime left, KTime right) => left.CompareTo(right) <= 0;

    #endregion

    public override string ToString()
    {
        if (Equals(Null))
        {
            return "0Nt";
        }
        else if (Equals(Infinity))
        {
            return "0Wt";
        }
        else if (Equals(NegativeInfinity))
        {
            return "-0Wt";
        }
        else
        {
            var ts = ToTimeSpan();
            return ts.ToString("hh':'mm':'ss");
        }
    }

    public TimeSpan ToTimeSpan()
    {
        return TimeSpan.FromMilliseconds(_value);
    }

    public static KTime FromTimeSpan(TimeSpan ts)
    {
        return new KTime((int)ts.TotalMilliseconds);
    }
}
