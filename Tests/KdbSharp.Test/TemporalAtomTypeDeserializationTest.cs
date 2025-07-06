using Xunit;

namespace KdbSharp.Test;

public class TemporalAtomTypeDeserializationTest
{
    public const string Host = "localhost";
    public const int Port = 5011;
    public const string Username = "username";
    public const string Password = "password";

    public TemporalAtomTypeDeserializationTest()
    {
        this.Connection = new KdbConnection(new KdbConnectionOptions
        {
            Host = Host,
            Port = Port,
        });
        this.Connection.OpenAsync().GetAwaiter().GetResult();
    }

    public KdbConnection Connection { get; }

    [Fact]
    public async Task TestTimestampZeroAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.timestamp_zero");
    }

    [Fact]
    public async Task TestTimestampNullAsync()
    {
        var result = await this.Connection.GetAsync<DateTime?>("alltype.timestamp_null");
    }

    [Fact]
    public async Task TestTimestampInfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.timestamp_inf");
    }

    [Fact]
    public async Task TestTimestampNinfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.timestamp_ninf");
    }

    [Fact]
    public async Task TestTimestampMaxAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.timestamp_max");
    }

    [Fact]
    public async Task TestTimestampMinAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.timestamp_min");
    }

    [Fact]
    public async Task TestMonthZeroAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.month_zero");
    }

    [Fact]
    public async Task TestMonthNullAsync()
    {
        var result = await this.Connection.GetAsync<DateTime?>("alltype.month_null");
    }

    [Fact]
    public async Task TestMonthInfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.month_inf");
    }

    [Fact]
    public async Task TestMonthNinfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.month_ninf");
    }

    [Fact]
    public async Task TestMonthMaxAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.month_max");
    }

    [Fact]
    public async Task TestMonthMinAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.month_min");
    }

    [Fact]
    public async Task TestDateZeroAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.date_zero");
    }

    [Fact]
    public async Task TestDateNullAsync()
    {
        var result = await this.Connection.GetAsync<DateTime?>("alltype.date_null");
    }

    [Fact]
    public async Task TestDateInfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.date_inf");
    }

    [Fact]
    public async Task TestDateNinfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.date_ninf");
    }

    [Fact]
    public async Task TestDateTimeZeroAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.datetime_zero");
    }

    [Fact]
    public async Task TestDateTimeNullAsync()
    {
        var result = await this.Connection.GetAsync<DateTime?>("alltype.datetime_null");
    }

    [Fact]
    public async Task TestDateTimeInfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.datetime_inf");
    }

    [Fact]
    public async Task TestDateTimeNinfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.datetime_ninf");
    }

    [Fact]
    public async Task TestDateTimeMaxAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.datetime_max");
    }

    [Fact]
    public async Task TestDateTimeMinAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.datetime_min");
    }

    [Fact]
    public async Task TestTimeSpanZeroAsync()
    {
        var result = await this.Connection.GetAsync<TimeSpan>("alltype.timespan_zero");
    }

    [Fact]
    public async Task TestTimeSpanNullAsync()
    {
        var result = await this.Connection.GetAsync<TimeSpan?>("alltype.timespan_null");
    }

    [Fact]
    public async Task TestTimeSpanInfAsync()
    {
        var result = await this.Connection.GetAsync<TimeSpan>("alltype.timespan_inf");
    }

    [Fact]
    public async Task TestTimeSpanNinfAsync()
    {
        var result = await this.Connection.GetAsync<TimeSpan>("alltype.timespan_ninf");
    }

    [Fact]
    public async Task TestTimeSpanMaxAsync()
    {
        var result = await this.Connection.GetAsync<TimeSpan>("alltype.timespan_max");
    }

    [Fact]
    public async Task TestTimeSpanMinAsync()
    {
        var result = await this.Connection.GetAsync<TimeSpan>("alltype.timespan_min");
    }

    [Fact]
    public async Task TestMinuteZeroAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.minute_zero");
    }

    [Fact]
    public async Task TestMinuteNullAsync()
    {
        var result = await this.Connection.GetAsync<DateTime?>("alltype.minute_null");
    }

    [Fact]
    public async Task TestMinuteInfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.minute_inf");
    }

    [Fact]
    public async Task TestMinuteNinfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.minute_ninf");
    }

    [Fact]
    public async Task TestMinuteMaxAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.minute_max");
    }

    [Fact]
    public async Task TestMinuteMinAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.minute_min");
    }

    [Fact]
    public async Task TestSecondZeroAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.second_zero");
    }

    [Fact]
    public async Task TestSecondNullAsync()
    {
        var result = await this.Connection.GetAsync<DateTime?>("alltype.second_null");
    }

    [Fact]
    public async Task TestSecondInfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.second_inf");
    }

    [Fact]
    public async Task TestSecondNinfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.second_ninf");
    }

    [Fact]
    public async Task TestSecondMaxAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.second_max");
    }

    [Fact]
    public async Task TestSecondMinAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.second_min");
    }

    [Fact]
    public async Task TestTimeZeroAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.time_zero");
    }

    [Fact]
    public async Task TestTimeNullAsync()
    {
        var result = await this.Connection.GetAsync<DateTime?>("alltype.time_null");
    }

    [Fact]
    public async Task TestTimeInfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.time_inf");
    }

    [Fact]
    public async Task TestTimeNinfAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.time_ninf");
    }

    [Fact]
    public async Task TestTimeMaxAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.time_max");
    }

    [Fact]
    public async Task TestTimeMinAsync()
    {
        var result = await this.Connection.GetAsync<DateTime>("alltype.time_min");
    }
}
