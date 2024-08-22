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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KdbSharp.Types;

public class KTable : IIndexable, IEnumerable<KTable.Row>, IKTable
{
    /// <summary>
    /// Represents a column in the table.
    /// </summary>
    public readonly struct Column
    {
        /// <summary>
        /// Gets or sets the type of the column.
        /// </summary>
        public readonly KType Type;

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        public readonly string Name;
        public Column(KType type, string name)
        {
            Type = type;
            Name = name;
        }
    }

    public KTable(Column[] columns, Array[] data)
    {
        // Null check
        if (columns is null)
        {
            throw new ArgumentNullException(nameof(columns));
        }
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        // Validation
        if (columns.Length == 0)
        {
            throw new ArgumentException("Table must have at least one column.");
        }
        if (columns.Length != data.Length)
        {
            throw new ArgumentException("Columns and data must have the same length.");
        }
        // Row count check
        if (data.Any(a => a.Length != data.First().Length))
        {
            throw new ArgumentException("All columns data must have the same number of rows.");
        }
        Columns = columns;
        Data = data;
    }

    /// <summary>
    /// Gets or sets the columns of the table.
    /// </summary>
    public Column[] Columns { get; }

    /// <summary>
    /// Gets or sets the columnar stored data of the table.
    /// </summary>
    public Array[] Data { get; }

    /// <summary>
    /// Gets or sets the number of rows in the table.
    /// </summary>
    public int RowCount => Data.First().Length;

    /// <summary>
    /// Gets or sets the number of columns in the table.
    /// </summary>
    public int ColumnCount => Columns.Length;

    object IIndexable.this[int index] => this[index];

    /// <summary>
    /// Gets the row at the specified index.
    /// </summary>
    /// <param name="index">The index of the row.</param>
    /// <returns>The row at the specified index.</returns>
    public Row this[int index] => new Row(this, index);

    /// <summary>
    /// Gets the enumerator for iterating over the rows of the table.
    /// </summary>
    /// <returns>The enumerator for iterating over the rows of the table.</returns>
    public IEnumerator<Row> GetEnumerator()
    {
        for (int i = 0; i < RowCount; i++)
        {
            yield return this[i];
        }
    }

    /// <summary>
    /// Gets the enumerator for iterating over the rows of the table.
    /// </summary>
    /// <returns>The enumerator for iterating over the rows of the table.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    /// <summary>
    /// Represents a row in the table.
    /// </summary>
    public readonly struct Row
    {
        private readonly KTable table;
        private readonly int rowIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> struct.
        /// </summary>
        /// <param name="table">The table containing the row.</param>
        /// <param name="rowIndex">The index of the row.</param>
        internal Row(KTable table, int rowIndex)
        {
            this.table = table;
            this.rowIndex = rowIndex;
        }
        public KTable.Column[] Columns => table.Columns;
        /// <summary>
        /// Gets the value at the specified column index.
        /// </summary>
        /// <param name="columnIndex">The index of the column.</param>
        /// <returns>The value at the specified column index.</returns>
        public readonly object? this[int columnIndex] => table.Data[columnIndex].GetValue(rowIndex);

        /// <summary>
        /// Gets the value of the specified column by name.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>The value of the specified column.</returns>
        public object? this[string columnName]
        {
            get
            {
                int columnIndex = GetColumnIndex(columnName);
                return this[columnIndex];
            }
        }

        /// <summary>
        /// Gets the index of the specified column by name.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>The index of the specified column. Returns -1 if the column is not found.</returns>
        private int GetColumnIndex(string columnName)
        {
            for (int i = 0; i < table.Columns.Length; i++)
            {
                if (table.Columns[i].Name == columnName)
                {
                    return i;
                }
            }
            throw new IndexOutOfRangeException($"Column '{columnName}' not found.");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Columns.Length; i++)
            {
                sb.Append($"{Columns[i].Name}: {this[i]}");
                if (i < Columns.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            return sb.ToString();
        }
    }
}


/// <summary>
/// Represents a keyed table.
/// </summary>
public class KKeyedTable : IEnumerable<KKeyedTable.KeyValuePair>, IIndexable, IKTable
{
    public KTable Keys { get; }

    /// <summary>
    /// Gets or sets the value table.
    /// </summary>
    public KTable Values { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KKeyedTable"/> class.
    /// </summary>
    /// <param name="keys">The key table.</param>
    /// <param name="values">The value table.</param>
    public KKeyedTable(KTable keys, KTable values)
    {
        // Null
        if (keys is null)
        {
            throw new ArgumentNullException(nameof(keys));
        }
        if (values is null)
        {
            throw new ArgumentNullException(nameof(values));
        }
        // Column count
        if (keys.ColumnCount == 0)
        {
            throw new ArgumentException("Keys table must have at least one column.");
        }
        if (values.ColumnCount == 0)
        {
            throw new ArgumentException("Values table must have at least one column.");
        }
        // validation
        if (keys.RowCount != values.RowCount)
        {
            throw new ArgumentException("Keys and values tables must have the same number of rows.");
        }
        Keys = keys;
        Values = values;
    }

    /// <summary>
    /// Gets the number of rows in the keyed table.
    /// </summary>
    public int RowCount => Keys.RowCount;

    /// <summary>
    /// Gets the number of columns in the keyed table.
    /// </summary>
    public int ColumnCount => Keys.ColumnCount + Values.ColumnCount;

    /// <summary>
    /// Gets the columns of the keyed table.
    /// </summary>
    public KTable.Column[] Columns => Keys.Columns.Concat(Values.Columns).ToArray();

    /// <summary>
    /// Gets the columnar stored data of the keyed table.
    /// </summary>
    public Array[] Data => Keys.Data.Concat(Values.Data).ToArray();

    KTable.Row IKTable.this[int index] => throw new NotImplementedException();

    object IIndexable.this[int index] => this[index];

    /// <summary>
    /// Gets the row at the specified index.
    /// </summary>
    /// <param name="index">The index of the row.</param>
    /// <returns>The row at the specified index.</returns>
    public KeyValuePair this[int index] => new KeyValuePair(this, index);

    /// <summary>
    /// Gets the enumerator for iterating over the rows of the keyed table.
    /// </summary>
    /// <returns>The enumerator for iterating over the rows of the keyed table.</returns>
    public IEnumerator<KeyValuePair> GetEnumerator()
    {
        for (int i = 0; i < RowCount; i++)
        {
            yield return this[i];
        }
    }

    /// <summary>
    /// Gets the enumerator for iterating over the rows of the keyed table.
    /// </summary>
    /// <returns>The enumerator for iterating over the rows of the keyed table.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator<KTable.Row> IEnumerable<KTable.Row>.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Represents a row in the keyed table.
    /// </summary>
    public readonly struct KeyValuePair
    {
        private readonly KKeyedTable table;
        private readonly int rowIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePair"/> struct.
        /// </summary>
        /// <param name="table">The keyed table containing the row.</param>
        /// <param name="rowIndex">The index of the row.</param>
        internal KeyValuePair(KKeyedTable table, int rowIndex)
        {
            this.table = table;
            this.rowIndex = rowIndex;
        }

        public KTable.Row Key => table.Keys[rowIndex];
        public KTable.Row Value => table.Values[rowIndex];

        public object? this[string columnName]
        {
            get
            {
                int columnIndex = GetColumnIndex(columnName);
                return this[columnIndex];
            }
        }

        // index by column index
        public object? this[int columnIndex]
        {
            get
            {
                if (columnIndex < table.Keys.ColumnCount)
                {
                    return Key[columnIndex];
                }
                else
                {
                    return Value[columnIndex - table.Keys.ColumnCount];
                }
            }
        }

        private int GetColumnIndex(string columnName)
        {
            for (int i = 0; i < table.Columns.Length; i++)
            {
                if (table.Columns[i].Name == columnName)
                {
                    return i;
                }
            }
            throw new IndexOutOfRangeException($"Column '{columnName}' not found.");
        }

    }
}

public interface IKTable : IEnumerable<KTable.Row>
{
    public KTable.Column[] Columns { get; }
    public Array[] Data { get; }
    public int RowCount { get; }
    public int ColumnCount { get; }
    public KTable.Row this[int index] { get; }
}
