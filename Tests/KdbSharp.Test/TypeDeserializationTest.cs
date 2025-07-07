//using Xunit;

//namespace KdbSharp.Test;

//public class TypeDeserializationTest
//{
//    public const string Host = "localhost";
//    public const int Port = 5011;
//    public const string Username = "username";
//    public const string Password = "password";

//    public TypeDeserializationTest()
//    {
//        this.Connection = new KdbConnection(new KdbConnectionOptions
//        {
//            Host = Host,
//            Port = Port,
//        });
//        this.Connection.OpenAsync().ConfigureAwait(false).GetAwaiter().GetResult();
//    }

//    public KdbConnection Connection { get; }

//    [Fact]
//    public async Task TestDeserializeBooleanListAsync()
//    {
//        var result = await this.Connection.GetAsync<bool[]>("alltype.boolean").ConfigureAwait(false);
//        Assert.Equal([false, true], result);
//    }

//    [Fact]
//    public async Task TestDeserializeByteListAsync()
//    {
//        var result = await this.Connection.GetAsync<byte[]>("alltype.byte").ConfigureAwait(false);
//        Assert.Equal([byte.MinValue, byte.MaxValue], result);
//    }

//    [Fact]
//    public async Task TestDeserializeGuidListAsync()
//    {
//        var result = await this.Connection.GetAsync<Guid[]>("alltype.guid").ConfigureAwait(false);
//        Assert.Equal([Guid.Empty, Guid.Parse("11223344-5566-7788-99aa-bbccddeeffaa")], result);
//    }

//    [Fact]
//    public async Task TestDeserializeCharListAsync()
//    {
//        var result = await this.Connection.GetAsync<char[]>("alltype.char").ConfigureAwait(false);
//        Assert.Equal([' ', 'a'], result);
//    }

//    [Fact]
//    public async Task TestDeserializeSymbolListAsync()
//    {
//        var result = await this.Connection.GetAsync<string[]>("alltype.symbol").ConfigureAwait(false);
//        Assert.Equal([string.Empty, "a"], result);
//    }

//    // Numerics.
//    [Fact]
//    public async Task TestDeserializeShortListAsync()
//    {
//        var result = await this.Connection.GetAsync<short[]>("alltype.short").ConfigureAwait(false);
//        Assert.Equal([default, short.MinValue, short.MaxValue, -short.MaxValue, short.MaxValue - 1, -(short.MaxValue - 1)], result);
//    }

//    [Fact]
//    public async Task TestDeserializeIntListAsync()
//    {
//        var result = await this.Connection.GetAsync<int[]>("alltype.int").ConfigureAwait(false);
//        Assert.Equal([default, int.MinValue, int.MaxValue, -int.MaxValue, int.MaxValue - 1, -(int.MaxValue - 1)], result);
//    }

//    [Fact]
//    public async Task TestDeserializeLongListAsync()
//    {
//        var result = await this.Connection.GetAsync<long[]>("alltype.long").ConfigureAwait(false);
//        Assert.Equal([default, long.MinValue, long.MaxValue, -long.MaxValue, long.MaxValue - 1, -(long.MaxValue - 1)], result);
//    }

//    [Fact]
//    public async Task TestDeserializeRealListAsync()
//    {
//        var result = await this.Connection.GetAsync<float[]>("alltype.real").ConfigureAwait(false);
//        Assert.Equal([default, float.MinValue, float.MaxValue, -float.MaxValue, 114514.1919810f, -114514.1919810f], result);
//    }

//    [Fact]
//    public async Task TestDeserializeFloatListAsync()
//    {
//        var result = await this.Connection.GetAsync<double[]>("alltype.float").ConfigureAwait(false);
//        Assert.Equal([default, double.MinValue, double.MaxValue, -double.MaxValue, 114514.1919810, -114514.1919810], result);
//    }

//    // Temporal.
//    [Fact]
//    public async Task TestDeserializeTimestampListAsync()
//    {
//        var result = await this.Connection.GetAsync<DateTime[]>("alltype.timestamp").ConfigureAwait(false);
//        Assert.Equal(result?.Length, 6);
//    }

//    [Fact]
//    public async Task TestDeserializeMonthListAsync()
//    {
//        var result = await this.Connection.GetAsync<DateTime[]>("alltype.month").ConfigureAwait(false);
//        Assert.Equal(result?.Length, 6);
//    }

//    [Fact]
//    public async Task TestDeserializeDateListAsync()
//    {
//        var result = await this.Connection.GetAsync<DateTime[]>("alltype.date").ConfigureAwait(false);
//        Assert.Equal(result?.Length, 6);
//    }

//    [Fact]
//    public async Task TestDeserializeDateTimeListAsync()
//    {
//        var result = await this.Connection.GetAsync<DateTime[]>("alltype.datetime").ConfigureAwait(false);
//        Assert.Equal(result?.Length, 6);
//    }

//    [Fact]
//    public async Task TestDeserializeTimeSpanListAsync()
//    {
//        var result = await this.Connection.GetAsync<TimeSpan[]>("alltype.timespan").ConfigureAwait(false);
//        Assert.Equal(result?.Length, 6);
//    }

//    [Fact]
//    public async Task TestDeserializeMinuteListAsync()
//    {
//        var result = await this.Connection.GetAsync<DateTime[]>("alltype.minute").ConfigureAwait(false);
//        Assert.Equal(result?.Length, 6);
//    }

//    [Fact]
//    public async Task TestDeserializeSecondListAsync()
//    {
//        var result = await this.Connection.GetAsync<DateTime[]>("alltype.second").ConfigureAwait(false);
//        Assert.Equal(result?.Length, 6);
//    }

//    [Fact]
//    public async Task TestDeserializeTimeListAsync()
//    {
//        var result = await this.Connection.GetAsync<DateTime[]>("alltype.time").ConfigureAwait(false);
//        Assert.Equal(result?.Length, 6);
//    }
//}
