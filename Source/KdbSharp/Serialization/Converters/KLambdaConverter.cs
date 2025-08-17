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

/// <summary>
/// Converter for KLambda type serialization and deserialization.
/// </summary>
public sealed class KLambdaConverter : KTypeConverter<KLambda>
{
    /// <summary>
    /// Gets the KType that this converter handles.
    /// </summary>
    public override KType? TypeToWriteTo => KType.Lambda;

    /// <summary>
    /// Gets the .NET type that this converter can read back to.
    /// </summary>
    public override Type TypeToReadBack => typeof(KLambda);

    /// <summary>
    /// Determines whether this converter can convert the specified type.
    /// </summary>
    /// <param name="t">The .NET type to check.</param>
    /// <param name="kt">The KType to check.</param>
    /// <returns>true if this converter can handle the type; otherwise, false.</returns>
    public override bool CanConvert(Type t, KType kt)
    {
        return t == typeof(KLambda) && kt == KType.Lambda;
    }

    /// <summary>
    /// Writes a KLambda instance to the writer.
    /// </summary>
    /// <param name="writer">The serialization writer.</param>
    /// <param name="value">The lambda to write.</param>
    /// <param name="options">Serialization options.</param>
    public override void Write(ref KSerializationWriter writer, KLambda value, KSerializerOptions options)
    {
        writer.WriteLambda(value);
    }

    /// <summary>
    /// Reads a KLambda instance from the reader.
    /// </summary>
    /// <param name="reader">The serialization reader.</param>
    /// <param name="options">Serialization options.</param>
    /// <returns>The deserialized lambda.</returns>
    public override KLambda Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        return reader.ReadLambda();
    }
}
