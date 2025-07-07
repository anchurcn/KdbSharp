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
        throw new NotImplementedException();
        reader.StartReadTable();
        var colNames = KSerializer.Deserialize<string[]>(ref reader, options);
        var columns = new KTable.Column[colNames.Length];
        var colData = new Array[colNames.Length];
        reader.BeginReadType();
        //_ = reader.Buffer.ReadInt32();
        //_ = reader.ReadByte(); // attributes
        for (int i = 0; i < colNames.Length; i++)
        {
            var data = KSerializer.Deserialize<object>(ref reader, options);

            colData[i] = data as Array ?? throw new InvalidOperationException();

            // Infer column type from the data array type
            var colType = InferKTypeFromArray(colData[i]);
            columns[i] = new KTable.Column(colType, colNames[i]);
        }
        reader.EndReadType();
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

    private static KType InferKTypeFromArray(Array array)
    {
        var elementType = array.GetType().GetElementType();

        return elementType switch
        {
            Type t when t == typeof(bool) => KType.BooleanList,
            Type t when t == typeof(Guid) => KType.GuidList,
            Type t when t == typeof(byte) => KType.ByteList,
            Type t when t == typeof(short) => KType.ShortList,
            Type t when t == typeof(int) => KType.IntList,
            Type t when t == typeof(long) => KType.LongList,
            Type t when t == typeof(float) => KType.RealList,
            Type t when t == typeof(double) => KType.FloatList,
            Type t when t == typeof(char) => KType.CharList,
            Type t when t == typeof(string) => KType.SymbolList,
            Type t when t == typeof(KShort) => KType.ShortList,
            Type t when t == typeof(KInt) => KType.IntList,
            Type t when t == typeof(KLong) => KType.LongList,
            Type t when t == typeof(KReal) => KType.RealList,
            Type t when t == typeof(KFloat) => KType.FloatList,
            Type t when t == typeof(KChar) => KType.CharList,
            Type t when t == typeof(KTimestamp) => KType.TimestampList,
            Type t when t == typeof(KDate) => KType.DateList,
            Type t when t == typeof(KTime) => KType.TimeList,
            Type t when t == typeof(KDateTime) => KType.DateTimeList,
            Type t when t == typeof(KTimeSpan) => KType.TimeSpanList,
            Type t when t == typeof(KMonth) => KType.MonthList,
            Type t when t == typeof(KMinute) => KType.MinuteList,
            Type t when t == typeof(KSecond) => KType.SecondList,
            _ => KType.GeneralList
        };
    }
}
