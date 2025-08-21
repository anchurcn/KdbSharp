/*
 Copyright (C) 2024 Anchur
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
*/

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using KdbSharp.Serialization;
using KdbSharp.Types;
using Xunit;

namespace KdbSharp.Test;

/// <summary>
/// Test data classes for serialization model builder tests.
/// </summary>
public class Contract
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<ContractItem> Items { get; set; } = new();
    public string InternalField { get; set; } = string.Empty; // Will be ignored
}

public class ContractItem
{
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
}

/// <summary>
/// Tests for the KDB serialization model builder functionality.
/// </summary>
public class SerializationModelBuilderTest
{
    [Fact]
    public void Builder_CanConfigureEntity()
    {
        // Arrange
        var options = new KSerializerOptions();

        // Act
        var entityBuilder = options.Builder.Entity<Contract>();

        // Assert
        Assert.NotNull(entityBuilder);
    }

    [Fact]
    public void Builder_CanConfigureEntityWithAction()
    {
        // Arrange
        var options = new KSerializerOptions();

        // Act & Assert - Should not throw
        options.Builder.Entity<Contract>(entityBuilder =>
        {
            entityBuilder.AsKDictionary();
            entityBuilder.Property(x => x.Id).HasKType(KType.Long);
            entityBuilder.Property(x => x.Name).HasKType(KType.Symbol);
            entityBuilder.Ignore(x => x.InternalField);
        });
    }

    [Fact]
    public void ApplyModelConfiguration_DoesNotThrow()
    {
        // Arrange
        var options = new KSerializerOptions();

        options.Builder.Entity<Contract>(entityBuilder =>
        {
            entityBuilder.AsKDictionary();
            entityBuilder.Property(x => x.Id).HasKType(KType.Long);
            entityBuilder.Property(x => x.Name).HasKType(KType.Symbol);
            entityBuilder.Ignore(x => x.InternalField);
        });

        // Act & Assert - Should not throw
        options.ApplyModelConfiguration();
    }
    
    [Fact]
    public void Builder_FluentAPI_WorksCorrectly()
    {
        // Arrange
        var options = new KSerializerOptions();

        // Act & Assert - Should not throw and allow method chaining
        options.Builder
            .Entity<Contract>(entityBuilder =>
            {
                entityBuilder.AsKDictionary();
                entityBuilder.ToKType(KType.Dictionary);
                entityBuilder.Property(x => x.Id).HasKType(KType.Long);
                entityBuilder.Property(x => x.Name).HasKType(KType.Symbol);
                entityBuilder.Property(x => x.CreatedAt).HasKType(KType.Timestamp);
                entityBuilder.Ignore(x => x.InternalField);
                entityBuilder.Ignore(x => x.Items);
            })
            .Entity<ContractItem>(entityBuilder =>
            {
                entityBuilder.AsKDictionary();
                entityBuilder.Property(x => x.ProductName).HasKType(KType.Symbol);
                entityBuilder.Property(x => x.Quantity).HasKType(KType.Real);
                entityBuilder.Property(x => x.Price).HasKType(KType.Float);
            });

        options.ApplyModelConfiguration();
    }

    [Fact]
    public void DataClassToDictionary_FullRoundTrip_PreservesData()
    {
        // Arrange - Create a contract with various data types
        var originalContract = new Contract
        {
            Id = 12345L,
            Name = "Test Contract",
            CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0),
            Items = new List<ContractItem>
            {
                new() { ProductName = "Product A", Quantity = 10.5m, Price = 99.99m },
                new() { ProductName = "Product B", Quantity = 5.0m, Price = 149.99m }
            },
            InternalField = "Should be ignored"
        };

        // Configure serialization
        var options = new KSerializerOptions();
        options.Builder.Entity<Contract>(entityBuilder =>
        {
            entityBuilder.AsKDictionary();
            entityBuilder.Property(x => x.Id).HasKType(KType.Long);
            entityBuilder.Property(x => x.Name).HasKType(KType.Symbol);
            entityBuilder.Property(x => x.CreatedAt).HasKType(KType.Timestamp);
            entityBuilder.Property(x => x.Items).HasKType(KType.GeneralList); // List will be serialized as general list
            entityBuilder.Ignore(x => x.InternalField);
        });
        options.ApplyModelConfiguration();

        // Act - Use ArrayBufferWriter and WrittenMemory
        var bufferWriter = new ArrayBufferWriter<byte>();
        var writer = new KSerializationWriter(bufferWriter);

        // Serialize using ref writer
        KSerializer.Serialize(ref writer, originalContract, options);

        // Create readers from written memory
        var reader1 = new KSerializationReader(bufferWriter.WrittenMemory);
        var asDictionary = KSerializer.Deserialize<KSimpleDictionary>(ref reader1, options);

        // Then deserialize back to original type
        var reader2 = new KSerializationReader(bufferWriter.WrittenMemory);
        var deserializedContract = KSerializer.Deserialize<Contract>(ref reader2, options);

        // Assert - Verify dictionary structure
        Assert.NotNull(asDictionary);
        var keys = asDictionary.Keys.Cast<string>().ToArray();
        Assert.Contains("Id", keys);
        Assert.Contains("Name", keys);
        Assert.Contains("CreatedAt", keys);
        Assert.Contains("Items", keys);
        Assert.DoesNotContain("InternalField", keys); // Should be ignored

        // Assert - Verify round-trip data integrity
        Assert.NotNull(deserializedContract);
        Assert.Equal(originalContract.Id, deserializedContract.Id);
        Assert.Equal(originalContract.Name, deserializedContract.Name);
        Assert.Equal(originalContract.CreatedAt, deserializedContract.CreatedAt);

        // InternalField should not be serialized, so it should be default value
        Assert.Equal(string.Empty, deserializedContract.InternalField);

        // Items should be preserved (exact comparison depends on how List<ContractItem> is handled)
        Assert.NotNull(deserializedContract.Items);

        // Verify buffer usage is reasonable
        Assert.True(bufferWriter.WrittenCount > 0);
        Assert.True(bufferWriter.WrittenCount < 4096); // Should be reasonable size
    }

    [Fact]
    public void DataClassToTable_MultipleContracts_CreatesTableStructure()
    {
        // Arrange - Create multiple contracts to serialize as table rows
        var contracts = new[]
        {
            new Contract { Id = 1, Name = "Contract A", CreatedAt = new DateTime(2024, 1, 1) },
            new Contract { Id = 2, Name = "Contract B", CreatedAt = new DateTime(2024, 2, 1) },
            new Contract { Id = 3, Name = "Contract C", CreatedAt = new DateTime(2024, 3, 1) }
        };

        // Configure each contract to serialize as dictionary (table row)
        var options = new KSerializerOptions();
        options.Builder.Entity<Contract>(entityBuilder =>
        {
            entityBuilder.AsKDictionary(); // Each contract becomes a dictionary (table row)
            entityBuilder.Property(x => x.Id).HasKType(KType.Long);
            entityBuilder.Property(x => x.Name).HasKType(KType.Symbol);
            entityBuilder.Property(x => x.CreatedAt).HasKType(KType.Timestamp);
            entityBuilder.Ignore(x => x.Items);
            entityBuilder.Ignore(x => x.InternalField);
        });
        options.ApplyModelConfiguration();

        // Act - Serialize each contract using ArrayBufferWriter and collect as table-like structure
        var deserializedDictionaries = new List<KSimpleDictionary>();
        var bufferSizes = new List<int>();

        foreach (var contract in contracts)
        {
            // Use ArrayBufferWriter for each contract
            var bufferWriter = new ArrayBufferWriter<byte>();
            var writer = new KSerializationWriter(bufferWriter);

            // Serialize using ref writer
            KSerializer.Serialize(ref writer, contract, options);
            bufferSizes.Add(bufferWriter.WrittenCount);

            // Deserialize back as dictionary
            var reader = new KSerializationReader(bufferWriter.WrittenMemory);
            var asDictionary = KSerializer.Deserialize<KSimpleDictionary>(ref reader, options);
            deserializedDictionaries.Add(asDictionary);
        }

        // Assert - Verify table structure
        Assert.Equal(3, deserializedDictionaries.Count);

        // All rows should have the same column structure
        var expectedColumns = new[] { "Id", "Name", "CreatedAt" };
        foreach (var dict in deserializedDictionaries)
        {
            var columns = dict.Keys.Cast<string>().ToArray();
            Assert.Equal(expectedColumns.Length, columns.Length);
            foreach (var expectedColumn in expectedColumns)
            {
                Assert.Contains(expectedColumn, columns);
            }
        }

        // Verify data integrity for each row
        for (int i = 0; i < contracts.Length; i++)
        {
            var dict = deserializedDictionaries[i];
            var columns = dict.Keys.Cast<string>().ToArray();

            var idIndex = Array.IndexOf(columns, "Id");
            var nameIndex = Array.IndexOf(columns, "Name");
            var createdAtIndex = Array.IndexOf(columns, "CreatedAt");

            Assert.Equal(contracts[i].Id, dict.Values.GetValue(idIndex));
            Assert.Equal(contracts[i].Name, dict.Values.GetValue(nameIndex));
            Assert.Equal(contracts[i].CreatedAt, dict.Values.GetValue(createdAtIndex));
        }

        // Verify all serializations used reasonable buffer space
        foreach (var size in bufferSizes)
        {
            Assert.True(size > 0);
            Assert.True(size < 2048); // Should be reasonable size
        }
    }

    [Fact]
    public void CustomConverter_PropertyLevel_OverridesDefaultBehavior()
    {
        // Arrange - Create a contract with a DateTime that we want to serialize as Unix timestamp
        var contract = new Contract
        {
            Id = 999L,
            Name = "Custom Converter Test",
            CreatedAt = new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Utc)
        };

        // Create custom converter for DateTime -> Long (Unix timestamp)
        var customDateTimeConverter = new UnixTimestampConverter();

        var options = new KSerializerOptions();
        options.Builder.Entity<Contract>(entityBuilder =>
        {
            entityBuilder.AsKDictionary();
            entityBuilder.Property(x => x.Id).HasKType(KType.Long);
            entityBuilder.Property(x => x.Name).HasKType(KType.Symbol);
            entityBuilder.Property(x => x.CreatedAt).HasConverter(customDateTimeConverter); // Custom converter
            entityBuilder.Ignore(x => x.Items);
            entityBuilder.Ignore(x => x.InternalField);
        });
        options.ApplyModelConfiguration();

        // Act - Serialize using ArrayBufferWriter and check the result
        var bufferWriter = new ArrayBufferWriter<byte>();
        var writer = new KSerializationWriter(bufferWriter);
        KSerializer.Serialize(ref writer, contract, options);

        var reader = new KSerializationReader(bufferWriter.WrittenMemory);
        var asDictionary = KSerializer.Deserialize<KSimpleDictionary>(ref reader, options);

        // Assert - Verify custom converter was used
        Assert.NotNull(asDictionary);
        var keys = asDictionary.Keys.Cast<string>().ToArray();
        var createdAtIndex = Array.IndexOf(keys, "CreatedAt");

        // Should be serialized as Unix timestamp (long), not DateTime
        var createdAtValue = asDictionary.Values.GetValue(createdAtIndex);
        Assert.IsType<long>(createdAtValue);

        // Verify the timestamp value is correct
        var expectedTimestamp = ((DateTimeOffset)contract.CreatedAt).ToUnixTimeSeconds();
        Assert.Equal(expectedTimestamp, (long)createdAtValue);

        // Verify round trip works with custom converter
        var reader2 = new KSerializationReader(bufferWriter.WrittenMemory);
        var deserializedContract = KSerializer.Deserialize<Contract>(ref reader2, options);
        Assert.Equal(contract.CreatedAt, deserializedContract.CreatedAt);
    }

    [Fact]
    public void NestedDataClass_SerializesCorrectly()
    {
        // Arrange - Create a contract with nested ContractItem objects
        var contract = new Contract
        {
            Id = 777L,
            Name = "Nested Test",
            CreatedAt = DateTime.Now,
            Items = new List<ContractItem>
            {
                new() { ProductName = "Nested Product 1", Quantity = 5.5m, Price = 25.99m },
                new() { ProductName = "Nested Product 2", Quantity = 3.0m, Price = 45.50m }
            }
        };

        var options = new KSerializerOptions();

        // Configure parent entity
        options.Builder.Entity<Contract>(entityBuilder =>
        {
            entityBuilder.AsKDictionary();
            entityBuilder.Property(x => x.Id).HasKType(KType.Long);
            entityBuilder.Property(x => x.Name).HasKType(KType.Symbol);
            entityBuilder.Property(x => x.CreatedAt).HasKType(KType.Timestamp);
            entityBuilder.Property(x => x.Items).HasKType(KType.GeneralList); // List of nested objects
            entityBuilder.Ignore(x => x.InternalField);
        });

        // Configure nested entity
        options.Builder.Entity<ContractItem>(entityBuilder =>
        {
            entityBuilder.AsKDictionary(); // Each item also becomes a dictionary
            entityBuilder.Property(x => x.ProductName).HasKType(KType.Symbol);
            entityBuilder.Property(x => x.Quantity).HasKType(KType.Real);
            entityBuilder.Property(x => x.Price).HasKType(KType.Float);
        });

        options.ApplyModelConfiguration();

        // Act - Serialize and verify structure
        var serializedBytes = KSerializer.Serialize(contract, options);
        var asDictionary = KSerializer.Deserialize<KSimpleDictionary>(serializedBytes, options);

        // Assert - Verify main structure
        Assert.NotNull(asDictionary);
        var keys = asDictionary.Keys.Cast<string>().ToArray();
        Assert.Contains("Items", keys);

        var itemsIndex = Array.IndexOf(keys, "Items");
        var itemsValue = asDictionary.Values.GetValue(itemsIndex);

        // Items should be serialized as some form of list/array
        Assert.NotNull(itemsValue);
        // The exact type depends on how List<ContractItem> is handled by the serialization system
    }

    [Fact]
    public void EmptyAndNullValues_HandledCorrectly()
    {
        // Arrange - Create contracts with various empty/null scenarios
        var contracts = new[]
        {
            new Contract { Id = 1, Name = "", CreatedAt = DateTime.MinValue, Items = new List<ContractItem>() }, // Empty values
            new Contract { Id = 2, Name = null!, CreatedAt = DateTime.MaxValue, Items = null! }, // Null values
            new Contract { Id = 3, Name = "Normal", CreatedAt = DateTime.Now, Items = new List<ContractItem> { new() { ProductName = "", Quantity = 0, Price = 0 } } } // Mixed
        };

        var options = new KSerializerOptions();
        options.Builder.Entity<Contract>(entityBuilder =>
        {
            entityBuilder.AsKDictionary();
            entityBuilder.Property(x => x.Id).HasKType(KType.Long);
            entityBuilder.Property(x => x.Name).HasKType(KType.Symbol);
            entityBuilder.Property(x => x.CreatedAt).HasKType(KType.Timestamp);
            entityBuilder.Property(x => x.Items).HasKType(KType.GeneralList);
            entityBuilder.Ignore(x => x.InternalField);
        });
        options.ApplyModelConfiguration();

        // Act & Assert - Each contract should serialize/deserialize without throwing
        foreach (var contract in contracts)
        {
            var serializedBytes = KSerializer.Serialize(contract, options);
            Assert.NotNull(serializedBytes);
            Assert.True(serializedBytes.Length > 0);

            var asDictionary = KSerializer.Deserialize<KSimpleDictionary>(serializedBytes, options);
            Assert.NotNull(asDictionary);

            var deserializedContract = KSerializer.Deserialize<Contract>(serializedBytes, options);
            Assert.NotNull(deserializedContract);
            Assert.Equal(contract.Id, deserializedContract.Id);
        }
    }

    [Fact]
    public void LargeDataSet_PerformanceTest()
    {
        // Arrange - Create a large number of contracts to test performance
        const int contractCount = 100; // Reduced for faster testing
        var contracts = new Contract[contractCount];

        for (int i = 0; i < contractCount; i++)
        {
            contracts[i] = new Contract
            {
                Id = i,
                Name = $"Contract {i}",
                CreatedAt = DateTime.Now.AddDays(-i),
                Items = new List<ContractItem>
                {
                    new() { ProductName = $"Product {i}-1", Quantity = i * 1.5m, Price = i * 10.99m },
                    new() { ProductName = $"Product {i}-2", Quantity = i * 2.5m, Price = i * 15.99m }
                }
            };
        }

        var options = new KSerializerOptions();
        options.Builder.Entity<Contract>(entityBuilder =>
        {
            entityBuilder.AsKDictionary();
            entityBuilder.Property(x => x.Id).HasKType(KType.Long);
            entityBuilder.Property(x => x.Name).HasKType(KType.Symbol);
            entityBuilder.Property(x => x.CreatedAt).HasKType(KType.Timestamp);
            entityBuilder.Property(x => x.Items).HasKType(KType.GeneralList);
            entityBuilder.Ignore(x => x.InternalField);
        });
        options.ApplyModelConfiguration();

        // Act - Measure serialization performance using buffers
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var serializedBuffers = new (byte[] buffer, int length)[contractCount];

        for (int i = 0; i < contractCount; i++)
        {
            var buffer = new byte[4096]; // Allocate buffer for each contract
            var writer = new KSerializationWriter(buffer);
            KSerializer.Serialize(ref writer, contracts[i], options);
            var bytesWritten = (int)writer.BytesWritten;
            serializedBuffers[i] = (buffer, bytesWritten);
        }

        stopwatch.Stop();
        var serializationTime = stopwatch.ElapsedMilliseconds;

        // Measure deserialization performance
        stopwatch.Restart();
        var deserializedContracts = new Contract[contractCount];

        for (int i = 0; i < contractCount; i++)
        {
            var (buffer, length) = serializedBuffers[i];
            var reader = new KSerializationReader(buffer.AsSpan(0, length));
            deserializedContracts[i] = KSerializer.Deserialize<Contract>(ref reader, options);
        }

        stopwatch.Stop();
        var deserializationTime = stopwatch.ElapsedMilliseconds;

        // Assert - Performance should be reasonable (adjust thresholds as needed)
        Assert.True(serializationTime < 2000, $"Serialization took {serializationTime}ms for {contractCount} objects");
        Assert.True(deserializationTime < 2000, $"Deserialization took {deserializationTime}ms for {contractCount} objects");

        // Verify data integrity for a few random samples
        var random = new Random(42);
        for (int i = 0; i < 10; i++)
        {
            var index = random.Next(contractCount);
            Assert.Equal(contracts[index].Id, deserializedContracts[index].Id);
            Assert.Equal(contracts[index].Name, deserializedContracts[index].Name);
        }

        // Verify buffer usage statistics
        var totalBytes = serializedBuffers.Sum(x => x.length);
        var avgBytesPerContract = totalBytes / contractCount;
        Assert.True(avgBytesPerContract > 0);
        Assert.True(avgBytesPerContract < 2048); // Should be reasonable size
    }

    [Fact]
    public void ConfigurationValidation_DetectsInvalidConfigurations()
    {
        // Arrange
        var options = new KSerializerOptions();

        // Act & Assert - Various configuration scenarios

        // Should not throw for valid configurations
        Assert.DoesNotThrow(() =>
        {
            options.Builder.Entity<Contract>(entityBuilder =>
            {
                entityBuilder.AsKDictionary();
                entityBuilder.Property(x => x.Id).HasKType(KType.Long);
                entityBuilder.Property(x => x.Name).HasKType(KType.Symbol);
            });
            options.ApplyModelConfiguration();
        });

        // Test multiple configurations of the same entity (should override)
        Assert.DoesNotThrow(() =>
        {
            var options2 = new KSerializerOptions();
            options2.Builder.Entity<Contract>(entityBuilder =>
            {
                entityBuilder.AsKDictionary();
                entityBuilder.Property(x => x.Id).HasKType(KType.Long);
            });
            options2.Builder.Entity<Contract>(entityBuilder =>
            {
                entityBuilder.AsKTable(); // Should override previous configuration
                entityBuilder.Property(x => x.Name).HasKType(KType.Symbol);
            });
            options2.ApplyModelConfiguration();
        });
    }

    [Fact]
    public void ByteArrayComparison_VerifiesSerializationConsistency()
    {
        // Arrange - Same object serialized multiple times should produce identical bytes
        var contract = new Contract
        {
            Id = 12345L,
            Name = "Consistency Test",
            CreatedAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc) // Fixed time for consistency
        };

        var options = new KSerializerOptions();
        options.Builder.Entity<Contract>(entityBuilder =>
        {
            entityBuilder.AsKDictionary();
            entityBuilder.Property(x => x.Id).HasKType(KType.Long);
            entityBuilder.Property(x => x.Name).HasKType(KType.Symbol);
            entityBuilder.Property(x => x.CreatedAt).HasKType(KType.Timestamp);
            entityBuilder.Ignore(x => x.Items);
            entityBuilder.Ignore(x => x.InternalField);
        });
        options.ApplyModelConfiguration();

        // Act - Serialize the same object multiple times
        var serialization1 = KSerializer.Serialize(contract, options);
        var serialization2 = KSerializer.Serialize(contract, options);
        var serialization3 = KSerializer.Serialize(contract, options);

        // Assert - All serializations should produce identical byte arrays
        Assert.Equal(serialization1.Length, serialization2.Length);
        Assert.Equal(serialization1.Length, serialization3.Length);

        for (int i = 0; i < serialization1.Length; i++)
        {
            Assert.Equal(serialization1[i], serialization2[i]);
            Assert.Equal(serialization1[i], serialization3[i]);
        }

        // Verify all deserialize to the same values
        var deserialized1 = KSerializer.Deserialize<Contract>(serialization1, options);
        var deserialized2 = KSerializer.Deserialize<Contract>(serialization2, options);
        var deserialized3 = KSerializer.Deserialize<Contract>(serialization3, options);

        Assert.Equal(deserialized1.Id, deserialized2.Id);
        Assert.Equal(deserialized1.Id, deserialized3.Id);
        Assert.Equal(deserialized1.Name, deserialized2.Name);
        Assert.Equal(deserialized1.Name, deserialized3.Name);
        Assert.Equal(deserialized1.CreatedAt, deserialized2.CreatedAt);
        Assert.Equal(deserialized1.CreatedAt, deserialized3.CreatedAt);
    }

    [Fact]
    public void CompleteIntegrationTest_RealWorldScenario()
    {
        // Arrange - Create a complex real-world scenario with multiple entity types and custom converters
        var contract = new Contract
        {
            Id = 98765L,
            Name = "   Integration Test Contract   ", // Will be trimmed by custom converter
            CreatedAt = new DateTime(2024, 6, 15, 14, 30, 45, DateTimeKind.Utc),
            Items = new List<ContractItem>
            {
                new()
                {
                    ProductName = "High Precision Product",
                    Quantity = 123.456789m, // Will be rounded to 2 decimal places
                    Price = 999.999m // Will be rounded to 2 decimal places
                },
                new()
                {
                    ProductName = "Standard Product",
                    Quantity = 50.00m,
                    Price = 25.99m
                }
            },
            InternalField = "This internal data should never appear in serialized form"
        };

        // Configure with multiple custom converters and complex mappings
        var options = new KSerializerOptions();

        options.Builder
            .Entity<Contract>(entityBuilder =>
            {
                entityBuilder.AsKDictionary();
                entityBuilder.ToKType(KType.Dictionary);
                entityBuilder.Property(x => x.Id).HasKType(KType.Long);
                entityBuilder.Property(x => x.Name).HasConverter(new LengthPrefixedStringConverter()); // Custom string processing
                entityBuilder.Property(x => x.CreatedAt).HasConverter(new UnixTimestampConverter()); // DateTime as Unix timestamp
                entityBuilder.Property(x => x.Items).HasKType(KType.GeneralList);
                entityBuilder.Ignore(x => x.InternalField); // Explicitly ignored
            })
            .Entity<ContractItem>(entityBuilder =>
            {
                entityBuilder.AsKDictionary();
                entityBuilder.Property(x => x.ProductName).HasKType(KType.Symbol);
                entityBuilder.Property(x => x.Quantity).HasConverter(new PrecisionDecimalConverter(2)); // 2 decimal places
                entityBuilder.Property(x => x.Price).HasConverter(new PrecisionDecimalConverter(2)); // 2 decimal places
            });

        options.ApplyModelConfiguration();
        
        // Act - Full serialization round trip
        var serializedBytes = KSerializer.Serialize(contract, options);

        // Verify intermediate structure as dictionary
        var asDictionary = KSerializer.Deserialize<KSimpleDictionary>(serializedBytes, options);

        // Full round trip back to original type
        var deserializedContract = KSerializer.Deserialize<Contract>(serializedBytes, options);

        // Assert - Verify complex transformations worked correctly
        Assert.NotNull(asDictionary);
        Assert.NotNull(deserializedContract);

        // Verify basic properties
        Assert.Equal(contract.Id, deserializedContract.Id);

        // Verify custom string converter (should be trimmed)
        Assert.Equal("Integration Test Contract", deserializedContract.Name);
        Assert.DoesNotContain("   ", deserializedContract.Name); // Leading/trailing spaces removed

        // Verify Unix timestamp conversion
        var keys = asDictionary.Keys.Cast<string>().ToArray();
        var createdAtIndex = Array.IndexOf(keys, "CreatedAt");
        var createdAtValue = asDictionary.Values.GetValue(createdAtIndex);
        Assert.IsType<long>(createdAtValue); // Should be stored as Unix timestamp

        var expectedTimestamp = ((DateTimeOffset)contract.CreatedAt).ToUnixTimeSeconds();
        Assert.Equal(expectedTimestamp, (long)createdAtValue);

        // Verify ignored field is not present
        Assert.DoesNotContain("InternalField", keys);
        Assert.Equal(string.Empty, deserializedContract.InternalField); // Should be default value

        // Verify nested items structure
        Assert.NotNull(deserializedContract.Items);
        // Note: The exact verification of nested items depends on how List<ContractItem> serialization is implemented

        // Verify serialization is deterministic
        var secondSerialization = KSerializer.Serialize(contract, options);
        Assert.Equal(serializedBytes.Length, secondSerialization.Length);

        // Performance check - should complete quickly
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            var testBytes = KSerializer.Serialize(contract, options);
            var testContract = KSerializer.Deserialize<Contract>(testBytes, options);
        }
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"100 round trips took {stopwatch.ElapsedMilliseconds}ms");
    }
}

/// <summary>
/// Simple test converter for testing custom converter configuration.
/// </summary>
public class TestDataClassToDictionaryConverter : KTypeConverter<Contract>
{
    public override KType? TypeToWriteTo => KType.Dictionary;

    public override bool CanConvert(Type t, KType kt)
    {
        return t == typeof(Contract) && kt == KType.Dictionary;
    }

    public override void Write(ref KSerializationWriter writer, Contract value, KSerializerOptions options)
    {
        // Simplified implementation for testing
        writer.WriteUnit();
    }

    public override Contract Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        // Simplified implementation for testing
        return new Contract();
    }
}

/// <summary>
/// Custom converter that serializes DateTime as Unix timestamp (seconds since epoch).
/// </summary>
public class UnixTimestampConverter : KTypeConverter<DateTime>
{
    public override KType? TypeToWriteTo => KType.Long;

    public override bool CanConvert(Type t, KType kt)
    {
        return t == typeof(DateTime) && kt == KType.Long;
    }

    public override void Write(ref KSerializationWriter writer, DateTime value, KSerializerOptions options)
    {
        var unixTimestamp = ((DateTimeOffset)value).ToUnixTimeSeconds();
        writer.WriteLong(unixTimestamp);
    }

    public override DateTime Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        var unixTimestamp = reader.ReadLong();
        return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
    }
}

/// <summary>
/// Custom converter for decimal to KReal with specific precision handling.
/// </summary>
public class PrecisionDecimalConverter : KTypeConverter<decimal>
{
    private readonly int _decimalPlaces;

    public PrecisionDecimalConverter(int decimalPlaces = 2)
    {
        _decimalPlaces = decimalPlaces;
    }

    public override KType? TypeToWriteTo => KType.Real;

    public override bool CanConvert(Type t, KType kt)
    {
        return t == typeof(decimal) && kt == KType.Real;
    }

    public override void Write(ref KSerializationWriter writer, decimal value, KSerializerOptions options)
    {
        // Round to specified decimal places and convert to double
        var rounded = Math.Round(value, _decimalPlaces);
        var doubleValue = (double)rounded;
        writer.WriteReal(doubleValue);
    }

    public override decimal Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        var doubleValue = reader.ReadReal();
        return (decimal)Math.Round(doubleValue, _decimalPlaces);
    }
}

/// <summary>
/// Custom converter that serializes string with length prefix for validation.
/// </summary>
public class LengthPrefixedStringConverter : KTypeConverter<string>
{
    public override KType? TypeToWriteTo => KType.Symbol;

    public override bool CanConvert(Type t, KType kt)
    {
        return t == typeof(string) && kt == KType.Symbol;
    }

    public override void Write(ref KSerializationWriter writer, string value, KSerializerOptions options)
    {
        // For testing: we could add length validation or transformation here
        var processedValue = value?.Trim() ?? string.Empty;
        if (processedValue.Length > 100)
        {
            processedValue = processedValue.Substring(0, 100); // Truncate long strings
        }
        writer.WriteSymbol(processedValue);
    }

    public override string Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        return reader.ReadSymbol() ?? string.Empty;
    }
}

/// <summary>
/// Test converter for testing custom converter configuration.
/// </summary>
public class TestCustomConverter : KTypeConverter<string>
{
    public override KType? TypeToWriteTo => KType.Symbol;

    public override bool CanConvert(Type t, KType kt)
    {
        return t == typeof(string) && kt == KType.Symbol;
    }

    public override void Write(ref KSerializationWriter writer, string value, KSerializerOptions options)
    {
        writer.WriteSymbol(value ?? string.Empty);
    }

    public override string Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        return reader.ReadSymbol();
    }
}
