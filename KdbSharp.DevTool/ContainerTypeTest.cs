/*
 * DevTool for testing Container types reading from KDB
 * This tool connects to KDB and tests various container types to check for exceptions
 */

using System;
using System.Threading.Tasks;
using KdbSharp;

namespace DevTool;

public class ContainerTypeTest
{
    private readonly KdbConnection _connection;

    public ContainerTypeTest(string connectionString = "localhost:5000")
    {
        var parts = connectionString.Split(':');
        var options = new KdbConnectionOptions
        {
            Host = parts.Length > 0 ? parts[0] : "localhost",
            Port = parts.Length > 1 ? int.Parse(parts[1]) : 5000
        };
        _connection = new KdbConnection(options);
    }

    public async Task RunAllTestsAsync()
    {
        try
        {
            await _connection.OpenAsync();
            Console.WriteLine("Connected to KDB successfully");

            // Test Dictionary types
            await TestDictionaries();

            // Test Table types
            await TestTables();

            // Test General List types
            await TestGeneralLists();

            // Test Mixed types
            await TestMixedTypes();

            Console.WriteLine("\nAll container type tests completed!");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Connection or general error: {ex.Message}");
            Console.ResetColor();
        }
        finally
        {
            _connection.Close();
        }
    }

    private async Task TestDictionaries()
    {
        Console.WriteLine("\n=== Testing Dictionary Types ===");

        var dictionaryTests = new[]
        {
            // Simple dictionary
            ("`a`b`c!1 2 3", "Simple symbol-int dictionary"),
            
            // Dictionary with different value types
            ("`name`age`active!(\"John\";25;1b)", "Mixed type dictionary"),
            
            // Dictionary with list values
            ("`x`y!(1 2 3;4 5 6)", "Dictionary with list values"),
            
            // Empty dictionary
            ("()!()", "Empty dictionary"),
            
            // Dictionary with nested structure
            ("`outer`inner!(1 2;`a`b!3 4)", "Nested dictionary"),
        };

        foreach (var (query, description) in dictionaryTests)
        {
            await TestQuery(query, description);
        }
    }

    private async Task TestTables()
    {
        Console.WriteLine("\n=== Testing Table Types ===");

        var tableTests = new[]
        {
            // Simple table
            ("([]name:`John`Jane`Bob;age:25 30 35)", "Simple table"),
            
            // Table with different column types
            ("([]sym:`AAPL`GOOGL`MSFT;price:150.5 2800.0 300.0;volume:1000 500 750)", "Mixed column types table"),
            
            // Empty table
            ("([]a:();b:())", "Empty table"),
            
            // Table with null values
            ("([]x:1 0N 3;y:`a`b`)", "Table with nulls"),
            
            // Keyed table
            ("([id:1 2 3]name:`John`Jane`Bob;age:25 30 35)", "Keyed table"),
        };

        foreach (var (query, description) in tableTests)
        {
            await TestQuery(query, description);
        }
    }

    private async Task TestGeneralLists()
    {
        Console.WriteLine("\n=== Testing General List Types ===");

        var listTests = new[]
        {
            // Mixed type list
            ("(1;`symbol;\"string\";1b)", "Mixed type general list"),
            
            // Nested lists
            ("((1 2 3);(`a`b`c);(\"hello\";\"world\"))", "Nested lists"),
            
            // List with nulls
            ("(1;0N;`)", "List with nulls"),
            
            // Empty general list
            ("()", "Empty general list"),
            
            // List with dictionaries
            ("((`a!1);(`b!2);(`c!3))", "List of dictionaries"),
            
            // List with tables
            ("(([]x:1 2);([]y:3 4))", "List of tables"),
        };

        foreach (var (query, description) in listTests)
        {
            await TestQuery(query, description);
        }
    }

    private async Task TestMixedTypes()
    {
        Console.WriteLine("\n=== Testing Mixed Complex Types ===");

        var mixedTests = new[]
        {
            // Dictionary containing tables
            ("`table1`table2!(([]a:1 2 3);([]b:4 5 6))", "Dictionary of tables"),
            
            // Table with dictionary column
            ("([]id:1 2;data:(`a!1;`b!2))", "Table with dictionary column"),
            
            // General list with mixed containers
            ("(([]x:1 2);`a`b!3 4;(5;6;7))", "Mixed containers in general list"),
            
            // Complex trading data structure
            ("([]time:.z.t;sym:`AAPL`GOOGL;data:(`price`volume!(150.5 2800.0;1000 500);`price`volume!(151.0 2801.0;1100 600)))", "Complex trading data"),

            // Intentional error test (invalid syntax)
            ("invalid_syntax_test", "Intentional error test"),
        };

        foreach (var (query, description) in mixedTests)
        {
            await TestQuery(query, description);
        }
    }

    private async Task TestQuery(string query, string description)
    {
        try
        {
            Console.Write($"Testing {description}: ");
            var result = await _connection.GetAsync<object>(query);
            
            if (result != null)
            {
                var resultType = result.GetType().Name;
                Console.WriteLine($"not null Success - Type: {resultType}");
                
                // Print some basic info about the result
                if (result is Array array)
                {
                    Console.WriteLine($"  Array length: {array.Length}");
                }
                else if (result.GetType().IsGenericType)
                {
                    Console.WriteLine($"  Generic type: {result.GetType().GetGenericTypeDefinition().Name}");
                }
            }
            else
            {
                Console.WriteLine("null Success - Result is null");
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ Exception: {ex.GetType().Name} - {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"  Inner: {ex.InnerException.GetType().Name} - {ex.InnerException.Message}");
            }
            Console.ResetColor();
        }
    }


}
