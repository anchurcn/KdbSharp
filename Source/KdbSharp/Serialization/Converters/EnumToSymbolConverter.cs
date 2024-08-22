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


public class EnumToSymbolConverterFactory : KTypeConverterFactory
{
    public override bool CanConvert(Type t, KType kt) => t.IsEnum && kt == KType.Symbol;

    public override KTypeConverter GetConverter(Type t, KType kt, KSerializerOptions options)
    {
        // Create via reflection
        var type = typeof(EnumToSymbolConverter<>).MakeGenericType(t);
        return (KTypeConverter)Activator.CreateInstance(type)!;
    }

    public class EnumToSymbolConverter<T> : KTypeConverter<T> where T : struct, Enum
    {
        public override bool CanConvert(Type t, KType kt)
        {
            return t == TypeToConvert && kt == KType.Symbol;
        }

        public override T Read(KReader reader, KSerializerOptions options)
        {
            var str = reader.ReadSymbol();
            return Enum.Parse<T>(str);
        }

        public override void Write(KWriter writer, T value, KSerializerOptions options)
        {
            writer.WriteSymbol(value.ToString());
        }
    }
}
