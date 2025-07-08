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
using System.Data;
using KdbSharp.Extensions;
using KdbSharp.Types;
using Xunit;

namespace KdbSharp.Test;

public class DataTableExtensionsTest
{
    [Fact]
    public void KTable_ToDataTable_BasicTest()
    {
        // Arrange
        var columns = new[]
        {
            new KTable.Column(KType.Symbol, "name"),
            new KTable.Column(KType.Int, "age"),
            new KTable.Column(KType.Float, "salary")
        };
        
        var data = new Array[]
        {
            new string[] { "John", "Jane", "Bob" },
            new int[] { 25, 30, 35 },
            new double[] { 50000.0, 60000.0, 55000.0 }
        };
        
        var kTable = new KTable(columns, data);
        
        // Act
        var dataTable = kTable.ToDataTable();
        
        // Assert
        Assert.Equal("KTable", dataTable.TableName);
        Assert.Equal(3, dataTable.Columns.Count);
        Assert.Equal(3, dataTable.Rows.Count);
        
        // Check column types
        Assert.Equal(typeof(string), dataTable.Columns["name"].DataType);
        Assert.Equal(typeof(int), dataTable.Columns["age"].DataType);
        Assert.Equal(typeof(double), dataTable.Columns["salary"].DataType);
        
        // Check KType preservation
        Assert.Equal(KType.Symbol, dataTable.Columns["name"].ExtendedProperties["KType"]);
        Assert.Equal(KType.Int, dataTable.Columns["age"].ExtendedProperties["KType"]);
        Assert.Equal(KType.Float, dataTable.Columns["salary"].ExtendedProperties["KType"]);
        
        // Check data
        Assert.Equal("John", dataTable.Rows[0]["name"]);
        Assert.Equal(25, dataTable.Rows[0]["age"]);
        Assert.Equal(50000.0, dataTable.Rows[0]["salary"]);
        
        Assert.Equal("Jane", dataTable.Rows[1]["name"]);
        Assert.Equal(30, dataTable.Rows[1]["age"]);
        Assert.Equal(60000.0, dataTable.Rows[1]["salary"]);
    }
    
    [Fact]
    public void KTable_ToDataTable_WithCustomTableName()
    {
        // Arrange
        var columns = new[]
        {
            new KTable.Column(KType.Symbol, "symbol"),
            new KTable.Column(KType.Real, "price")
        };
        
        var data = new Array[]
        {
            new string[] { "AAPL", "GOOGL" },
            new float[] { 150.5f, 2800.0f }
        };
        
        var kTable = new KTable(columns, data);
        
        // Act
        var dataTable = kTable.ToDataTable("StockPrices");
        
        // Assert
        Assert.Equal("StockPrices", dataTable.TableName);
        Assert.Equal(2, dataTable.Columns.Count);
        Assert.Equal(2, dataTable.Rows.Count);
    }
    
    [Fact]
    public void KTable_ToDataTable_WithNullValues()
    {
        // Arrange
        var columns = new[]
        {
            new KTable.Column(KType.Symbol, "name"),
            new KTable.Column(KType.Boolean, "active")
        };

        var data = new Array[]
        {
            new string[] { "John", null, "Bob" },
            new bool[] { true, false, true }
        };

        var kTable = new KTable(columns, data);

        // Act
        var dataTable = kTable.ToDataTable();

        // Assert
        Assert.Equal("John", dataTable.Rows[0]["name"]);
        Assert.Equal(DBNull.Value, dataTable.Rows[1]["name"]);
        Assert.Equal("Bob", dataTable.Rows[2]["name"]);

        Assert.Equal(true, dataTable.Rows[0]["active"]);
        Assert.Equal(false, dataTable.Rows[1]["active"]);
        Assert.Equal(true, dataTable.Rows[2]["active"]);
    }
    
    [Fact]
    public void KKeyedTable_ToDataTable_BasicTest()
    {
        // Arrange - Create Keys table
        var keyColumns = new[]
        {
            new KTable.Column(KType.Int, "id"),
            new KTable.Column(KType.Symbol, "category")
        };
        
        var keyData = new Array[]
        {
            new int[] { 1, 2, 3 },
            new string[] { "A", "B", "C" }
        };
        
        var keysTable = new KTable(keyColumns, keyData);
        
        // Arrange - Create Values table
        var valueColumns = new[]
        {
            new KTable.Column(KType.Symbol, "name"),
            new KTable.Column(KType.Float, "score")
        };
        
        var valueData = new Array[]
        {
            new string[] { "John", "Jane", "Bob" },
            new double[] { 85.5, 92.0, 78.5 }
        };
        
        var valuesTable = new KTable(valueColumns, valueData);
        
        var keyedTable = new KKeyedTable(keysTable, valuesTable);
        
        // Act
        var dataTable = keyedTable.ToDataTable();
        
        // Assert
        Assert.Equal("KKeyedTable", dataTable.TableName);
        Assert.Equal(4, dataTable.Columns.Count); // 2 key + 2 value columns
        Assert.Equal(3, dataTable.Rows.Count);
        
        // Check column names and types
        Assert.Equal("id", dataTable.Columns[0].ColumnName);
        Assert.Equal("category", dataTable.Columns[1].ColumnName);
        Assert.Equal("name", dataTable.Columns[2].ColumnName);
        Assert.Equal("score", dataTable.Columns[3].ColumnName);
        
        Assert.Equal(typeof(int), dataTable.Columns["id"].DataType);
        Assert.Equal(typeof(string), dataTable.Columns["category"].DataType);
        Assert.Equal(typeof(string), dataTable.Columns["name"].DataType);
        Assert.Equal(typeof(double), dataTable.Columns["score"].DataType);
        
        // Check IsKeyColumn property
        Assert.Equal(true, dataTable.Columns["id"].ExtendedProperties["IsKeyColumn"]);
        Assert.Equal(true, dataTable.Columns["category"].ExtendedProperties["IsKeyColumn"]);
        Assert.Equal(false, dataTable.Columns["name"].ExtendedProperties["IsKeyColumn"]);
        Assert.Equal(false, dataTable.Columns["score"].ExtendedProperties["IsKeyColumn"]);
        
        // Check data
        Assert.Equal(1, dataTable.Rows[0]["id"]);
        Assert.Equal("A", dataTable.Rows[0]["category"]);
        Assert.Equal("John", dataTable.Rows[0]["name"]);
        Assert.Equal(85.5, dataTable.Rows[0]["score"]);
    }
    
    [Fact]
    public void KKeyedTable_ToDataTable_WithDuplicateColumnNames()
    {
        // Arrange - Create Keys table with "name" column
        var keyColumns = new[]
        {
            new KTable.Column(KType.Symbol, "name")
        };
        
        var keyData = new Array[]
        {
            new string[] { "Key1", "Key2" }
        };
        
        var keysTable = new KTable(keyColumns, keyData);
        
        // Arrange - Create Values table with "name" column (duplicate)
        var valueColumns = new[]
        {
            new KTable.Column(KType.Symbol, "name"),
            new KTable.Column(KType.Int, "value")
        };
        
        var valueData = new Array[]
        {
            new string[] { "Value1", "Value2" },
            new int[] { 100, 200 }
        };
        
        var valuesTable = new KTable(valueColumns, valueData);
        
        var keyedTable = new KKeyedTable(keysTable, valuesTable);
        
        // Act
        var dataTable = keyedTable.ToDataTable("TestTable");
        
        // Assert
        Assert.Equal("TestTable", dataTable.TableName);
        Assert.Equal(3, dataTable.Columns.Count);
        
        // Check that duplicate column names are resolved
        Assert.Equal("name", dataTable.Columns[0].ColumnName);
        Assert.Equal("name_1", dataTable.Columns[1].ColumnName);
        Assert.Equal("value", dataTable.Columns[2].ColumnName);
        
        // Check data
        Assert.Equal("Key1", dataTable.Rows[0]["name"]);     // Key column
        Assert.Equal("Value1", dataTable.Rows[0]["name_1"]); // Value column
        Assert.Equal(100, dataTable.Rows[0]["value"]);
    }
    
    [Fact]
    public void KTable_ToDataTable_ThrowsOnNullTable()
    {
        // Arrange
        KTable nullTable = null;
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => nullTable.ToDataTable());
    }
    
    [Fact]
    public void KKeyedTable_ToDataTable_ThrowsOnNullTable()
    {
        // Arrange
        KKeyedTable nullTable = null;
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => nullTable.ToDataTable());
    }
}
