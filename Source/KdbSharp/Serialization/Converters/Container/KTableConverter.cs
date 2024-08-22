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
    public override bool CanConvert(Type t, KType kt)
    {
        return t == typeof(KTable) && kt == KType.Table;
    }

    public override KTable Read(KReader reader, KSerializerOptions options)
    {
        if (reader.TypeStamp != KType.Table)
        {
            throw new InvalidOperationException();
        }
        var colNames = KSerializer.Deserialize<string[]>(reader, options);
        var columns = new KTable.Column[colNames.Length];
        var colData = new Array[colNames.Length];
        reader.BeginReadType();
        //_ = reader.Buffer.ReadInt32();
        //_ = reader.ReadByte(); // attributes
        for (int i = 0; i < colNames.Length; i++)
        {
            var data = KSerializer.Deserialize<object>(reader, options);
            var colType = reader.LastNestedType ?? throw new InvalidOperationException();
            colData[i] = data as Array ?? throw new InvalidOperationException();
            columns[i] = new KTable.Column(colType, colNames[i]);
        }
        reader.EndReadType();
        return new KTable(columns, colData);
    }

    public override void Write(KWriter writer, KTable value, KSerializerOptions options)
    {
        writer.WriteStartTable();
        KSerializer.Serialize(writer, value.Columns.Select(x => x.Name).ToArray(), options);
        writer.WriteStartList(KType.GeneralList, value.ColumnCount);
        for (int i = 0; i < value.ColumnCount; i++)
        {
            KSerializer.Serialize(writer, value.Data[i], value.Columns[i].Type, options);
        }
        writer.WriteEndList();
        writer.WriteEndTable();
    }
}
