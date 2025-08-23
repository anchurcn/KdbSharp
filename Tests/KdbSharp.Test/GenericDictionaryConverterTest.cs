/*
 * Test cases for KdbSharp generic Dictionary<TKey, TValue> serialization
 */

using System;
using System.Collections.Generic;
using Xunit;
using KdbSharp;
using KdbSharp.Serialization;
using KdbSharp.Serialization.Converters;
using KdbSharp.Types;

namespace KdbSharp.Test;

public class GenericDictionaryConverterTest
{
    [Fact]
    public void Test_StringIntDictionary_RoundTrip()
    {
        // Arrange
        var originalDict = new Dictionary<string, int>
        {
            ["apple"] = 1,
            ["banana"] = 2,
            ["cherry"] = 3
        };

        // Act - Serialize
        var serialized = KSerializer.Serialize(originalDict, KType.Dictionary);

        // Act - Deserialize
        var reader = new KSerializationReader(serialized) { IsLittleEndian = true };
        var deserializedDict = KSerializer.Deserialize<Dictionary<string, int>>(ref reader);

        // Assert
        Assert.NotNull(deserializedDict);
        Assert.Equal(originalDict.Count, deserializedDict.Count);
        
        foreach (var kvp in originalDict)
        {
            Assert.True(deserializedDict.ContainsKey(kvp.Key));
            Assert.Equal(kvp.Value, deserializedDict[kvp.Key]);
        }
    }

    [Fact]
    public void Test_IntStringDictionary_RoundTrip()
    {
        // Arrange
        var originalDict = new Dictionary<int, string>
        {
            [1] = "one",
            [2] = "two",
            [3] = "three"
        };

        // Act - Serialize
        var serialized = KSerializer.Serialize(originalDict, KType.Dictionary);

        // Act - Deserialize
        var reader = new KSerializationReader(serialized) { IsLittleEndian = true };
        var deserializedDict = KSerializer.Deserialize<Dictionary<int, string>>(ref reader);

        // Assert
        Assert.NotNull(deserializedDict);
        Assert.Equal(originalDict.Count, deserializedDict.Count);
        
        foreach (var kvp in originalDict)
        {
            Assert.True(deserializedDict.ContainsKey(kvp.Key));
            Assert.Equal(kvp.Value, deserializedDict[kvp.Key]);
        }
    }

    [Fact]
    public void Test_EmptyDictionary_RoundTrip()
    {
        // Arrange
        var originalDict = new Dictionary<string, int>();

        // Act - Serialize
        var serialized = KSerializer.Serialize(originalDict, KType.Dictionary);

        // Act - Deserialize
        var reader = new KSerializationReader(serialized) { IsLittleEndian = true };
        var deserializedDict = KSerializer.Deserialize<Dictionary<string, int>>(ref reader);

        // Assert
        Assert.NotNull(deserializedDict);
        Assert.Empty(deserializedDict);
    }

    [Fact]
    public void Test_DoubleDoubleDictionary_RoundTrip()
    {
        // Arrange
        var originalDict = new Dictionary<double, double>
        {
            [1.1] = 2.2,
            [3.3] = 4.4,
            [5.5] = 6.6
        };

        // Act - Serialize
        var serialized = KSerializer.Serialize(originalDict, KType.Dictionary);

        // Act - Deserialize
        var reader = new KSerializationReader(serialized) { IsLittleEndian = true };
        var deserializedDict = KSerializer.Deserialize<Dictionary<double, double>>(ref reader);

        // Assert
        Assert.NotNull(deserializedDict);
        Assert.Equal(originalDict.Count, deserializedDict.Count);
        
        foreach (var kvp in originalDict)
        {
            Assert.True(deserializedDict.ContainsKey(kvp.Key));
            Assert.Equal(kvp.Value, deserializedDict[kvp.Key], 10); // Allow for floating point precision
        }
    }

    [Fact]
    public void Test_StringArray_Serialization()
    {
        // Test if string[] can be serialized directly to SymbolList
        var keys = new string[] { "a", "b", "c" };
        var serialized = KSerializer.Serialize(keys, KType.SymbolList);
        Assert.NotNull(serialized);
        Assert.True(serialized.Length > 0);

        // Also test deserialization
        var reader = new KSerializationReader(serialized) { IsLittleEndian = true };
        var deserialized = KSerializer.Deserialize<string[]>(ref reader);
        Assert.NotNull(deserialized);
        Assert.Equal(3, deserialized.Length);
        Assert.Equal("a", deserialized[0]);
        Assert.Equal("b", deserialized[1]);
        Assert.Equal("c", deserialized[2]);
    }

    [Fact]
    public void Test_IntArray_Serialization()
    {
        // Test if int[] can be serialized
        var values = new int[] { 1, 2, 3 };
        var serialized = KSerializer.Serialize(values, KType.IntList);
        Assert.NotNull(serialized);
        Assert.True(serialized.Length > 0);
    }

    [Fact]
    public void Test_KAtomListConverterFactory_CanConvert()
    {
        // Test if KAtomListConverterFactory can handle string[] and KType.SymbolList
        var factory = new KAtomListConverterFactory();
        var canConvert = factory.CanConvert(typeof(string[]), KType.SymbolList);
        Assert.True(canConvert);

        // Test if it can create a converter
        var options = new KSerializerOptions();
        var converter = factory.GetConverter(typeof(string[]), KType.SymbolList, options);
        Assert.NotNull(converter);
    }

    [Fact]
    public void Test_KSimpleDictionary_ToDictionary_Conversion()
    {
        // Arrange - Create a KSimpleDictionary
        var keys = new string[] { "a", "b", "c" };
        var values = new int[] { 1, 2, 3 };
        var kDict = new KSimpleDictionary(keys, values);

        // Act - First test that KSimpleDictionary can be serialized
        var serialized = KSerializer.Serialize(kDict, KType.Dictionary);
        Assert.NotNull(serialized);
        Assert.True(serialized.Length > 0);

        // Then deserialize as Dictionary<string, int>
        var reader = new KSerializationReader(serialized) { IsLittleEndian = true };
        var genericDict = KSerializer.Deserialize<Dictionary<string, int>>(ref reader);

        // Assert
        Assert.NotNull(genericDict);
        Assert.Equal(3, genericDict.Count);
        Assert.Equal(1, genericDict["a"]);
        Assert.Equal(2, genericDict["b"]);
        Assert.Equal(3, genericDict["c"]);
    }

    [Fact]
    public void Test_Dictionary_ToKSimpleDictionary_Conversion()
    {
        // Arrange
        var genericDict = new Dictionary<string, int>
        {
            ["x"] = 10,
            ["y"] = 20,
            ["z"] = 30
        };

        // Act - Serialize Dictionary and deserialize as KSimpleDictionary
        var serialized = KSerializer.Serialize(genericDict, KType.Dictionary);
        var reader = new KSerializationReader(serialized) { IsLittleEndian = true };
        var kDict = KSerializer.Deserialize<KSimpleDictionary>(ref reader);

        // Assert
        Assert.NotNull(kDict);
        Assert.Equal(3, kDict.Keys.Length);
        Assert.Equal(3, kDict.Values.Length);
        
        // Convert back to Dictionary for easier verification
        var convertedDict = kDict.ToDictionary<string, int>();
        Assert.Equal(10, convertedDict["x"]);
        Assert.Equal(20, convertedDict["y"]);
        Assert.Equal(30, convertedDict["z"]);
    }
}
