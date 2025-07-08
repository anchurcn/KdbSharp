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

public class CharArrayConverter : KTypeConverter<char[]>
{
    public override KType? TypeToWriteTo => KType.CharList;

    public override bool CanConvert(Type t, KType kt) => t == TypeToReadBack && kt == TypeToWriteTo;

    public override char[] Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        var value = reader.ReadCharListAsString();
        return value.ToCharArray();
    }

    public override void Write(ref KSerializationWriter writer, char[] value, KSerializerOptions options)
    {
        writer.WriteCharList(value);
    }
}
