
# KdbSharp

[![KdbSharp NuGet Package](https://img.shields.io/nuget/v/KdbSharp.svg)](https://www.nuget.org/packages/KdbSharp/) [![KdbSharp NuGet Package Downloads](https://img.shields.io/nuget/dt/KdbSharp)](https://www.nuget.org/packages/KdbSharp) [![GitHub Actions Status](https://github.com/anchurcn/KdbSharp/workflows/Build/badge.svg?branch=main)](https://github.com/anchurcn/KdbSharp/actions)

[![GitHub Actions Build History](https://buildstats.info/github/chart/anchurcn/KdbSharp?branch=main&includeBuildsFromPullRequest=false)](https://github.com/anchurcn/KdbSharp/actions)

KdbSharp is a kdb client for .NET, providing a modern and efficient way to interact with kdb+ databases from .NET applications.

## Status

This project is a work in progress. The API is subject to change.
Production use is not recommended at this time.

## Features

- Connect to kdb+ databases
- Serialize and deserialize kdb+ data types
- Execute queries against kdb+ databases
- Support for asynchronous operations

## Installation

Install the package via NuGet:

```bash
dotnet add package KdbSharp
```

## Usage

Basic usage example:

```csharp
// Connect to a kdb+ server
var connection = new KdbConnection(new KdbConnectionOptions
{
    Host = "localhost",
    Port = 5000,
    Username = "username",  // Optional
    Password = "password"   // Optional
});

// Open the connection
await connection.OpenAsync();

// Execute a simple query and get the result
var result = await connection.GetAsync<object>("`q`w`e!1 2 3");

// Execute a query with parameters using CreateCommand
var command = connection.CreateCommand<int, string>("select from table where id=? and name=?", 123, "test");
var queryResult = await command.GetAsync<KTable>();

// Subscribe to a data feed
await connection.SetAsync(".u.sub[`trade;`]");
await foreach (var message in connection.Subscribe().WithCancellation(cancellationToken))
{
    if (message.Compressed)
    {
        message.Uncompress();
    }
    
    var data = message.Deserialize(new KSerializerOptions());
    // Process the data
    
    // Important: Dispose the message when done
    message.Dispose();
}
```

Connection string can also be used:

```csharp
// Connect using a connection string
var connection = new KdbConnection("Host=localhost;Port=5000;Username=user;Password=pass");
await connection.OpenAsync();
```

Working with different data types:

```csharp
// Get primitive types
bool boolValue = await connection.GetAsync<bool>("1b");
int intValue = await connection.GetAsync<int>("42i");
double doubleValue = await connection.GetAsync<double>("3.14");
string stringValue = await connection.GetAsync<string>("\"hello\"");

// Get collections
int[] intArray = await connection.GetAsync<int[]>("1 2 3 4 5");
Dictionary<string, object> dict = await connection.GetAsync<Dictionary<string, object>>("`a`b`c!1 2 3");

// Get tables
KTable table = await connection.GetAsync<KTable>("([] col1:1 2 3; col2:`a`b`c)");
```

## Requirements

- .NET 6.0 or higher

## Building from Source

1. Clone the repository
2. Run `dotnet tool restore`
3. Run `dotnet cake --target=Build`

## Running Tests

```bash
dotnet cake --target=Test
```

## Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](https://github.com/anchurcn/KdbSharp/blob/main/.github/CONTRIBUTING.md) for details.

## License

This project is licensed under the Apache License 2.0 - see the LICENSE file for details.

## Acknowledgements

The `KMessage.Uncompress` method modifications were made to functions originally found in the following repository:
- [Original Repository URL](https://github.com/exxeleron/qSharp/blob/acd64929ce84e4329ba74ff58356eb4f5aafaf03/qSharp/src/QReader.cs#L136)
