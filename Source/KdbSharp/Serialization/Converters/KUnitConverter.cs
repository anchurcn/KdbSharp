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

public class KUnitConverter : KTypeConverter<KUnit>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return kt == KType.Unit && t == TypeToConvert;
    }

    public override KUnit Read(KReader reader, KSerializerOptions options)
    {
        if (reader.ReadByte() != 0)
        {
            throw new InvalidOperationException("Invalid unit type");
        }
        return KUnit.Value;
    }

    public override void Write(KWriter writer, KUnit value, KSerializerOptions options)
    {
        writer.BeginWriteType(KType.Unit);
        writer.WriteByte(0);
        writer.EndWriteType();
    }
}
