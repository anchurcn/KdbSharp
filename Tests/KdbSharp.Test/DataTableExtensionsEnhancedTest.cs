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

using System.Data;
using KdbSharp.Extensions;
using KdbSharp.Types;
using Xunit;

namespace KdbSharp.Test;

/// <summary>
/// Tests for enhanced DataTable extension methods including KSimpleDictionary support and ForDisplay options.
/// </summary>
public class DataTableExtensionsEnhancedTest
{
    [Fact]
    public void KSimpleDictionary_ToDataTable_BasicConversion_ShouldWork()
    {
        // Arrange
        var keys = new[] { "key1", "key2", "key3" };
        var values = new[] { 1, 2, 3 };
        var dictionary = new KSimpleDictionary(keys, values);

        // Act
        var dataTable = dictionary.ToDataTable();

        // Assert
        Assert.Equal("KSimpleDictionary", dataTable.TableName);
        Assert.Equal(2, dataTable.Columns.Count);
        Assert.Equal("Key", dataTable.Columns[0].ColumnName);
        Assert.Equal("Value", dataTable.Columns[1].ColumnName);
        Assert.Equal(typeof(string), dataTable.Columns[0].DataType);
        Assert.Equal(typeof(int), dataTable.Columns[1].DataType);
        Assert.Equal(3, dataTable.Rows.Count);

        // Check data
        Assert.Equal("key1", dataTable.Rows[0]["Key"]);
        Assert.Equal(1, dataTable.Rows[0]["Value"]);
        Assert.Equal("key2", dataTable.Rows[1]["Key"]);
        Assert.Equal(2, dataTable.Rows[1]["Value"]);
        Assert.Equal("key3", dataTable.Rows[2]["Key"]);
        Assert.Equal(3, dataTable.Rows[2]["Value"]);
    }

    [Fact]
    public void KSimpleDictionary_ToDataTable_WithCustomTableName_ShouldWork()
    {
        // Arrange
        var keys = new[] { 1, 2 };
        var values = new[] { "a", "b" };
        var dictionary = new KSimpleDictionary(keys, values);

        // Act
        var dataTable = dictionary.ToDataTable("CustomDictionary");

        // Assert
        Assert.Equal("CustomDictionary", dataTable.TableName);
        Assert.Equal(2, dataTable.Columns.Count);
        Assert.Equal(2, dataTable.Rows.Count);
    }

    [Fact]
    public void KSimpleDictionary_ToDataTable_WithCharArrays_ForDisplay_ShouldConvertToString()
    {
        // Arrange
        var keys = new[] { new char[] { 'k', '1' }, new char[] { 'k', '2' } };
        var values = new[] { new char[] { 'v', '1' }, new char[] { 'v', '2' } };
        var dictionary = new KSimpleDictionary(keys, values);

        // Act
        var dataTable = dictionary.ToDataTable(options: DataTableConversionOptions.ForDisplayPurpose);

        // Assert
        Assert.Equal(typeof(string), dataTable.Columns["Key"].DataType);
        Assert.Equal(typeof(string), dataTable.Columns["Value"].DataType);
        Assert.Equal("k1", dataTable.Rows[0]["Key"]);
        Assert.Equal("v1", dataTable.Rows[0]["Value"]);
        Assert.Equal("k2", dataTable.Rows[1]["Key"]);
        Assert.Equal("v2", dataTable.Rows[1]["Value"]);
    }

    [Fact]
    public void KSimpleDictionary_ToDataTable_WithCharArrays_NoForDisplay_ShouldKeepCharArrays()
    {
        // Arrange
        var keys = new[] { new char[] { 'k', '1' }, new char[] { 'k', '2' } };
        var values = new[] { new char[] { 'v', '1' }, new char[] { 'v', '2' } };
        var dictionary = new KSimpleDictionary(keys, values);

        // Act
        var dataTable = dictionary.ToDataTable();

        // Assert
        Assert.Equal(typeof(char[]), dataTable.Columns["Key"].DataType);
        Assert.Equal(typeof(char[]), dataTable.Columns["Value"].DataType);
        Assert.IsType<char[]>(dataTable.Rows[0]["Key"]);
        Assert.IsType<char[]>(dataTable.Rows[0]["Value"]);
    }

    [Fact]
    public void KTable_ToDataTable_WithCharArrayColumn_ForDisplay_ShouldConvertToString()
    {
        // Arrange
        var columns = new[]
        {
            new KTable.Column(KType.CharList, "name"),
            new KTable.Column(KType.Int, "age")
        };
        var data = new Array[]
        {
            new char[][] { new char[] { 'J', 'o', 'h', 'n' }, new char[] { 'J', 'a', 'n', 'e' } },
            new int[] { 25, 30 }
        };
        var table = new KTable(columns, data);

        // Act
        var dataTable = table.ToDataTable(options: DataTableConversionOptions.ForDisplayPurpose);

        // Assert
        Assert.Equal(typeof(string), dataTable.Columns["name"].DataType);
        Assert.Equal(typeof(int), dataTable.Columns["age"].DataType);
        Assert.Equal("John", dataTable.Rows[0]["name"]);
        Assert.Equal("Jane", dataTable.Rows[1]["name"]);
        Assert.Equal(25, dataTable.Rows[0]["age"]);
        Assert.Equal(30, dataTable.Rows[1]["age"]);
    }

    [Fact]
    public void KKeyedTable_ToDataTable_WithCharArrayColumns_ForDisplay_ShouldConvertToString()
    {
        // Arrange
        var keyColumns = new[]
        {
            new KTable.Column(KType.CharList, "id")
        };
        var keyData = new Array[]
        {
            new char[][] { new char[] { 'A' }, new char[] { 'B' } }
        };
        var keys = new KTable(keyColumns, keyData);

        var valueColumns = new[]
        {
            new KTable.Column(KType.CharList, "description")
        };
        var valueData = new Array[]
        {
            new char[][] { new char[] { 'D', 'e', 's', 'c', '1' }, new char[] { 'D', 'e', 's', 'c', '2' } }
        };
        var values = new KTable(valueColumns, valueData);

        var keyedTable = new KKeyedTable(keys, values);

        // Act
        var dataTable = keyedTable.ToDataTable(options: DataTableConversionOptions.ForDisplayPurpose);

        // Assert
        Assert.Equal(typeof(string), dataTable.Columns["id"].DataType);
        Assert.Equal(typeof(string), dataTable.Columns["description"].DataType);
        Assert.Equal("A", dataTable.Rows[0]["id"]);
        Assert.Equal("Desc1", dataTable.Rows[0]["description"]);
        Assert.Equal("B", dataTable.Rows[1]["id"]);
        Assert.Equal("Desc2", dataTable.Rows[1]["description"]);
    }

    [Fact]
    public void DataTableConversionOptions_Default_ShouldHaveForDisplayFalse()
    {
        // Act
        var options = DataTableConversionOptions.Default;

        // Assert
        Assert.False(options.ForDisplay);
    }

    [Fact]
    public void DataTableConversionOptions_ForDisplayPurpose_ShouldHaveForDisplayTrue()
    {
        // Act
        var options = DataTableConversionOptions.ForDisplayPurpose;

        // Assert
        Assert.True(options.ForDisplay);
    }

    [Fact]
    public void KSimpleDictionary_ToDataTable_WithNullValues_ShouldHandleCorrectly()
    {
        // Arrange
        var keys = new object[] { "key1", "key2" };
        var values = new object[] { "value1", null };
        var dictionary = new KSimpleDictionary(keys, values);

        // Act
        var dataTable = dictionary.ToDataTable();

        // Assert
        Assert.Equal("value1", dataTable.Rows[0]["Value"]);
        Assert.Equal(DBNull.Value, dataTable.Rows[1]["Value"]);
    }

    [Fact]
    public void KSimpleDictionary_ToDataTable_WithMetadata_ShouldPreserveOriginalTypes()
    {
        // Arrange
        var keys = new[] { 1, 2 };
        var values = new[] { "a", "b" };
        var dictionary = new KSimpleDictionary(keys, values);

        // Act
        var dataTable = dictionary.ToDataTable();

        // Assert
        Assert.Equal(typeof(int[]), dataTable.Columns["Key"].ExtendedProperties["OriginalType"]);
        Assert.Equal(typeof(string[]), dataTable.Columns["Value"].ExtendedProperties["OriginalType"]);
    }
}
