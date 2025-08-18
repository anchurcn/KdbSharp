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

namespace KdbSharp.Types;

/// <summary>
/// Base class for all KDB function types.
/// Represents q functions that can be serialized through IPC protocol.
/// </summary>
public abstract class KFunction
{
    /// <summary>
    /// Gets the KType code associated with this function.
    /// </summary>
    public abstract KType FunctionType { get; }

    /// <summary>
    /// Gets the raw type code as a byte value.
    /// </summary>
    public byte TypeCode => (byte)FunctionType;

    /// <summary>
    /// Returns a string representation of the function.
    /// </summary>
    /// <returns>A string that represents the current function.</returns>
    public override string ToString()
    {
        return $"KFunction#{TypeCode}h";
    }

    /// <summary>
    /// Creates a generic function instance with the specified type code.
    /// Used for primitive functions that don't have specific implementations.
    /// </summary>
    /// <param name="functionType">The function type.</param>
    /// <returns>A KFunction instance.</returns>
    public static KFunction Create(KType functionType)
    {
        return new KGenericFunction(functionType);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current function.
    /// </summary>
    /// <param name="obj">The object to compare with the current function.</param>
    /// <returns>true if the specified object is equal to the current function; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not KFunction other)
            return false;

        return FunctionType == other.FunctionType;
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current function.</returns>
    public override int GetHashCode()
    {
        return FunctionType.GetHashCode();
    }
}

/// <summary>
/// Generic function implementation for primitive functions without specific behavior.
/// </summary>
internal sealed class KGenericFunction : KFunction
{
    private readonly KType _functionType;

    /// <summary>
    /// Initializes a new instance of the KGenericFunction class.
    /// </summary>
    /// <param name="functionType">The function type.</param>
    public KGenericFunction(KType functionType)
    {
        _functionType = functionType;
    }

    /// <summary>
    /// Gets the KType code associated with this function.
    /// </summary>
    public override KType FunctionType => _functionType;

    /// <summary>
    /// Returns a string representation of the generic function.
    /// </summary>
    /// <returns>A string that represents the current generic function.</returns>
    public override string ToString()
    {
        return $"KGenericFunction#{TypeCode}h";
    }
}
