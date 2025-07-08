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
/// Extension methods for converting KTable and KKeyedTable to DataTable.
/// </summary>
public static class DataTableExtensions
{
    /// <summary>
    /// Converts a KTable to a DataTable.
    /// </summary>
    /// <param name="table">The KTable to convert.</param>
    /// <param name="tableName">The name for the DataTable. If null, defaults to "KTable".</param>
    /// <returns>A DataTable containing the data from the KTable.</returns>
    public static DataTable ToDataTable(this KTable table, string? tableName = null)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));

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
    /// <returns>A DataTable containing the data from the KKeyedTable.</returns>
    public static DataTable ToDataTable(this KKeyedTable keyedTable, string? tableName = null)
    {
        if (keyedTable == null)
            throw new ArgumentNullException(nameof(keyedTable));

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
                    row[dataColumnIndex] = value ?? DBNull.Value;
                    dataColumnIndex++;
                }
                
                // Add value column values
                for (int valueColIndex = 0; valueColIndex < keyedTable.Values.ColumnCount; valueColIndex++)
                {
                    var value = keyedTable.Values.Data[valueColIndex].GetValue(rowIndex);
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
}
