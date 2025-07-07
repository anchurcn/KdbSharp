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
using KdbSharp.Types;

namespace KdbSharp.Serialization.Converters;

public class KTableConverter : KTypeConverter<KTable>
{
    public override KType? TypeToWriteTo => KType.Table;

    public override bool CanConvert(Type t, KType kt)
    {
        return t == typeof(KTable) && kt == KType.Table;
    }

    public override KTable Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        reader.StartReadTable();
        var colNames = KSerializer.Deserialize<string[]>(ref reader, options);
        var columns = new KTable.Column[colNames.Length];
        var colData = new Array[colNames.Length];
        reader.StartReadList();
        for (int i = 0; i < colNames.Length; i++)
        {
            var data = KSerializer.Deserialize<object>(ref reader, options);

            colData[i] = data as Array ?? throw new InvalidOperationException();

            // Infer column type from the data array type
            var colType = reader.LastNestedType ?? throw new InvalidOperationException();
            columns[i] = new KTable.Column(colType, colNames[i]);
        }
        reader.EndReadList();
        reader.EndReadTable();
        return new KTable(columns, colData);
    }

    public override void Write(ref KSerializationWriter writer, KTable value, KSerializerOptions options)
    {
        writer.StartWriteTable();
        KSerializer.Serialize(ref writer, value.Columns.Select(x => x.Name).ToArray(), options);
        writer.StartWriteList(KType.GeneralList, value.ColumnCount);
        for (int i = 0; i < value.ColumnCount; i++)
        {
            KSerializer.Serialize(ref writer, value.Data[i], value.Columns[i].Type, options);
        }
        writer.EndWriteList();
        writer.EndWriteTable();
    }
}
