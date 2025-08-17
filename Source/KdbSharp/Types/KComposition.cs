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

using System.Collections;

namespace KdbSharp.Types;

/// <summary>
/// Represents a q composition function.
/// A composition is a function created by combining multiple functions.
/// </summary>
public sealed class KComposition : KFunction, IReadOnlyList<object?>
{
    private readonly object?[] _functions;

    /// <summary>
    /// Initializes a new instance of the KComposition class with the specified functions.
    /// </summary>
    /// <param name="functions">The functions to compose.</param>
    /// <exception cref="ArgumentNullException">Thrown when functions is null.</exception>
    /// <exception cref="ArgumentException">Thrown when functions array is empty.</exception>
    public KComposition(params object?[] functions)
    {
        if (functions == null)
            throw new ArgumentNullException(nameof(functions));
        
        if (functions.Length == 0)
            throw new ArgumentException("Composition must contain at least one function.", nameof(functions));

        _functions = functions;
    }

    /// <summary>
    /// Initializes a new instance of the KComposition class with functions from an array.
    /// </summary>
    /// <param name="functions">The array containing functions to compose.</param>
    /// <exception cref="ArgumentNullException">Thrown when functions is null.</exception>
    /// <exception cref="ArgumentException">Thrown when functions array is empty.</exception>
    public KComposition(Array functions)
    {
        if (functions == null)
            throw new ArgumentNullException(nameof(functions));
        
        if (functions.Length == 0)
            throw new ArgumentException("Composition must contain at least one function.", nameof(functions));

        _functions = new object?[functions.Length];
        for (int i = 0; i < functions.Length; i++)
        {
            _functions[i] = functions.GetValue(i);
        }
    }

    /// <summary>
    /// Initializes a new instance of the KComposition class with functions from an enumerable.
    /// </summary>
    /// <param name="functions">The enumerable containing functions to compose.</param>
    /// <exception cref="ArgumentNullException">Thrown when functions is null.</exception>
    /// <exception cref="ArgumentException">Thrown when functions enumerable is empty.</exception>
    public KComposition(IEnumerable<object?> functions)
    {
        if (functions == null)
            throw new ArgumentNullException(nameof(functions));

        _functions = functions.ToArray();
        
        if (_functions.Length == 0)
            throw new ArgumentException("Composition must contain at least one function.", nameof(functions));
    }

    /// <summary>
    /// Gets the KType code associated with this composition function.
    /// </summary>
    public override KType FunctionType => KType.Composition;

    /// <summary>
    /// Gets the functions in the composition as a read-only list.
    /// </summary>
    public IReadOnlyList<object?> Functions => _functions;

    /// <summary>
    /// Gets the number of functions in the composition.
    /// </summary>
    public int Count => _functions.Length;

    /// <summary>
    /// Gets the function at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the function to get.</param>
    /// <returns>The function at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when index is out of range.</exception>
    public object? this[int index]
    {
        get
        {
            if (index < 0 || index >= _functions.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _functions[index];
        }
    }

    /// <summary>
    /// Returns an array copy of the functions.
    /// </summary>
    /// <returns>A new array containing all functions.</returns>
    public object?[] ToArray()
    {
        var result = new object?[_functions.Length];
        Array.Copy(_functions, result, _functions.Length);
        return result;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current composition.
    /// </summary>
    /// <param name="obj">The object to compare with the current composition.</param>
    /// <returns>true if the specified object is equal to the current composition; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not KComposition other)
            return false;

        return _functions.SequenceEqual(other._functions);
    }

    /// <summary>
    /// Determines whether the specified composition is equal to the current composition.
    /// </summary>
    /// <param name="other">The composition to compare with the current composition.</param>
    /// <returns>true if the specified composition is equal to the current composition; otherwise, false.</returns>
    public bool Equals(KComposition? other)
    {
        if (other is null)
            return false;

        return _functions.SequenceEqual(other._functions);
    }

    /// <summary>
    /// Serves as a hash function for the composition type.
    /// </summary>
    /// <returns>A hash code for the current composition.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var func in _functions)
        {
            hash.Add(func);
        }
        return hash.ToHashCode();
    }

    /// <summary>
    /// Returns a string representation of the composition.
    /// </summary>
    /// <returns>A string that represents the current composition.</returns>
    public override string ToString()
    {
        var funcStrings = _functions.Select(f => f?.ToString() ?? "<null>");
        return $"KComposition: [{string.Join(" ∘ ", funcStrings)}]";
    }

    /// <summary>
    /// Returns an enumerator that iterates through the functions.
    /// </summary>
    /// <returns>An enumerator for the functions.</returns>
    public IEnumerator<object?> GetEnumerator()
    {
        return ((IEnumerable<object?>)_functions).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the functions.
    /// </summary>
    /// <returns>An enumerator for the functions.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _functions.GetEnumerator();
    }
}
