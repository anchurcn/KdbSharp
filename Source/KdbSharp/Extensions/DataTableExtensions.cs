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
using System.Linq;
using KdbSharp.Types;

namespace KdbSharp.Extensions;

/// <summary>
/// Extension methods for converting KTable, KKeyedTable, and KSimpleDictionary to DataTable.
/// </summary>
public static class DataTableExtensions
{
    /// <summary>
    /// Converts a KTable to a DataTable.
    /// </summary>
    /// <param name="table">The KTable to convert.</param>
    /// <param name="tableName">The name for the DataTable. If null, defaults to "KTable".</param>
    /// <param name="options">Options for configuring the conversion behavior.</param>
    /// <returns>A DataTable containing the data from the KTable.</returns>
    public static DataTable ToDataTable(this KTable table, string? tableName = null, DataTableConversionOptions? options = null)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));

        options ??= DataTableConversionOptions.Default;
        var dataTable = new DataTable(tableName ?? "KTable");
        
        // Create columns
        for (int i = 0; i < table.ColumnCount; i++)
        {
            var column = table.Columns[i];
            var columnData = table.Data[i];
            
            // Get the element type from the array
            var elementType = columnData.GetType().GetElementType() ?? typeof(object);

            // Handle nullable types - DataTable doesn't support nullable types directly
            if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                elementType = Nullable.GetUnderlyingType(elementType) ?? typeof(object);
            }

            // For display purposes, convert char[] to string
            if (options.ForDisplay && elementType == typeof(char[]))
            {
                elementType = typeof(string);
            }

            var dataColumn = new DataColumn(column.Name, elementType);

            // Preserve KType information
            dataColumn.ExtendedProperties["KType"] = column.Type;
            dataColumn.ExtendedProperties["KTypeName"] = column.Type.ToString();
            
            dataTable.Columns.Add(dataColumn);
        }
        
        // Add rows
        dataTable.BeginLoadData();
        try
        {
            for (int rowIndex = 0; rowIndex < table.RowCount; rowIndex++)
            {
                var row = dataTable.NewRow();
                for (int colIndex = 0; colIndex < table.ColumnCount; colIndex++)
                {
                    var value = table.Data[colIndex].GetValue(rowIndex);

                    // Convert char[] to string for display purposes
                    if (options.ForDisplay && value is char[] charArray)
                    {
                        value = new string(charArray);
                    }

                    row[colIndex] = value ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }
        }
        finally
        {
            dataTable.EndLoadData();
        }
        
        return dataTable;
    }

    /// <summary>
    /// Converts a KKeyedTable to a DataTable.
    /// </summary>
    /// <param name="keyedTable">The KKeyedTable to convert.</param>
    /// <param name="tableName">The name for the DataTable. If null, defaults to "KKeyedTable".</param>
    /// <param name="options">Options for configuring the conversion behavior.</param>
    /// <returns>A DataTable containing the data from the KKeyedTable.</returns>
    public static DataTable ToDataTable(this KKeyedTable keyedTable, string? tableName = null, DataTableConversionOptions? options = null)
    {
        if (keyedTable == null)
            throw new ArgumentNullException(nameof(keyedTable));

        options ??= DataTableConversionOptions.Default;
        var dataTable = new DataTable(tableName ?? "KKeyedTable");
        
        // Collect all column names to handle duplicates
        var allColumnNames = new List<string>();

        // Always include key columns
        allColumnNames.AddRange(keyedTable.Keys.Columns.Select(c => c.Name));
        allColumnNames.AddRange(keyedTable.Values.Columns.Select(c => c.Name));
        
        // Resolve duplicate column names by appending numbers
        var resolvedColumnNames = ResolveDuplicateColumnNames(allColumnNames);
        
        int columnIndex = 0;

        // Add key columns
        for (int i = 0; i < keyedTable.Keys.ColumnCount; i++)
        {
            var column = keyedTable.Keys.Columns[i];
            var columnData = keyedTable.Keys.Data[i];
            var elementType = columnData.GetType().GetElementType() ?? typeof(object);

            // Handle nullable types - DataTable doesn't support nullable types directly
            if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                elementType = Nullable.GetUnderlyingType(elementType) ?? typeof(object);
            }

            // For display purposes, convert char[] to string
            if (options.ForDisplay && elementType == typeof(char[]))
            {
                elementType = typeof(string);
            }

            var dataColumn = new DataColumn(resolvedColumnNames[columnIndex], elementType);

            dataColumn.ExtendedProperties["KType"] = column.Type;
            dataColumn.ExtendedProperties["KTypeName"] = column.Type.ToString();
            dataColumn.ExtendedProperties["IsKeyColumn"] = true;

            dataTable.Columns.Add(dataColumn);
            columnIndex++;
        }
        
        // Add value columns
        for (int i = 0; i < keyedTable.Values.ColumnCount; i++)
        {
            var column = keyedTable.Values.Columns[i];
            var columnData = keyedTable.Values.Data[i];
            var elementType = columnData.GetType().GetElementType() ?? typeof(object);

            // Handle nullable types - DataTable doesn't support nullable types directly
            if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                elementType = Nullable.GetUnderlyingType(elementType) ?? typeof(object);
            }

            // For display purposes, convert char[] to string
            if (options.ForDisplay && elementType == typeof(char[]))
            {
                elementType = typeof(string);
            }

            var dataColumn = new DataColumn(resolvedColumnNames[columnIndex], elementType);

            dataColumn.ExtendedProperties["KType"] = column.Type;
            dataColumn.ExtendedProperties["KTypeName"] = column.Type.ToString();
            dataColumn.ExtendedProperties["IsKeyColumn"] = false;
            
            dataTable.Columns.Add(dataColumn);
            columnIndex++;
        }
        
        // Add rows
        dataTable.BeginLoadData();
        try
        {
            for (int rowIndex = 0; rowIndex < keyedTable.RowCount; rowIndex++)
            {
                var row = dataTable.NewRow();
                int dataColumnIndex = 0;

                // Add key column values
                for (int keyColIndex = 0; keyColIndex < keyedTable.Keys.ColumnCount; keyColIndex++)
                {
                    var value = keyedTable.Keys.Data[keyColIndex].GetValue(rowIndex);

                    // Convert char[] to string for display purposes
                    if (options.ForDisplay && value is char[] keyCharArray)
                    {
                        value = new string(keyCharArray);
                    }

                    row[dataColumnIndex] = value ?? DBNull.Value;
                    dataColumnIndex++;
                }

                // Add value column values
                for (int valueColIndex = 0; valueColIndex < keyedTable.Values.ColumnCount; valueColIndex++)
                {
                    var value = keyedTable.Values.Data[valueColIndex].GetValue(rowIndex);

                    // Convert char[] to string for display purposes
                    if (options.ForDisplay && value is char[] valueCharArray)
                    {
                        value = new string(valueCharArray);
                    }

                    row[dataColumnIndex] = value ?? DBNull.Value;
                    dataColumnIndex++;
                }
                
                dataTable.Rows.Add(row);
            }
        }
        finally
        {
            dataTable.EndLoadData();
        }
        
        return dataTable;
    }

    /// <summary>
    /// Resolves duplicate column names by appending sequential numbers.
    /// </summary>
    /// <param name="columnNames">The original column names.</param>
    /// <returns>A list of resolved column names with no duplicates.</returns>
    private static List<string> ResolveDuplicateColumnNames(List<string> columnNames)
    {
        var resolved = new List<string>();
        var nameCount = new Dictionary<string, int>();
        
        foreach (var name in columnNames)
        {
            if (!nameCount.ContainsKey(name))
            {
                nameCount[name] = 0;
                resolved.Add(name);
            }
            else
            {
                nameCount[name]++;
                resolved.Add($"{name}_{nameCount[name]}");
            }
        }
        
        return resolved;
    }

    /// <summary>
    /// Converts a KSimpleDictionary to a DataTable.
    /// </summary>
    /// <param name="dictionary">The KSimpleDictionary to convert.</param>
    /// <param name="tableName">The name for the DataTable. If null, defaults to "KSimpleDictionary".</param>
    /// <param name="options">Options for configuring the conversion behavior.</param>
    /// <returns>A DataTable containing the key-value pairs from the KSimpleDictionary.</returns>
    public static DataTable ToDataTable(this KSimpleDictionary dictionary, string? tableName = null, DataTableConversionOptions? options = null)
    {
        if (dictionary == null)
            throw new ArgumentNullException(nameof(dictionary));

        options ??= DataTableConversionOptions.Default;
        var dataTable = new DataTable(tableName ?? "KSimpleDictionary");

        // Determine the types for keys and values
        var keyElementType = dictionary.Keys.GetType().GetElementType() ?? typeof(object);
        var valueElementType = dictionary.Values.GetType().GetElementType() ?? typeof(object);

        // Handle nullable types - DataTable doesn't support nullable types directly
        if (keyElementType.IsGenericType && keyElementType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            keyElementType = Nullable.GetUnderlyingType(keyElementType) ?? typeof(object);
        }

        if (valueElementType.IsGenericType && valueElementType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            valueElementType = Nullable.GetUnderlyingType(valueElementType) ?? typeof(object);
        }

        // For display purposes, convert char[] to string
        if (options.ForDisplay && keyElementType == typeof(char[]))
        {
            keyElementType = typeof(string);
        }

        if (options.ForDisplay && valueElementType == typeof(char[]))
        {
            valueElementType = typeof(string);
        }

        // Create columns
        var keyColumn = new DataColumn("Key", keyElementType);
        var valueColumn = new DataColumn("Value", valueElementType);

        // Add metadata about the original array types
        keyColumn.ExtendedProperties["OriginalType"] = dictionary.Keys.GetType();
        valueColumn.ExtendedProperties["OriginalType"] = dictionary.Values.GetType();

        dataTable.Columns.Add(keyColumn);
        dataTable.Columns.Add(valueColumn);

        // Add rows
        dataTable.BeginLoadData();
        try
        {
            for (int i = 0; i < dictionary.Keys.Length; i++)
            {
                var row = dataTable.NewRow();

                var keyValue = dictionary.Keys.GetValue(i);
                var valueValue = dictionary.Values.GetValue(i);

                // Convert char[] to string for display purposes
                if (options.ForDisplay && keyValue is char[] keyCharArray)
                {
                    keyValue = new string(keyCharArray);
                }

                if (options.ForDisplay && valueValue is char[] valueCharArray)
                {
                    valueValue = new string(valueCharArray);
                }

                row["Key"] = keyValue ?? DBNull.Value;
                row["Value"] = valueValue ?? DBNull.Value;

                dataTable.Rows.Add(row);
            }
        }
        finally
        {
            dataTable.EndLoadData();
        }

        return dataTable;
    }
}
