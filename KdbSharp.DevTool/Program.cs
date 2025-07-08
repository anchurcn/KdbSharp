/*
 * 开发一个工具，连接到 KDB，收集 alltype 命名空间下的变量，
 * 然后遍历读取它们并将他们的 Body 字节流写入 alltype.txt 文件。
 * 每个变量的格式为（每32字节换行）：
 * variable_name: [
 * FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF
 * FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF
 * ...
 * ];
 * 后续为这些变量生成读取测试用例，测试数据就读取这些序列化的字节流
 */

using DevTool;
using KdbSharp;
using System.Text;

// Main menu
Console.WriteLine("KdbSharp DevTool");
Console.WriteLine("================");
Console.WriteLine();
Console.WriteLine("Available options:");
Console.WriteLine("1. Generate test data from KDB alltype namespace");
Console.WriteLine("2. Test Container types reading from KDB");
Console.WriteLine("3. Exit");
Console.WriteLine();

while (true)
{
    Console.Write("Please select an option (1-3): ");
    var input = Console.ReadLine();

    switch (input)
    {
        case "1":
            await RunGenerateTestDataAsync();
            break;
        case "2":
            await RunContainerTypeTestAsync();
            break;
        case "3":
            Console.WriteLine("Goodbye!");
            return 0;
        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid option. Please select 1, 2, or 3.");
            Console.ResetColor();
            continue;
    }

    Console.WriteLine();
    Console.WriteLine("Press any key to return to main menu...");
    Console.ReadKey();
    Console.Clear();
    Console.WriteLine("KdbSharp DevTool");
    Console.WriteLine("================");
    Console.WriteLine();
    Console.WriteLine("Available options:");
    Console.WriteLine("1. Generate test data from KDB alltype namespace");
    Console.WriteLine("2. Test Container types reading from KDB");
    Console.WriteLine("3. Exit");
    Console.WriteLine();
}

static async Task RunContainerTypeTestAsync()
{
    Console.WriteLine();
    Console.WriteLine("Container Type Test");
    Console.WriteLine("==================");

    Console.Write("Enter connection string (default: localhost:5000): ");
    var connectionString = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        connectionString = "localhost:5000";
    }

    Console.WriteLine($"Connecting to: {connectionString}");
    Console.WriteLine();

    try
    {
        var tester = new ContainerTypeTest(connectionString);
        await tester.RunAllTestsAsync();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error running container type test: {ex.Message}");
        Console.ResetColor();
    }
}

static async Task RunGenerateTestDataAsync()
{
    Console.WriteLine();
    Console.WriteLine("Generate Test Data from KDB alltype namespace");
    Console.WriteLine("=============================================");

    try
    {
        await GenerateTestDataAsync();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Test data generation completed successfully!");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        Console.ResetColor();
    }
}


static async Task GenerateTestDataAsync()
{
    Console.Write("Enter KDB host (default: localhost): ");
    var hostInput = Console.ReadLine();
    var host = string.IsNullOrWhiteSpace(hostInput) ? "localhost" : hostInput;

    Console.Write("Enter KDB port (default: 5000): ");
    var portInput = Console.ReadLine();
    var port = string.IsNullOrWhiteSpace(portInput) ? 5000 : int.Parse(portInput);

    // Output to test project directory
    const string testProjectDir = "../../../../Tests/KdbSharp.Test";
    var outputFile = Path.Combine(testProjectDir, "alltype.txt");
    var testFile = Path.Combine(testProjectDir, "DeserializeTest.cs");

    // Ensure the test project directory exists
    Directory.CreateDirectory(testProjectDir);
    Console.WriteLine($"Output directory: {Path.GetFullPath(testProjectDir)}");

    Console.WriteLine($"Connecting to KDB at {host}:{port}...");

    var options = new KdbConnectionOptions
    {
        Host = host,
        Port = port
    };

    var connection = new KdbConnection(options);
    await connection.OpenAsync();

    Console.WriteLine("Connected successfully!");

    // Load the init.q file to set up test data
    Console.WriteLine("Loading init.q file...");
    var initScript = await File.ReadAllTextAsync("init.q");
    await connection.SetAsync(initScript);

    Console.WriteLine("Getting alltype namespace variables...");

    // Get all variables in alltype namespace
    var variableNames = await GetAllTypeVariablesAsync(connection);
    Console.WriteLine($"Found {variableNames.Count} variables in alltype namespace");

    // Generate test data file
    await GenerateTestDataFileAsync(connection, variableNames, outputFile);
    Console.WriteLine($"Generated test data file: {outputFile}");

    // Generate test case file
    await GenerateTestCaseFileAsync(variableNames, testFile);
    Console.WriteLine($"Generated test case file: {testFile}");

    // Close the connection
    connection.Close();
}

static async Task<List<string>> GetAllTypeVariablesAsync(KdbConnection connection)
{
    // Query to get all variables in alltype namespace
    var query = "key alltype";
    var result = await connection.GetAsync<string[]>(query);

    // Filter out empty or null variable names
    return result?.Where(name => !string.IsNullOrWhiteSpace(name)).ToList() ?? new List<string>();
}

static async Task GenerateTestDataFileAsync(KdbConnection connection, List<string> variableNames, string outputFile)
{
    using var writer = new StreamWriter(outputFile, false, Encoding.UTF8);

    foreach (var variableName in variableNames)
    {
        Console.WriteLine($"Processing variable: {variableName}");

        try
        {
            // Get the serialized bytes for this variable
            var query = $"alltype.{variableName}";
            var message = await GetRawMessageAsync(connection, query);

            // Extract the body bytes (skip the header)
            var bodyBytes = message.Body.ToArray();

            // Write variable data in the specified format
            await writer.WriteLineAsync($"{variableName}: [");

            // Write bytes in hex format, 32 bytes per line
            for (int i = 0; i < bodyBytes.Length; i += 32)
            {
                var lineBytes = bodyBytes.Skip(i).Take(32).ToArray();
                var hexLine = string.Join(" ", lineBytes.Select(b => b.ToString("X2")));
                await writer.WriteLineAsync(hexLine);
            }

            await writer.WriteLineAsync("];");
            await writer.WriteLineAsync(); // Empty line between variables

            // Dispose the message
            message.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing {variableName}: {ex.Message}");
            await writer.WriteLineAsync($"// Error processing {variableName}: {ex.Message}");
            await writer.WriteLineAsync();
        }
    }
}

static async Task<KMessage> GetRawMessageAsync(KdbConnection connection, string query)
{
    // Send the query
    await connection.SendQueryObjectAsync(query, MessageType.Request, null, default);

    // Receive the raw response message
    var message = await connection.RecvAsync(default);

    if (message.Type != MessageType.Response)
    {
        message.Dispose();
        throw new InvalidOperationException($"Unexpected message type {message.Type}.");
    }

    if (message.Compressed)
    {
        var uncompressed = KMessage.Uncompress(message);
        message.Dispose();
        return uncompressed;
    }

    return message;
}

static async Task GenerateTestCaseFileAsync(List<string> variableNames, string testFile)
{
    using var writer = new StreamWriter(testFile, false, Encoding.UTF8);

    // Write file header
    await writer.WriteLineAsync("/*");
    await writer.WriteLineAsync(" * Auto-generated test cases for KdbSharp deserialization");
    await writer.WriteLineAsync(" * Generated from alltype namespace variables");
    await writer.WriteLineAsync(" */");
    await writer.WriteLineAsync();
    await writer.WriteLineAsync("using System;");
    await writer.WriteLineAsync("using System.IO;");
    await writer.WriteLineAsync("using Xunit;");
    await writer.WriteLineAsync("using KdbSharp;");
    await writer.WriteLineAsync("using KdbSharp.Serialization;");
    await writer.WriteLineAsync("using KdbSharp.Types;");
    await writer.WriteLineAsync();
    await writer.WriteLineAsync("namespace KdbSharp.Test;");
    await writer.WriteLineAsync();
    await writer.WriteLineAsync("public class DeserializeTest");
    await writer.WriteLineAsync("{");
    await writer.WriteLineAsync("    private static readonly TestDataReader _testDataReader = new();");
    await writer.WriteLineAsync();

    // Generate test methods for each variable
    foreach (var variableName in variableNames)
    {
        var methodName = ConvertToValidMethodName(variableName);
        var (netType, expectedValue, assertionCode) = GetExpectedValueForVariable(variableName);

        await writer.WriteLineAsync($"    [Fact]");
        await writer.WriteLineAsync($"    public void Test_{methodName}()");
        await writer.WriteLineAsync("    {");
        await writer.WriteLineAsync($"        // Test deserialization of alltype.{variableName}");
        await writer.WriteLineAsync($"        var data = _testDataReader.GetTestData(\"{variableName}\");");
        await writer.WriteLineAsync("        Assert.NotNull(data);");
        await writer.WriteLineAsync("        Assert.NotEmpty(data);");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("        var reader = new KSerializationReader(data);");
        await writer.WriteLineAsync($"        var result = reader.Read<{netType}>();");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync($"        {assertionCode}");
        await writer.WriteLineAsync("    }");
        await writer.WriteLineAsync();
    }

    await writer.WriteLineAsync("}");
    await writer.WriteLineAsync();

    // Generate TestDataReader helper class
    await GenerateTestDataReaderClassAsync(writer, variableNames);
}

static async Task GenerateTestDataReaderClassAsync(StreamWriter writer, List<string> variableNames)
{
    await writer.WriteLineAsync("/// <summary>");
    await writer.WriteLineAsync("/// Helper class to read test data from alltype.txt file");
    await writer.WriteLineAsync("/// </summary>");
    await writer.WriteLineAsync("public class TestDataReader");
    await writer.WriteLineAsync("{");
    await writer.WriteLineAsync("    private static readonly Dictionary<string, byte[]> _testData = LoadTestData();");
    await writer.WriteLineAsync();
    await writer.WriteLineAsync("    public byte[] GetTestData(string variableName)");
    await writer.WriteLineAsync("    {");
    await writer.WriteLineAsync("        return _testData.TryGetValue(variableName, out var data) ? data : Array.Empty<byte>();");
    await writer.WriteLineAsync("    }");
    await writer.WriteLineAsync();
    await writer.WriteLineAsync("    private static Dictionary<string, byte[]> LoadTestData()");
    await writer.WriteLineAsync("    {");
    await writer.WriteLineAsync("        var result = new Dictionary<string, byte[]>();");
    await writer.WriteLineAsync("        var lines = File.ReadAllLines(\"alltype.txt\");");
    await writer.WriteLineAsync();
    await writer.WriteLineAsync("        string? currentVariable = null;");
    await writer.WriteLineAsync("        var currentBytes = new List<byte>();");
    await writer.WriteLineAsync();
    await writer.WriteLineAsync("        foreach (var line in lines)");
    await writer.WriteLineAsync("        {");
    await writer.WriteLineAsync("            var trimmedLine = line.Trim();");
    await writer.WriteLineAsync();
    await writer.WriteLineAsync("            if (trimmedLine.EndsWith(\": [\"))");
    await writer.WriteLineAsync("            {");
    await writer.WriteLineAsync("                // Start of new variable");
    await writer.WriteLineAsync("                currentVariable = trimmedLine.Substring(0, trimmedLine.Length - 3);");
    await writer.WriteLineAsync("                currentBytes.Clear();");
    await writer.WriteLineAsync("            }");
    await writer.WriteLineAsync("            else if (trimmedLine == \"];\")");
    await writer.WriteLineAsync("            {");
    await writer.WriteLineAsync("                // End of current variable");
    await writer.WriteLineAsync("                if (currentVariable != null)");
    await writer.WriteLineAsync("                {");
    await writer.WriteLineAsync("                    result[currentVariable] = currentBytes.ToArray();");
    await writer.WriteLineAsync("                    currentVariable = null;");
    await writer.WriteLineAsync("                }");
    await writer.WriteLineAsync("            }");
    await writer.WriteLineAsync("            else if (!string.IsNullOrEmpty(trimmedLine) && !trimmedLine.StartsWith(\"//\"))");
    await writer.WriteLineAsync("            {");
    await writer.WriteLineAsync("                // Hex data line");
    await writer.WriteLineAsync("                var hexValues = trimmedLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);");
    await writer.WriteLineAsync("                foreach (var hex in hexValues)");
    await writer.WriteLineAsync("                {");
    await writer.WriteLineAsync("                    if (byte.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out var b))");
    await writer.WriteLineAsync("                    {");
    await writer.WriteLineAsync("                        currentBytes.Add(b);");
    await writer.WriteLineAsync("                    }");
    await writer.WriteLineAsync("                }");
    await writer.WriteLineAsync("            }");
    await writer.WriteLineAsync("        }");
    await writer.WriteLineAsync();
    await writer.WriteLineAsync("        return result;");
    await writer.WriteLineAsync("    }");
    await writer.WriteLineAsync("}");
}

static string ConvertToValidMethodName(string variableName)
{
    // Convert variable name to valid C# method name
    // Replace invalid characters with underscores and ensure it starts with a letter
    var result = new StringBuilder();

    for (int i = 0; i < variableName.Length; i++)
    {
        char c = variableName[i];
        if (char.IsLetterOrDigit(c))
        {
            result.Append(c);
        }
        else
        {
            result.Append('_');
        }
    }

    var methodName = result.ToString();

    // Ensure it starts with a letter
    if (methodName.Length > 0 && !char.IsLetter(methodName[0]))
    {
        methodName = "Var_" + methodName;
    }

    return methodName;
}

static (string netType, string expectedValue, string assertionCode) GetExpectedValueForVariable(string variableName)
{
    // Based on the variable name pattern, determine the .NET type and expected value
    // Pattern: type_condition (e.g., boolean_false, int_null, float_inf)

    if (variableName.StartsWith("boolean_"))
    {
        if (variableName.EndsWith("_false"))
            return ("bool", "false", "Assert.False(result);");
        if (variableName.EndsWith("_true"))
            return ("bool", "true", "Assert.True(result);");
    }
    else if (variableName.StartsWith("guid_"))
    {
        if (variableName.EndsWith("_null"))
            return ("Guid", "Guid.Empty", "Assert.Equal(Guid.Empty, result);");
        if (variableName.EndsWith("_value"))
            return ("Guid", "Guid.Parse(\"11223344-5566-7788-99aa-bbccddeeffaa\")", "Assert.Equal(Guid.Parse(\"11223344-5566-7788-99aa-bbccddeeffaa\"), result);");
    }
    else if (variableName.StartsWith("byte_"))
    {
        if (variableName.EndsWith("_zero"))
            return ("byte", "0", "Assert.Equal((byte)0, result);");
        if (variableName.EndsWith("_max"))
            return ("byte", "255", "Assert.Equal((byte)255, result);");
    }
    else if (variableName.StartsWith("char_"))
    {
        if (variableName.EndsWith("_null"))
            return ("char", "' '", "Assert.Equal(' ', result);");
        if (variableName.EndsWith("_value"))
            return ("char", "'a'", "Assert.Equal('a', result);");
    }
    else if (variableName.StartsWith("symbol_"))
    {
        if (variableName.EndsWith("_null"))
            return ("string", "string.Empty", "Assert.Equal(string.Empty, result);");
        if (variableName.EndsWith("_value"))
            return ("string", "\"a\"", "Assert.Equal(\"a\", result);");
    }
    else if (variableName.StartsWith("short_"))
    {
        if (variableName.EndsWith("_zero"))
            return ("short", "0", "Assert.Equal((short)0, result);");
        if (variableName.EndsWith("_null"))
            return ("short", "short.MinValue", "Assert.Equal(short.MinValue, result);");
        if (variableName.EndsWith("_inf"))
            return ("short", "short.MaxValue", "Assert.Equal(short.MaxValue, result);");
        if (variableName.EndsWith("_ninf"))
            return ("short", "-short.MaxValue", "Assert.Equal(-short.MaxValue, result);");
        if (variableName.EndsWith("_max"))
            return ("short", "32766", "Assert.Equal((short)32766, result);");
        if (variableName.EndsWith("_min"))
            return ("short", "-32766", "Assert.Equal((short)-32766, result);");
    }
    else if (variableName.StartsWith("int_"))
    {
        if (variableName.EndsWith("_zero"))
            return ("int", "0", "Assert.Equal(0, result);");
        if (variableName.EndsWith("_null"))
            return ("int", "int.MinValue", "Assert.Equal(int.MinValue, result);");
        if (variableName.EndsWith("_inf"))
            return ("int", "int.MaxValue", "Assert.Equal(int.MaxValue, result);");
        if (variableName.EndsWith("_ninf"))
            return ("int", "-int.MaxValue", "Assert.Equal(-int.MaxValue, result);");
        if (variableName.EndsWith("_max"))
            return ("int", "2147483646", "Assert.Equal(2147483646, result);");
        if (variableName.EndsWith("_min"))
            return ("int", "-2147483646", "Assert.Equal(-2147483646, result);");
    }
    else if (variableName.StartsWith("long_"))
    {
        if (variableName.EndsWith("_zero"))
            return ("long", "0L", "Assert.Equal(0L, result);");
        if (variableName.EndsWith("_null"))
            return ("long", "long.MinValue", "Assert.Equal(long.MinValue, result);");
        if (variableName.EndsWith("_inf"))
            return ("long", "long.MaxValue", "Assert.Equal(long.MaxValue, result);");
        if (variableName.EndsWith("_ninf"))
            return ("long", "-long.MaxValue", "Assert.Equal(-long.MaxValue, result);");
        if (variableName.EndsWith("_max"))
            return ("long", "9223372036854775806L", "Assert.Equal(9223372036854775806L, result);");
        if (variableName.EndsWith("_min"))
            return ("long", "-9223372036854775806L", "Assert.Equal(-9223372036854775806L, result);");
    }
    else if (variableName.StartsWith("real_"))
    {
        if (variableName.EndsWith("_zero"))
            return ("float", "0f", "Assert.Equal(0f, result);");
        if (variableName.EndsWith("_null"))
            return ("float", "float.NaN", "Assert.Equal(float.NaN, result);");
        if (variableName.EndsWith("_inf"))
            return ("float", "float.PositiveInfinity", "Assert.Equal(float.PositiveInfinity, result);");
        if (variableName.EndsWith("_ninf"))
            return ("float", "float.NegativeInfinity", "Assert.Equal(float.NegativeInfinity, result);");
        if (variableName.EndsWith("_max"))
            return ("float", "114514.1919810f", "Assert.Equal(114514.1919810f, result);");
        if (variableName.EndsWith("_min"))
            return ("float", "-114514.1919810f", "Assert.Equal(-114514.1919810f, result);");
    }
    else if (variableName.StartsWith("float_"))
    {
        if (variableName.EndsWith("_zero"))
            return ("double", "0.0", "Assert.Equal(0.0, result);");
        if (variableName.EndsWith("_null"))
            return ("double", "double.NaN", "Assert.Equal(double.NaN, result);");
        if (variableName.EndsWith("_inf"))
            return ("double", "double.PositiveInfinity", "Assert.Equal(double.PositiveInfinity, result);");
        if (variableName.EndsWith("_ninf"))
            return ("double", "double.NegativeInfinity", "Assert.Equal(double.NegativeInfinity, result);");
        if (variableName.EndsWith("_max"))
            return ("double", "114514.1919810", "Assert.Equal(114514.1919810, result);");
        if (variableName.EndsWith("_min"))
            return ("double", "-114514.1919810", "Assert.Equal(-114514.1919810, result);");
    }
    else if (variableName.StartsWith("timestamp_"))
    {
        if (variableName.EndsWith("_zero"))
            return ("DateTime", "new DateTime(2000, 1, 1)", "Assert.Equal(new DateTime(2000, 1, 1), result);");
        if (variableName.EndsWith("_null"))
            return ("DateTime?", "null", "Assert.Null(result);");
        if (variableName.EndsWith("_inf"))
            return ("DateTime", "DateTime.MaxValue", "Assert.Equal(DateTime.MaxValue, result);");
        if (variableName.EndsWith("_ninf"))
            return ("DateTime", "DateTime.MinValue", "Assert.Equal(DateTime.MinValue, result);");
        if (variableName.EndsWith("_max"))
            return ("DateTime", "new DateTime(2000, 1, 1).AddTicks(9223372036854775806L / 100)", "Assert.Equal(new DateTime(2000, 1, 1).AddTicks(9223372036854775806L / 100), result);");
        if (variableName.EndsWith("_min"))
            return ("DateTime", "new DateTime(2000, 1, 1).AddTicks(-9223372036854775806L / 100)", "Assert.Equal(new DateTime(2000, 1, 1).AddTicks(-9223372036854775806L / 100), result);");
    }
    else if (variableName.StartsWith("month_"))
    {
        if (variableName.EndsWith("_zero"))
            return ("DateTime", "new DateTime(2000, 1, 1)", "Assert.Equal(new DateTime(2000, 1, 1), result);");
        if (variableName.EndsWith("_null"))
            return ("DateTime?", "null", "Assert.Null(result);");
        if (variableName.EndsWith("_inf"))
            return ("DateTime", "DateTime.MaxValue", "Assert.Equal(DateTime.MaxValue, result);");
        if (variableName.EndsWith("_ninf"))
            return ("DateTime", "DateTime.MinValue", "Assert.Equal(DateTime.MinValue, result);");
        if (variableName.EndsWith("_max"))
            return ("DateTime", "new DateTime(2000, 1, 1).AddMonths(2147483646)", "Assert.Equal(new DateTime(2000, 1, 1).AddMonths(2147483646), result);");
        if (variableName.EndsWith("_min"))
            return ("DateTime", "new DateTime(2000, 1, 1).AddMonths(-2147483646)", "Assert.Equal(new DateTime(2000, 1, 1).AddMonths(-2147483646), result);");
    }
    else if (variableName.StartsWith("date_"))
    {
        if (variableName.EndsWith("_zero"))
            return ("DateTime", "new DateTime(2000, 1, 1)", "Assert.Equal(new DateTime(2000, 1, 1), result);");
        if (variableName.EndsWith("_null"))
            return ("DateTime?", "null", "Assert.Null(result);");
        if (variableName.EndsWith("_inf"))
            return ("DateTime", "DateTime.MaxValue", "Assert.Equal(DateTime.MaxValue, result);");
        if (variableName.EndsWith("_ninf"))
            return ("DateTime", "DateTime.MinValue", "Assert.Equal(DateTime.MinValue, result);");
        if (variableName.EndsWith("_max"))
            return ("DateTime", "new DateTime(2000, 1, 1).AddDays(2147483646)", "Assert.Equal(new DateTime(2000, 1, 1).AddDays(2147483646), result);");
        if (variableName.EndsWith("_min"))
            return ("DateTime", "new DateTime(2000, 1, 1).AddDays(-2147483646)", "Assert.Equal(new DateTime(2000, 1, 1).AddDays(-2147483646), result);");
    }

    // Default fallback
    return ("object", "null", "// TODO: Add specific assertion for " + variableName);
}

