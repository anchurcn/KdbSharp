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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KdbSharp.Serialization.Converters
{
    public class KGenericListConverter : KTypeConverter<object[]>
    {
        public override bool CanConvert(Type t, KType kt)
        {
            return kt == KType.GeneralList && t == typeof(object[]);
        }

        public override object[] Read(KReader reader, KSerializerOptions options)
        {
            var len = reader.ListLength ?? throw new MessageSerializationException("Array length is null.");
            var arr = new object[len];
            for (int i = 0; i < len; i++)
            {
                var elemType = reader.BeginReadType();
                var elemTypeInfo = options.GetTypeInfo(typeof(object));
                arr[i] = elemTypeInfo.DeserializeAsObject(reader)!;
                reader.EndReadType();
            }
            return arr;
        }

        public override void Write(KWriter writer, object[] value, KSerializerOptions options)
        {
            writer.WriteStartList(KType.GeneralList, value.Length);
            foreach (var elem in value)
            {
                var elemTypeInfo = options.GetTypeInfo(typeof(object));
                elemTypeInfo.SerializeAsObject(writer, elem);
            }
            writer.WriteEndList();
        }
    }
}


