/*
 * Test cases for KdbSharp object deserialization
 * Tests all variables from alltype.txt to ensure they can be deserialized to object without exceptions
 */

using System;
using Xunit;
using KdbSharp;
using KdbSharp.Serialization;

namespace KdbSharp.Test;

public class ObjectDeserializationTest
{
    private static readonly TestDataReader _testDataReader = new();

    // Empty variable test
    [Fact]
    public void Test_Empty_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("");
        if (data.Length == 0) return; // Skip if no data

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.True(true);
    }

    // List type tests
    [Fact]
    public void Test_boolean_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("boolean");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_guid_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("guid");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_byte_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("byte");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_char_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("char");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_symbol_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("symbol");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_short_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("short");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_int_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("int");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_long_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("long");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_real_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("real");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_float_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("float");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_timestamp_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timestamp");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_month_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("month");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_date_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("date");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_datetime_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("datetime");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_timespan_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timespan");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_minute_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("minute");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_second_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("second");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_time_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("time");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    // Atom type tests
    [Fact]
    public void Test_boolean_false_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("boolean_false");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_boolean_true_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("boolean_true");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_guid_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("guid_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_guid_value_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("guid_value");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_byte_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("byte_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_byte_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("byte_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_char_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("char_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_char_value_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("char_value");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_symbol_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("symbol_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_symbol_value_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("symbol_value");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        // Assert no exception thrown
        Assert.NotNull(result);
    }

    // Short atom tests
    [Fact]
    public void Test_short_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("short_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_short_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("short_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_short_inf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("short_inf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_short_ninf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("short_ninf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_short_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("short_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_short_min_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("short_min");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    // Int atom tests
    [Fact]
    public void Test_int_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("int_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_int_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("int_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_int_inf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("int_inf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_int_ninf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("int_ninf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_int_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("int_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_int_min_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("int_min");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    // Long atom tests
    [Fact]
    public void Test_long_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("long_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_long_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("long_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_long_inf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("long_inf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_long_ninf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("long_ninf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_long_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("long_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_long_min_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("long_min");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    // Real (float) atom tests
    [Fact]
    public void Test_real_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("real_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_real_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("real_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_real_inf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("real_inf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_real_ninf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("real_ninf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_real_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("real_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_real_min_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("real_min");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    // Float (double) atom tests
    [Fact]
    public void Test_float_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("float_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_float_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("float_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_float_inf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("float_inf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_float_ninf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("float_ninf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_float_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("float_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_float_min_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("float_min");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    // Timestamp atom tests
    [Fact]
    public void Test_timestamp_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timestamp_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_timestamp_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timestamp_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_timestamp_inf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timestamp_inf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_timestamp_ninf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timestamp_ninf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_timestamp_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timestamp_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_timestamp_min_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timestamp_min");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    // Month atom tests
    [Fact]
    public void Test_month_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("month_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_month_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("month_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_month_inf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("month_inf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_month_ninf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("month_ninf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_month_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("month_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_month_min_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("month_min");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    // Date atom tests
    [Fact]
    public void Test_date_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("date_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_date_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("date_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_date_inf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("date_inf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_date_ninf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("date_ninf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_date_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("date_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_date_min_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("date_min");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    // DateTime atom tests
    [Fact]
    public void Test_datetime_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("datetime_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_datetime_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("datetime_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_datetime_inf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("datetime_inf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_datetime_ninf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("datetime_ninf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_datetime_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("datetime_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_datetime_min_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("datetime_min");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    // TimeSpan atom tests
    [Fact]
    public void Test_timespan_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timespan_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_timespan_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timespan_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_timespan_inf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timespan_inf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_timespan_ninf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timespan_ninf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_timespan_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timespan_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_timespan_min_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("timespan_min");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    // Minute atom tests
    [Fact]
    public void Test_minute_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("minute_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_minute_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("minute_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_minute_inf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("minute_inf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_minute_ninf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("minute_ninf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_minute_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("minute_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_minute_min_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("minute_min");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    // Second atom tests
    [Fact]
    public void Test_second_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("second_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_second_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("second_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_second_inf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("second_inf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_second_ninf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("second_ninf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_second_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("second_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_second_min_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("second_min");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    // Time atom tests
    [Fact]
    public void Test_time_zero_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("time_zero");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_time_null_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("time_null");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_time_inf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("time_inf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_time_ninf_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("time_ninf");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_time_max_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("time_max");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_time_min_DeserializeToObject()
    {
        var data = _testDataReader.GetTestData("time_min");
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<object>(ref reader);
        Assert.NotNull(result);
    }
}
