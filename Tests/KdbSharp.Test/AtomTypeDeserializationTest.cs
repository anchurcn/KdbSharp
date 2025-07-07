using KdbSharp.Types;
using Xunit;

namespace KdbSharp.Test;

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
    }

    public KdbConnection Connection { get; }

    [Fact]
    public async Task TestDeserializeBooleanFalseAsync()
    {
        var value = new KLong(long.MinValue);
        Assert.False(value.IsNull);
    }
}
