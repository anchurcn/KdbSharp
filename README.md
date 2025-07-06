
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

### Basic Connection

```csharp
// Connect to a kdb+ server
var connection = new KdbConnection(new KdbConnectionOptions
{
    Host = "localhost",
    Port = 5000,
    Username = "username",  // Optional
    Password = "password"   // Optional
});

// h: hopen `:localhost:5000:username:password
await connection.OpenAsync();
```

### GetAsync - Execute Queries and Get Results

```csharp
// result: h "2+3"
var result = await connection.GetAsync<long>("2 + 3");
// table: h "([] col1:1 2 3; col2:`a`b`c)"
var table = await connection.GetAsync<KTable>("([] col1:1 2 3; col2:`a`b`c)");
```

### SetAsync - Execute Async Commands

```csharp
// neg[h] "insert[`trade] (1; `AAPL; 100.5; 1000)"
await connection.SetAsync("insert[`trade] (1; `AAPL; 100.5; 1000)");
// neg[h] ".u.sub[`trade;`]"
await connection.SetAsync(".u.sub[`trade;`]");  // Subscribe to updates
```

### Parameterized Queries with CreateCommand

```csharp
// h ("{select from trade where sym=x}"; "AAPL")
var result1 = await connection.CreateCommand("{select from trade where sym=x}", "AAPL")
    .GetAsync<KTable>();

// h ("select from trade where sym=x and size>y"; "AAPL"; 500)
var result2 = connection.CreateCommand("select from trade where sym=x and size>y", "AAPL", 500L)
    .GetAsync<KTable>();

// neg[h] ("insert[`trade] (`MSFT; x)"; 150.75)
var result3 = connection.CreateCommand("insert[`trade] (`MSFT; x)", 150.75)
    .SetAsync();
```

### Receiving Async Messages Using Subscribe Extension

```csharp
// Subscribe to updates
await connection.SetAsync(".u.sub[`trade;`]");

// Use the Subscribe extension method for easier async enumeration
await foreach (var message in connection.Subscribe().WithCancellation(cancellationToken))
{
    if (message.Compressed)
    {
        message.Uncompress();
    }

    var data = message.Deserialize();
    // TODO: Process the data

    // Important: Dispose the message when done
    message.Dispose();
}
```

## Requirements

- .NET 6.0 or higher


## Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](https://github.com/anchurcn/KdbSharp/blob/main/.github/CONTRIBUTING.md) for details.

## License

This project is licensed under the Apache License 2.0 - see the LICENSE file for details.

## Acknowledgements

The `KMessage.Uncompress` method modifications were made to functions originally found in the following repository:
- [Original Repository URL](https://github.com/exxeleron/qSharp/blob/acd64929ce84e4329ba74ff58356eb4f5aafaf03/qSharp/src/QReader.cs#L136)
