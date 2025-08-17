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
/// Converter for KFunction base type serialization and deserialization.
/// Handles polymorphic function types.
/// </summary>
public sealed class KFunctionConverter : KTypeConverter<KFunction>
{
    /// <summary>
    /// Gets the KType that this converter handles. Returns null since functions have multiple types.
    /// </summary>
    public override KType? TypeToWriteTo => null;

    /// <summary>
    /// Gets the .NET type that this converter can read back to.
    /// </summary>
    public override Type TypeToReadBack => typeof(KFunction);

    /// <summary>
    /// Determines whether this converter can convert the specified type.
    /// </summary>
    /// <param name="t">The .NET type to check.</param>
    /// <param name="kt">The KType to check.</param>
    /// <returns>true if this converter can handle the type; otherwise, false.</returns>
    public override bool CanConvert(Type t, KType kt)
    {
        return typeof(KFunction).IsAssignableFrom(t);
    }

    /// <summary>
    /// Writes a KFunction instance to the writer.
    /// </summary>
    /// <param name="writer">The serialization writer.</param>
    /// <param name="value">The function to write.</param>
    /// <param name="options">Serialization options.</param>
    public override void Write(ref KSerializationWriter writer, KFunction value, KSerializerOptions options)
    {
        writer.WriteFunction(value);
    }

    /// <summary>
    /// Reads a KFunction instance from the reader.
    /// This method should not be called directly as function reading is type-specific.
    /// </summary>
    /// <param name="reader">The serialization reader.</param>
    /// <param name="options">Serialization options.</param>
    /// <returns>The deserialized function.</returns>
    /// <exception cref="NotSupportedException">This method should not be called directly.</exception>
    public override KFunction Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        throw new NotSupportedException("KFunction reading should be handled by type-specific converters based on the type stamp.");
    }
}

/// <summary>
/// Converter factory for function types that creates appropriate converters based on KType.
/// </summary>
public sealed class KFunctionConverterFactory : KTypeConverterFactory
{
    /// <summary>
    /// Determines whether this factory can convert the specified type.
    /// </summary>
    /// <param name="t">The .NET type to check.</param>
    /// <param name="kt">The KType to check.</param>
    /// <returns>true if this factory can handle the type; otherwise, false.</returns>
    public override bool CanConvert(Type t, KType kt)
    {
        return typeof(KFunction).IsAssignableFrom(t);
    }

    /// <summary>
    /// Gets a converter for the specified type and KType.
    /// </summary>
    /// <param name="t">The .NET type to create a converter for.</param>
    /// <param name="kt">The KType, if known.</param>
    /// <param name="options">Serialization options.</param>
    /// <returns>A converter for the specified type.</returns>
    public override KTypeConverter GetConverter(Type t, KType? kt, KSerializerOptions options)
    {
        if (t == typeof(KLambda))
            return new KLambdaConverter();

        if (t == typeof(KProjection))
            return new KProjectionConverter();

        if (t == typeof(KComposition))
            return new KCompositionConverter();

        if (t == typeof(KFunction) || typeof(KFunction).IsAssignableFrom(t))
            return new KFunctionConverter();

        throw new NotSupportedException($"Function type {t} is not supported.");
    }
}

/// <summary>
/// Converter for reading functions based on KType stamps.
/// </summary>
public sealed class KFunctionTypeConverter : KTypeConverter<KFunction>
{
    private readonly KType _functionType;

    /// <summary>
    /// Initializes a new instance of the KFunctionTypeConverter class.
    /// </summary>
    /// <param name="functionType">The specific function type this converter handles.</param>
    public KFunctionTypeConverter(KType functionType)
    {
        _functionType = functionType;
    }

    /// <summary>
    /// Gets the KType that this converter handles.
    /// </summary>
    public override KType? TypeToWriteTo => _functionType;

    /// <summary>
    /// Gets the .NET type that this converter can read back to.
    /// </summary>
    public override Type TypeToReadBack => typeof(KFunction);

    /// <summary>
    /// Determines whether this converter can convert the specified type.
    /// </summary>
    /// <param name="t">The .NET type to check.</param>
    /// <param name="kt">The KType to check.</param>
    /// <returns>true if this converter can handle the type; otherwise, false.</returns>
    public override bool CanConvert(Type t, KType kt)
    {
        return typeof(KFunction).IsAssignableFrom(t) && kt == _functionType;
    }

    /// <summary>
    /// Writes a KFunction instance to the writer.
    /// </summary>
    /// <param name="writer">The serialization writer.</param>
    /// <param name="value">The function to write.</param>
    /// <param name="options">Serialization options.</param>
    public override void Write(ref KSerializationWriter writer, KFunction value, KSerializerOptions options)
    {
        writer.WriteFunction(value);
    }

    /// <summary>
    /// Reads a KFunction instance from the reader based on the function type.
    /// </summary>
    /// <param name="reader">The serialization reader.</param>
    /// <param name="options">Serialization options.</param>
    /// <returns>The deserialized function.</returns>
    public override KFunction Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        return reader.ReadFunction(_functionType);
    }
}
