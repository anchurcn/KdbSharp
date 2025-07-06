using Xunit;

namespace KdbSharp.Test;

/*
// Atom.
// boolean.
alltype.boolean_false: 0b;
alltype.boolean_true: 1b;
// guid.
alltype.guid_null: 0Ng;
alltype.guid_value: "G"$"11223344-5566-7788-99aa-bbccddeeffaa";
// byte.
alltype.byte_zero: 0x00;
alltype.byte_max: 0xff;
// char.
alltype.char_null: " ";
alltype.char_value: "a";
// symbol.
alltype.symbol_null: `;
alltype.symbol_value: `a;
// short.
alltype.short_zero: 0h;
alltype.short_null: 0Nh;
alltype.short_inf: 0Wh;
alltype.short_ninf: -0Wh;
alltype.short_max: 32766h;
alltype.short_min: -32766h;
// int.
alltype.int_zero: 0i;
alltype.int_null: 0Ni;
alltype.int_inf: 0Wi;
alltype.int_ninf: -0Wi;
alltype.int_max: 2147483646;
alltype.int_min: -2147483646;
// long.
alltype.long_zero: 0j;
alltype.long_null: 0Nj;
alltype.long_inf: 0Wj;
alltype.long_ninf: -0Wj;
alltype.long_max: 9223372036854775806;
alltype.long_min: -9223372036854775806;
// real.
alltype.real_zero: 0e;
alltype.real_null: 0Ne;
alltype.real_inf: 0We;
alltype.real_ninf: -0We;
alltype.real_max: 114514.1919810e;
alltype.real_min: -114514.1919810e;
// float.
alltype.float_zero: 0f;
alltype.float_null: 0Nf;
alltype.float_inf: 0Wf;
alltype.float_ninf: -0Wf;
alltype.float_max: 114514.1919810f;
alltype.float_min: -114514.1919810f;
 */
public class AtomTypeDeserializationTest
{
    public const string Host = "localhost";
    public const int Port = 5011;
    public const string Username = "username";
    public const string Password = "password";

    public const int MaxIntNeg1 = int.MaxValue - 1;
    public const long MaxLongNeg1 = long.MaxValue - 1;
    public const short MaxShortNeg1 = short.MaxValue - 1;

    public AtomTypeDeserializationTest()
    {
        this.Connection = new KdbConnection(new KdbConnectionOptions
        {
            Host = Host,
            Port = Port,
        });
        this.Connection.OpenAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public KdbConnection Connection { get; }

    [Fact]
    public async Task TestDeserializeBooleanFalseAsync()
    {
        var result = await this.Connection.GetAsync<bool>("alltype.boolean_false").ConfigureAwait(false);
        Assert.False(result);
    }

    [Fact]
    public async Task TestDeserializeBooleanTrueAsync()
    {
        var result = await this.Connection.GetAsync<bool>("alltype.boolean_true").ConfigureAwait(false);
        Assert.True(result);
    }

    [Fact]
    public async Task TestDeserializeGuidNullAsync()
    {
        var result = await this.Connection.GetAsync<Guid>("alltype.guid_null").ConfigureAwait(false);
        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public async Task TestDeserializeGuidAsync()
    {
        var result = await this.Connection.GetAsync<Guid>("alltype.guid_value").ConfigureAwait(false);
        Assert.Equal(Guid.Parse("11223344-5566-7788-99aa-bbccddeeffaa"), result);
    }

    [Fact]
    public async Task TestDeserializeByteZeroAsync()
    {
        var result = await this.Connection.GetAsync<byte>("alltype.byte_zero").ConfigureAwait(false);
        Assert.Equal(byte.MinValue, result);
    }

    [Fact]
    public async Task TestDeserializeByteMaxAsync()
    {
        var result = await this.Connection.GetAsync<byte>("alltype.byte_max").ConfigureAwait(false);
        Assert.Equal(byte.MaxValue, result);
    }

    [Fact]
    public async Task TestDeserializeCharNullAsync()
    {
        var result = await this.Connection.GetAsync<char>("alltype.char_null").ConfigureAwait(false);
        Assert.Equal(' ', result);
    }

    [Fact]
    public async Task TestDeserializeCharAsync()
    {
        var result = await this.Connection.GetAsync<char>("alltype.char_value").ConfigureAwait(false);
        Assert.Equal('a', result);
    }

    [Fact]
    public async Task TestDeserializeSymbolNullAsync()
    {
        var result = await this.Connection.GetAsync<string>("alltype.symbol_null").ConfigureAwait(false);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public async Task TestDeserializeSymbolAsync()
    {
        var result = await this.Connection.GetAsync<string>("alltype.symbol_value").ConfigureAwait(false);
        Assert.Equal("a", result);
    }

    [Fact]
    public async Task TestDeserializeShortZeroAsync()
    {
        var result = await this.Connection.GetAsync<short>("alltype.short_zero").ConfigureAwait(false);
        Assert.Equal(default, result);
    }

    [Fact]
    public async Task TestDeserializeShortNullAsync()
    {
        var result = await this.Connection.GetAsync<short>("alltype.short_null").ConfigureAwait(false);
        Assert.Equal(short.MinValue, result);
    }

    [Fact]
    public async Task TestDeserializeShortInfAsync()
    {
        var result = await this.Connection.GetAsync<short>("alltype.short_inf").ConfigureAwait(false);
        Assert.Equal(short.MaxValue, result);
    }

    [Fact]
    public async Task TestDeserializeShortNInfAsync()
    {
        var result = await this.Connection.GetAsync<short>("alltype.short_ninf").ConfigureAwait(false);
        Assert.Equal(-short.MaxValue, result);
    }

    [Fact]
    public async Task TestDeserializeShortMaxAsync()
    {
        var result = await this.Connection.GetAsync<short>("alltype.short_max").ConfigureAwait(false);
        Assert.Equal(MaxShortNeg1, result);
    }

    [Fact]
    public async Task TestDeserializeShortMinAsync()
    {
        var result = await this.Connection.GetAsync<short>("alltype.short_min").ConfigureAwait(false);
        Assert.Equal(-MaxShortNeg1, result);
    }

    [Fact]
    public async Task TestDeserializeIntZeroAsync()
    {
        var result = await this.Connection.GetAsync<int>("alltype.int_zero").ConfigureAwait(false);
        Assert.Equal(default, result);
    }

    [Fact]
    public async Task TestDeserializeIntNullAsync()
    {
        var result = await this.Connection.GetAsync<int>("alltype.int_null").ConfigureAwait(false);
        Assert.Equal(int.MinValue, result);
    }

    [Fact]
    public async Task TestDeserializeIntInfAsync()
    {
        var result = await this.Connection.GetAsync<int>("alltype.int_inf").ConfigureAwait(false);
        Assert.Equal(int.MaxValue, result);
    }

    [Fact]
    public async Task TestDeserializeIntNInfAsync()
    {
        var result = await this.Connection.GetAsync<int>("alltype.int_ninf").ConfigureAwait(false);
        Assert.Equal(-int.MaxValue, result);
    }

    [Fact]
    public async Task TestDeserializeIntMaxAsync()
    {
        var result = await this.Connection.GetAsync<int>("alltype.int_max").ConfigureAwait(false);
        Assert.Equal(MaxIntNeg1, result);
    }

    [Fact]
    public async Task TestDeserializeIntMinAsync()
    {
        var result = await this.Connection.GetAsync<int>("alltype.int_min").ConfigureAwait(false);
        Assert.Equal(-MaxIntNeg1, result);
    }

    [Fact]
    public async Task TestDeserializeLongZeroAsync()
    {
        var result = await this.Connection.GetAsync<long>("alltype.long_zero").ConfigureAwait(false);
        Assert.Equal(default, result);
    }

    [Fact]
    public async Task TestDeserializeLongNullAsync()
    {
        var result = await this.Connection.GetAsync<long>("alltype.long_null").ConfigureAwait(false);
        Assert.Equal(long.MinValue, result);
    }

    [Fact]
    public async Task TestDeserializeLongInfAsync()
    {
        var result = await this.Connection.GetAsync<long>("alltype.long_inf").ConfigureAwait(false);
        Assert.Equal(long.MaxValue, result);
    }

    [Fact]
    public async Task TestDeserializeLongNInfAsync()
    {
        var result = await this.Connection.GetAsync<long>("alltype.long_ninf").ConfigureAwait(false);
        Assert.Equal(-long.MaxValue, result);
    }

    [Fact]
    public async Task TestDeserializeLongMaxAsync()
    {
        var result = await this.Connection.GetAsync<long>("alltype.long_max").ConfigureAwait(false);
        Assert.Equal(MaxLongNeg1, result);
    }

    [Fact]
    public async Task TestDeserializeLongMinAsync()
    {
        var result = await this.Connection.GetAsync<long>("alltype.long_min").ConfigureAwait(false);
        Assert.Equal(-MaxLongNeg1, result);
    }

    [Fact]
    public async Task TestDeserializeRealZeroAsync()
    {
        var result = await this.Connection.GetAsync<float>("alltype.real_zero").ConfigureAwait(false);
        Assert.Equal(default, result);
    }

    [Fact]
    public async Task TestDeserializeRealNullAsync()
    {
        var result = await this.Connection.GetAsync<float>("alltype.real_null").ConfigureAwait(false);
        Assert.Equal(float.NaN, result);
    }

    [Fact]
    public async Task TestDeserializeRealInfAsync()
    {
        var result = await this.Connection.GetAsync<float>("alltype.real_inf").ConfigureAwait(false);
        Assert.Equal(float.PositiveInfinity, result);
    }

    [Fact]
    public async Task TestDeserializeRealNInfAsync()
    {
        var result = await this.Connection.GetAsync<float>("alltype.real_ninf").ConfigureAwait(false);
        Assert.Equal(float.NegativeInfinity, result);
    }

    [Fact]
    public async Task TestDeserializeRealMaxAsync()
    {
        var result = await this.Connection.GetAsync<float>("alltype.real_max").ConfigureAwait(false);
        Assert.Equal(114514.1919810f, result);
    }

    [Fact]
    public async Task TestDeserializeRealMinAsync()
    {
        var result = await this.Connection.GetAsync<float>("alltype.real_min").ConfigureAwait(false);
        Assert.Equal(-114514.1919810f, result);
    }

    [Fact]
    public async Task TestDeserializeFloatZeroAsync()
    {
        var result = await this.Connection.GetAsync<double>("alltype.float_zero").ConfigureAwait(false);
        Assert.Equal(default, result);
    }

    [Fact]
    public async Task TestDeserializeFloatNullAsync()
    {
        var result = await this.Connection.GetAsync<double>("alltype.float_null").ConfigureAwait(false);
        Assert.Equal(double.NaN, result);
    }

    [Fact]
    public async Task TestDeserializeFloatInfAsync()
    {
        var result = await this.Connection.GetAsync<double>("alltype.float_inf").ConfigureAwait(false);
        Assert.Equal(double.PositiveInfinity, result);
    }

    [Fact]
    public async Task TestDeserializeFloatNInfAsync()
    {
        var result = await this.Connection.GetAsync<double>("alltype.float_ninf").ConfigureAwait(false);
        Assert.Equal(double.NegativeInfinity, result);
    }

    [Fact]
    public async Task TestDeserializeFloatMaxAsync()
    {
        var result = await this.Connection.GetAsync<double>("alltype.float_max").ConfigureAwait(false);
        Assert.Equal(114514.1919810, result);
    }

    [Fact]
    public async Task TestDeserializeFloatMinAsync()
    {
        var result = await this.Connection.GetAsync<double>("alltype.float_min").ConfigureAwait(false);
        Assert.Equal(-114514.1919810, result);
    }

    [Fact]
    public async Task TestDeserializeFloatMinToObjectAsync()
    {
        var result = await this.Connection.GetAsync<object>("alltype.float_min").ConfigureAwait(false);
        Assert.Equal(-114514.1919810, result);
    }
}
