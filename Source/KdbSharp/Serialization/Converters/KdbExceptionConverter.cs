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

public class KdbExceptionConverter : KTypeConverter<KdbException>
{
    public override bool CanConvert(Type t, KType kt)
    {
        return kt == KType.Error && t == TypeToConvert;
    }

    public override KdbException Read(KReader reader, KSerializerOptions options)
    {
        var errorMessage = reader.ReadSymbol();
        return new KdbException(errorMessage);
    }

    public override void Write(KWriter writer, KdbException value, KSerializerOptions options)
    {
        writer.BeginWriteType(KType.Error);
        writer.WriteSymbol(value.ShortText);
        writer.EndWriteType();
    }
}
