/*
 * Auto-generated test cases for KdbSharp deserialization
 * Generated from alltype namespace variables
 */

using System;
using System.IO;
using Xunit;
using KdbSharp;
using KdbSharp.Serialization;
using KdbSharp.Types;

namespace KdbSharp.Test;

public class DeserializeTest
{
    private static readonly TestDataReader _testDataReader = new();

    // 基础类型数组
    private static readonly short[] ExpectedShortArray = [0, short.MinValue, short.MaxValue, (short)-short.MaxValue, (short)32766, (short)-32766];
    private static readonly int[] ExpectedIntArray = [0, int.MinValue, int.MaxValue, -int.MaxValue, 2147483646, -2147483646];
    private static readonly long[] ExpectedLongArray = [0L, long.MinValue, long.MaxValue, -long.MaxValue, 9223372036854775806L, -9223372036854775806L];
    private static readonly float[] ExpectedFloatArray = [0f, float.NaN, float.PositiveInfinity, float.NegativeInfinity, 114514.1919810f, -114514.1919810f];
    private static readonly double[] ExpectedDoubleArray = [0.0, double.NaN, double.PositiveInfinity, double.NegativeInfinity, 114514.1919810, -114514.1919810];

    // 日期时间类型数组（由基础类型数组生成）
    private static readonly KTimestamp[] ExpectedTimestampArray = ExpectedLongArray.Select(x => new KTimestamp(x)).ToArray();
    private static readonly KMonth[] ExpectedMonthArray = ExpectedIntArray.Select(x => new KMonth(x)).ToArray();
    private static readonly KDate[] ExpectedDateArray = ExpectedIntArray.Select(x => new KDate(x)).ToArray();
    private static readonly KDateTime[] ExpectedDateTimeArray = ExpectedDoubleArray.Select(x => new KDateTime(x)).ToArray();
    private static readonly KTimeSpan[] ExpectedTimeSpanArray = ExpectedLongArray.Select(x => new KTimeSpan(x)).ToArray();
    private static readonly KMinute[] ExpectedMinuteArray = ExpectedIntArray.Select(x => new KMinute(x)).ToArray();
    private static readonly KSecond[] ExpectedSecondArray = ExpectedIntArray.Select(x => new KSecond(x)).ToArray();
    private static readonly KTime[] ExpectedTimeArray = ExpectedIntArray.Select(x => new KTime(x)).ToArray();

    [Fact]
    public void Test_boolean()
    {
        // Test deserialization of alltype.boolean
        var data = _testDataReader.GetTestData("boolean");

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<bool[]>(ref reader);
        Assert.Equal(new bool[] { false, true }, result);
    }

    [Fact]
    public void Test_guid()
    {
        // Test deserialization of alltype.guid
        var data = _testDataReader.GetTestData("guid");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<Guid[]>(ref reader);
        Assert.Equal(new Guid[] { Guid.Empty, Guid.Parse("11223344-5566-7788-99aa-bbccddeeffaa") }, result);
    }

    [Fact]
    public void Test_byte()
    {
        // Test deserialization of alltype.byte
        var data = _testDataReader.GetTestData("byte");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<byte[]>(ref reader);
        Assert.Equal(new byte[] { 0x00, 0xff }, result);
    }

    [Fact]
    public void Test_char()
    {
        // Test deserialization of alltype.char
        var data = _testDataReader.GetTestData("char");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<char[]>(ref reader);
        Assert.Equal(new char[] { ' ', 'a' }, result);
    }

    [Fact]
    public void Test_symbol()
    {
        // Test deserialization of alltype.symbol
        var data = _testDataReader.GetTestData("symbol");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<string[]>(ref reader);
        Assert.Equal(new string[] { string.Empty, "a" }, result);
    }

    [Fact]
    public void Test_short()
    {
        // Test deserialization of alltype.short
        var data = _testDataReader.GetTestData("short");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<short[]>(ref reader);
        Assert.Equal(ExpectedShortArray, result);
    }

    [Fact]
    public void Test_int()
    {
        // Test deserialization of alltype.int
        var data = _testDataReader.GetTestData("int");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<int[]>(ref reader);
        Assert.Equal(ExpectedIntArray, result);
    }

    [Fact]
    public void Test_long()
    {
        // Test deserialization of alltype.long
        var data = _testDataReader.GetTestData("long");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<long[]>(ref reader);
        Assert.Equal(ExpectedLongArray, result);
    }

    [Fact]
    public void Test_real()
    {
        // Test deserialization of alltype.real
        var data = _testDataReader.GetTestData("real");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<float[]>(ref reader);
        Assert.Equal(ExpectedFloatArray, result);
    }

    [Fact]
    public void Test_float()
    {
        // Test deserialization of alltype.float
        var data = _testDataReader.GetTestData("float");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<double[]>(ref reader);
        Assert.Equal(ExpectedDoubleArray, result);
    }

    [Fact]
    public void Test_timestamp()
    {
        // Test deserialization of alltype.timestamp
        var data = _testDataReader.GetTestData("timestamp");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KTimestamp[]>(ref reader);
        Assert.Equal(ExpectedTimestampArray, result);
    }

    [Fact]
    public void Test_month()
    {
        // Test deserialization of alltype.month
        var data = _testDataReader.GetTestData("month");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KMonth[]>(ref reader);
        Assert.Equal(ExpectedMonthArray, result);
    }

    [Fact]
    public void Test_date()
    {
        // Test deserialization of alltype.date
        var data = _testDataReader.GetTestData("date");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KDate[]>(ref reader);
        Assert.Equal(ExpectedDateArray, result);
    }

    [Fact]
    public void Test_datetime()
    {
        // Test deserialization of alltype.datetime
        var data = _testDataReader.GetTestData("datetime");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KDateTime[]>(ref reader);
        Assert.Equal(ExpectedDateTimeArray, result);
    }

    [Fact]
    public void Test_timespan()
    {
        // Test deserialization of alltype.timespan
        var data = _testDataReader.GetTestData("timespan");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KTimeSpan[]>(ref reader);
        Assert.Equal(ExpectedTimeSpanArray, result);
    }

    [Fact]
    public void Test_minute()
    {
        // Test deserialization of alltype.minute
        var data = _testDataReader.GetTestData("minute");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KMinute[]>(ref reader);
        Assert.Equal(ExpectedMinuteArray, result);
    }

    [Fact]
    public void Test_second()
    {
        // Test deserialization of alltype.second
        var data = _testDataReader.GetTestData("second");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KSecond[]>(ref reader);
        Assert.Equal(ExpectedSecondArray, result);
    }

    [Fact]
    public void Test_time()
    {
        // Test deserialization of alltype.time
        var data = _testDataReader.GetTestData("time");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KTime[]>(ref reader);
        Assert.Equal(ExpectedTimeArray, result);
    }

    [Fact]
    public void Test_boolean_false()
    {
        // Test deserialization of alltype.boolean_false
        var data = _testDataReader.GetTestData("boolean_false");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<bool>(ref reader);
        Assert.False(result);
    }

    [Fact]
    public void Test_boolean_true()
    {
        // Test deserialization of alltype.boolean_true
        var data = _testDataReader.GetTestData("boolean_true");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<bool>(ref reader);
        Assert.True(result);
    }

    [Fact]
    public void Test_guid_null()
    {
        // Test deserialization of alltype.guid_null
        var data = _testDataReader.GetTestData("guid_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<Guid>(ref reader);
        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public void Test_guid_value()
    {
        // Test deserialization of alltype.guid_value
        var data = _testDataReader.GetTestData("guid_value");

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<Guid>(ref reader);
        Assert.Equal(Guid.Parse("11223344-5566-7788-99aa-bbccddeeffaa"), result);
    }

    [Fact]
    public void Test_byte_zero()
    {
        // Test deserialization of alltype.byte_zero
        var data = _testDataReader.GetTestData("byte_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<byte>(ref reader);
        Assert.Equal((byte)0x00, result);
    }

    [Fact]
    public void Test_byte_max()
    {
        // Test deserialization of alltype.byte_max
        var data = _testDataReader.GetTestData("byte_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<byte>(ref reader);
        Assert.Equal((byte)0xff, result);
    }

    [Fact]
    public void Test_char_null()
    {
        // Test deserialization of alltype.char_null
        var data = _testDataReader.GetTestData("char_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<char>(ref reader);
        Assert.Equal(' ', result); // space char
    }

    [Fact]
    public void Test_char_value()
    {
        // Test deserialization of alltype.char_value
        var data = _testDataReader.GetTestData("char_value");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<char>(ref reader);
        Assert.Equal('a', result);
    }

    [Fact]
    public void Test_symbol_null()
    {
        // Test deserialization of alltype.symbol_null
        var data = _testDataReader.GetTestData("symbol_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<string>(ref reader);
        Assert.Equal(string.Empty, result); // null symbol
    }

    [Fact]
    public void Test_symbol_value()
    {
        // Test deserialization of alltype.symbol_value
        var data = _testDataReader.GetTestData("symbol_value");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<string>(ref reader);
        Assert.Equal("a", result);
    }

    [Fact]
    public void Test_short_zero()
    {
        // Test deserialization of alltype.short_zero
        var data = _testDataReader.GetTestData("short_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<short>(ref reader);
        Assert.Equal((short)0, result);
    }

    [Fact]
    public void Test_short_null()
    {
        // Test deserialization of alltype.short_null
        var data = _testDataReader.GetTestData("short_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<short>(ref reader);
        Assert.Equal(short.MinValue, result);
    }

    [Fact]
    public void Test_short_inf()
    {
        // Test deserialization of alltype.short_inf
        var data = _testDataReader.GetTestData("short_inf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<short>(ref reader);
        Assert.Equal(short.MaxValue, result);
    }

    [Fact]
    public void Test_short_ninf()
    {
        // Test deserialization of alltype.short_ninf
        var data = _testDataReader.GetTestData("short_ninf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<short>(ref reader);
        Assert.Equal(-short.MaxValue, result);
    }

    [Fact]
    public void Test_short_max()
    {
        // Test deserialization of alltype.short_max
        var data = _testDataReader.GetTestData("short_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<short>(ref reader);
        Assert.Equal((short)32766, result);
    }

    [Fact]
    public void Test_short_min()
    {
        // Test deserialization of alltype.short_min
        var data = _testDataReader.GetTestData("short_min");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<short>(ref reader);
        Assert.Equal((short)-32766, result);
    }

    [Fact]
    public void Test_int_zero()
    {
        // Test deserialization of alltype.int_zero
        var data = _testDataReader.GetTestData("int_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<int>(ref reader);
        Assert.Equal(0, result);
    }

    [Fact]
    public void Test_int_null()
    {
        // Test deserialization of alltype.int_null
        var data = _testDataReader.GetTestData("int_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<int>(ref reader);
        Assert.Equal(int.MinValue, result);
    }

    [Fact]
    public void Test_int_inf()
    {
        // Test deserialization of alltype.int_inf
        var data = _testDataReader.GetTestData("int_inf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<int>(ref reader);
        Assert.Equal(int.MaxValue, result);
    }

    [Fact]
    public void Test_int_ninf()
    {
        // Test deserialization of alltype.int_ninf
        var data = _testDataReader.GetTestData("int_ninf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<int>(ref reader);
        Assert.Equal(-int.MaxValue, result);
    }

    [Fact]
    public void Test_int_max()
    {
        // Test deserialization of alltype.int_max
        var data = _testDataReader.GetTestData("int_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<int>(ref reader);
        Assert.Equal(2147483646, result);
    }

    [Fact]
    public void Test_int_min()
    {
        // Test deserialization of alltype.int_min
        var data = _testDataReader.GetTestData("int_min");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<int>(ref reader);
        Assert.Equal(-2147483646, result);
    }

    [Fact]
    public void Test_long_zero()
    {
        // Test deserialization of alltype.long_zero
        var data = _testDataReader.GetTestData("long_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<long>(ref reader);
        Assert.Equal(0L, result);
    }

    [Fact]
    public void Test_long_null()
    {
        // Test deserialization of alltype.long_null
        var data = _testDataReader.GetTestData("long_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<long>(ref reader);
        Assert.Equal(long.MinValue, result);
    }

    [Fact]
    public void Test_long_inf()
    {
        // Test deserialization of alltype.long_inf
        var data = _testDataReader.GetTestData("long_inf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<long>(ref reader);
        Assert.Equal(long.MaxValue, result);
    }

    [Fact]
    public void Test_long_ninf()
    {
        // Test deserialization of alltype.long_ninf
        var data = _testDataReader.GetTestData("long_ninf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<long>(ref reader);
        Assert.Equal(-long.MaxValue, result);
    }

    [Fact]
    public void Test_long_max()
    {
        // Test deserialization of alltype.long_max
        var data = _testDataReader.GetTestData("long_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<long>(ref reader);
        Assert.Equal(9223372036854775806L, result);
    }

    [Fact]
    public void Test_long_min()
    {
        // Test deserialization of alltype.long_min
        var data = _testDataReader.GetTestData("long_min");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<long>(ref reader);
        Assert.Equal(-9223372036854775806L, result);
    }

    [Fact]
    public void Test_real_zero()
    {
        // Test deserialization of alltype.real_zero
        var data = _testDataReader.GetTestData("real_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<float>(ref reader);
        Assert.Equal(0f, result);
    }

    [Fact]
    public void Test_real_null()
    {
        // Test deserialization of alltype.real_null
        var data = _testDataReader.GetTestData("real_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<float>(ref reader);
        Assert.Equal(float.NaN, result);
    }

    [Fact]
    public void Test_real_inf()
    {
        // Test deserialization of alltype.real_inf
        var data = _testDataReader.GetTestData("real_inf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<float>(ref reader);
        Assert.Equal(float.PositiveInfinity, result);
    }

    [Fact]
    public void Test_real_ninf()
    {
        // Test deserialization of alltype.real_ninf
        var data = _testDataReader.GetTestData("real_ninf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<float>(ref reader);
        Assert.Equal(float.NegativeInfinity, result);
    }

    [Fact]
    public void Test_real_max()
    {
        // Test deserialization of alltype.real_max
        var data = _testDataReader.GetTestData("real_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<float>(ref reader);
        Assert.Equal(114514.1919810f, result);
    }

    [Fact]
    public void Test_real_min()
    {
        // Test deserialization of alltype.real_min
        var data = _testDataReader.GetTestData("real_min");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<float>(ref reader);
        Assert.Equal(-114514.1919810f, result);
    }

    [Fact]
    public void Test_float_zero()
    {
        // Test deserialization of alltype.float_zero
        var data = _testDataReader.GetTestData("float_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<double>(ref reader);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Test_float_null()
    {
        // Test deserialization of alltype.float_null
        var data = _testDataReader.GetTestData("float_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<double>(ref reader);
        Assert.Equal(double.NaN, result);
    }

    [Fact]
    public void Test_float_inf()
    {
        // Test deserialization of alltype.float_inf
        var data = _testDataReader.GetTestData("float_inf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<double>(ref reader);
        Assert.Equal(double.PositiveInfinity, result);
    }

    [Fact]
    public void Test_float_ninf()
    {
        // Test deserialization of alltype.float_ninf
        var data = _testDataReader.GetTestData("float_ninf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<double>(ref reader);
        Assert.Equal(double.NegativeInfinity, result);
    }

    [Fact]
    public void Test_float_max()
    {
        // Test deserialization of alltype.float_max
        var data = _testDataReader.GetTestData("float_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<double>(ref reader);
        Assert.Equal(114514.1919810, result);
    }

    [Fact]
    public void Test_float_min()
    {
        // Test deserialization of alltype.float_min
        var data = _testDataReader.GetTestData("float_min");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<double>(ref reader);
        Assert.Equal(-114514.1919810, result);
    }

    [Fact]
    public void Test_timestamp_zero()
    {
        // Test deserialization of alltype.timestamp_zero
        var data = _testDataReader.GetTestData("timestamp_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KTimestamp>(ref reader);
        Assert.Equal(KTimestamp.Zero, result);
    }

    [Fact]
    public void Test_timestamp_null()
    {
        // Test deserialization of alltype.timestamp_null
        var data = _testDataReader.GetTestData("timestamp_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KTimestamp>(ref reader);
        Assert.Equal(KTimestamp.Null, result);
    }

    [Fact]
    public void Test_timestamp_inf()
    {
        // Test deserialization of alltype.timestamp_inf
        var data = _testDataReader.GetTestData("timestamp_inf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KTimestamp>(ref reader);
        Assert.Equal(KTimestamp.Infinity, result);
    }

    [Fact]
    public void Test_timestamp_ninf()
    {
        // Test deserialization of alltype.timestamp_ninf
        var data = _testDataReader.GetTestData("timestamp_ninf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KTimestamp>(ref reader);
        Assert.Equal(KTimestamp.NegativeInfinity, result);
    }

    [Fact]
    public void Test_timestamp_max()
    {
        // Test deserialization of alltype.timestamp_max
        var data = _testDataReader.GetTestData("timestamp_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KTimestamp>(ref reader);
        Assert.Equal(new KTimestamp(long.MaxValue-1), result);
    }

    [Fact]
    public void Test_timestamp_min()
    {
        // Test deserialization of alltype.timestamp_min
        var data = _testDataReader.GetTestData("timestamp_min");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<KTimestamp>(ref reader);
        Assert.Equal(new KTimestamp(long.MinValue + 2), result);
    }

    [Fact]
    public void Test_month_zero()
    {
        // Test deserialization of alltype.month_zero
        var data = _testDataReader.GetTestData("month_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_month_null()
    {
        // Test deserialization of alltype.month_null
        var data = _testDataReader.GetTestData("month_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_month_inf()
    {
        // Test deserialization of alltype.month_inf
        var data = _testDataReader.GetTestData("month_inf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_month_ninf()
    {
        // Test deserialization of alltype.month_ninf
        var data = _testDataReader.GetTestData("month_ninf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_month_max()
    {
        // Test deserialization of alltype.month_max
        var data = _testDataReader.GetTestData("month_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_month_min()
    {
        // Test deserialization of alltype.month_min
        var data = _testDataReader.GetTestData("month_min");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_date_zero()
    {
        // Test deserialization of alltype.date_zero
        var data = _testDataReader.GetTestData("date_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_date_null()
    {
        // Test deserialization of alltype.date_null
        var data = _testDataReader.GetTestData("date_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_date_inf()
    {
        // Test deserialization of alltype.date_inf
        var data = _testDataReader.GetTestData("date_inf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_date_ninf()
    {
        // Test deserialization of alltype.date_ninf
        var data = _testDataReader.GetTestData("date_ninf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_date_max()
    {
        // Test deserialization of alltype.date_max
        var data = _testDataReader.GetTestData("date_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_date_min()
    {
        // Test deserialization of alltype.date_min
        var data = _testDataReader.GetTestData("date_min");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_datetime_zero()
    {
        // Test deserialization of alltype.datetime_zero
        var data = _testDataReader.GetTestData("datetime_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_datetime_null()
    {
        // Test deserialization of alltype.datetime_null
        var data = _testDataReader.GetTestData("datetime_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_datetime_inf()
    {
        // Test deserialization of alltype.datetime_inf
        var data = _testDataReader.GetTestData("datetime_inf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_datetime_ninf()
    {
        // Test deserialization of alltype.datetime_ninf
        var data = _testDataReader.GetTestData("datetime_ninf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_datetime_max()
    {
        // Test deserialization of alltype.datetime_max
        var data = _testDataReader.GetTestData("datetime_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_datetime_min()
    {
        // Test deserialization of alltype.datetime_min
        var data = _testDataReader.GetTestData("datetime_min");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_timespan_zero()
    {
        // Test deserialization of alltype.timespan_zero
        var data = _testDataReader.GetTestData("timespan_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_timespan_null()
    {
        // Test deserialization of alltype.timespan_null
        var data = _testDataReader.GetTestData("timespan_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_timespan_inf()
    {
        // Test deserialization of alltype.timespan_inf
        var data = _testDataReader.GetTestData("timespan_inf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_timespan_ninf()
    {
        // Test deserialization of alltype.timespan_ninf
        var data = _testDataReader.GetTestData("timespan_ninf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_timespan_max()
    {
        // Test deserialization of alltype.timespan_max
        var data = _testDataReader.GetTestData("timespan_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_timespan_min()
    {
        // Test deserialization of alltype.timespan_min
        var data = _testDataReader.GetTestData("timespan_min");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_minute_zero()
    {
        // Test deserialization of alltype.minute_zero
        var data = _testDataReader.GetTestData("minute_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_minute_null()
    {
        // Test deserialization of alltype.minute_null
        var data = _testDataReader.GetTestData("minute_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_minute_inf()
    {
        // Test deserialization of alltype.minute_inf
        var data = _testDataReader.GetTestData("minute_inf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_minute_ninf()
    {
        // Test deserialization of alltype.minute_ninf
        var data = _testDataReader.GetTestData("minute_ninf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_minute_max()
    {
        // Test deserialization of alltype.minute_max
        var data = _testDataReader.GetTestData("minute_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_minute_min()
    {
        // Test deserialization of alltype.minute_min
        var data = _testDataReader.GetTestData("minute_min");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_second_zero()
    {
        // Test deserialization of alltype.second_zero
        var data = _testDataReader.GetTestData("second_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_second_null()
    {
        // Test deserialization of alltype.second_null
        var data = _testDataReader.GetTestData("second_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_second_inf()
    {
        // Test deserialization of alltype.second_inf
        var data = _testDataReader.GetTestData("second_inf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_second_ninf()
    {
        // Test deserialization of alltype.second_ninf
        var data = _testDataReader.GetTestData("second_ninf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_second_max()
    {
        // Test deserialization of alltype.second_max
        var data = _testDataReader.GetTestData("second_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_second_min()
    {
        // Test deserialization of alltype.second_min
        var data = _testDataReader.GetTestData("second_min");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_time_zero()
    {
        // Test deserialization of alltype.time_zero
        var data = _testDataReader.GetTestData("time_zero");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_time_null()
    {
        // Test deserialization of alltype.time_null
        var data = _testDataReader.GetTestData("time_null");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_time_inf()
    {
        // Test deserialization of alltype.time_inf
        var data = _testDataReader.GetTestData("time_inf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_time_ninf()
    {
        // Test deserialization of alltype.time_ninf
        var data = _testDataReader.GetTestData("time_ninf");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_time_max()
    {
        // Test deserialization of alltype.time_max
        var data = _testDataReader.GetTestData("time_max");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_time_min()
    {
        // Test deserialization of alltype.time_min
        var data = _testDataReader.GetTestData("time_min");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        // TODO: Add specific deserialization and validation logic
        // var reader = new KSerializationReader(data);
        // var result = reader.ReadXXX();
        // Assert.Equal(expectedValue, result);
    }

}

/// <summary>
/// Helper class to read test data from alltype.txt file
/// </summary>
public class TestDataReader
{
    private static readonly Dictionary<string, byte[]> _testData = LoadTestData();

    public byte[] GetTestData(string variableName)
    {
        return _testData.TryGetValue(variableName, out var data) ? data : Array.Empty<byte>();
    }

    private static Dictionary<string, byte[]> LoadTestData()
    {
        var result = new Dictionary<string, byte[]>();
        var lines = File.ReadAllLines("alltype.txt");

        string? currentVariable = null;
        var currentBytes = new List<byte>();

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (trimmedLine.EndsWith(": ["))
            {
                // Start of new variable
                currentVariable = trimmedLine.Substring(0, trimmedLine.Length - 3);
                currentBytes.Clear();
            }
            else if (trimmedLine == "];")
            {
                // End of current variable
                if (currentVariable != null)
                {
                    result[currentVariable] = currentBytes.ToArray();
                    currentVariable = null;
                }
            }
            else if (!string.IsNullOrEmpty(trimmedLine) && !trimmedLine.StartsWith("//"))
            {
                // Hex data line
                var hexValues = trimmedLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var hex in hexValues)
                {
                    if (byte.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out var b))
                    {
                        currentBytes.Add(b);
                    }
                }
            }
        }

        return result;
    }
}
