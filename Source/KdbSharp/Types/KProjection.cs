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
/// Represents a q projection function.
/// A projection is a partially applied function where some arguments are fixed.
/// </summary>
public sealed class KProjection : KFunction, IReadOnlyList<object?>
{
    private readonly object?[] _parameters;

    /// <summary>
    /// Initializes a new instance of the KProjection class with the specified parameters.
    /// </summary>
    /// <param name="parameters">The parameters for the projection.</param>
    /// <exception cref="ArgumentNullException">Thrown when parameters is null.</exception>
    public KProjection(params object?[] parameters)
    {
        _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
    }

    /// <summary>
    /// Initializes a new instance of the KProjection class with parameters from an array.
    /// </summary>
    /// <param name="parameters">The array containing projection parameters.</param>
    /// <exception cref="ArgumentNullException">Thrown when parameters is null.</exception>
    public KProjection(Array parameters)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        _parameters = new object?[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            _parameters[i] = parameters.GetValue(i);
        }
    }

    /// <summary>
    /// Initializes a new instance of the KProjection class with parameters from an enumerable.
    /// </summary>
    /// <param name="parameters">The enumerable containing projection parameters.</param>
    /// <exception cref="ArgumentNullException">Thrown when parameters is null.</exception>
    public KProjection(IEnumerable<object?> parameters)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        _parameters = parameters.ToArray();
    }

    /// <summary>
    /// Gets the KType code associated with this projection function.
    /// </summary>
    public override KType FunctionType => KType.Projection;

    /// <summary>
    /// Gets the parameters of the projection as a read-only array.
    /// </summary>
    public IReadOnlyList<object?> Parameters => _parameters;

    /// <summary>
    /// Gets the number of parameters in the projection.
    /// </summary>
    public int Count => _parameters.Length;

    /// <summary>
    /// Gets the parameter at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the parameter to get.</param>
    /// <returns>The parameter at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when index is out of range.</exception>
    public object? this[int index]
    {
        get
        {
            if (index < 0 || index >= _parameters.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _parameters[index];
        }
    }

    /// <summary>
    /// Returns an array copy of the parameters.
    /// </summary>
    /// <returns>A new array containing all parameters.</returns>
    public object?[] ToArray()
    {
        var result = new object?[_parameters.Length];
        Array.Copy(_parameters, result, _parameters.Length);
        return result;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current projection.
    /// </summary>
    /// <param name="obj">The object to compare with the current projection.</param>
    /// <returns>true if the specified object is equal to the current projection; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not KProjection other)
            return false;

        return _parameters.SequenceEqual(other._parameters);
    }

    /// <summary>
    /// Determines whether the specified projection is equal to the current projection.
    /// </summary>
    /// <param name="other">The projection to compare with the current projection.</param>
    /// <returns>true if the specified projection is equal to the current projection; otherwise, false.</returns>
    public bool Equals(KProjection? other)
    {
        if (other is null)
            return false;

        return _parameters.SequenceEqual(other._parameters);
    }

    /// <summary>
    /// Serves as a hash function for the projection type.
    /// </summary>
    /// <returns>A hash code for the current projection.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var param in _parameters)
        {
            hash.Add(param);
        }
        return hash.ToHashCode();
    }

    /// <summary>
    /// Returns a string representation of the projection.
    /// </summary>
    /// <returns>A string that represents the current projection.</returns>
    public override string ToString()
    {
        var paramStrings = _parameters.Select(p => p?.ToString() ?? "<null>");
        return $"KProjection: [{string.Join(", ", paramStrings)}]";
    }

    /// <summary>
    /// Returns an enumerator that iterates through the parameters.
    /// </summary>
    /// <returns>An enumerator for the parameters.</returns>
    public IEnumerator<object?> GetEnumerator()
    {
        return ((IEnumerable<object?>)_parameters).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the parameters.
    /// </summary>
    /// <returns>An enumerator for the parameters.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _parameters.GetEnumerator();
    }
}
