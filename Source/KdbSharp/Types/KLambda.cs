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

using System.Text.RegularExpressions;

namespace KdbSharp.Types;

/// <summary>
/// Represents a q lambda expression.
/// Lambda expressions are user-defined functions in q that can be serialized and executed.
/// </summary>
public sealed class KLambda : KFunction
{
    private static readonly Regex LambdaRegex = new(@"\s*(k\))?\s*\{.*\}", RegexOptions.Compiled);
    
    private readonly string _expression;
    private readonly string? _context;

    /// <summary>
    /// Initializes a new instance of the KLambda class with the specified expression.
    /// </summary>
    /// <param name="expression">The lambda expression body. Must be enclosed in { and } brackets.</param>
    /// <param name="context">Optional context information for the lambda.</param>
    /// <exception cref="ArgumentException">Thrown when the expression is null, empty, or invalid.</exception>
    public KLambda(string expression, string? context = null)
    {
        if (string.IsNullOrEmpty(expression))
            throw new ArgumentException("Lambda expression cannot be null or empty.", nameof(expression));

        expression = expression.Trim();

        if (!LambdaRegex.IsMatch(expression))
            throw new ArgumentException($"Invalid lambda expression: {expression}", nameof(expression));

        _expression = expression;
        _context = context;
    }

    /// <summary>
    /// Gets the KType code associated with this lambda function.
    /// </summary>
    public override KType FunctionType => KType.Lambda;

    /// <summary>
    /// Gets the body of the q lambda expression.
    /// </summary>
    public string Expression => _expression;

    /// <summary>
    /// Gets the context information for the lambda, if any.
    /// </summary>
    public string? Context => _context;

    /// <summary>
    /// Determines whether the specified object is equal to the current lambda.
    /// </summary>
    /// <param name="obj">The object to compare with the current lambda.</param>
    /// <returns>true if the specified object is equal to the current lambda; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not KLambda other)
            return false;

        return _expression.Equals(other._expression, StringComparison.Ordinal) &&
               string.Equals(_context, other._context, StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines whether the specified lambda is equal to the current lambda.
    /// </summary>
    /// <param name="other">The lambda to compare with the current lambda.</param>
    /// <returns>true if the specified lambda is equal to the current lambda; otherwise, false.</returns>
    public bool Equals(KLambda? other)
    {
        if (other is null)
            return false;

        return _expression.Equals(other._expression, StringComparison.Ordinal) &&
               string.Equals(_context, other._context, StringComparison.Ordinal);
    }

    /// <summary>
    /// Serves as a hash function for the lambda type.
    /// </summary>
    /// <returns>A hash code for the current lambda.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(_expression, _context);
    }

    /// <summary>
    /// Returns a string representation of the lambda.
    /// </summary>
    /// <returns>A string that represents the current lambda.</returns>
    public override string ToString()
    {
        var contextPart = string.IsNullOrEmpty(_context) ? "" : $"[{_context}] ";
        return $"KLambda: {contextPart}{_expression}";
    }

    /// <summary>
    /// Validates whether the given string is a valid lambda expression format.
    /// </summary>
    /// <param name="expression">The expression to validate.</param>
    /// <returns>true if the expression is valid; otherwise, false.</returns>
    public static bool IsValidExpression(string? expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return false;

        return LambdaRegex.IsMatch(expression.Trim());
    }
}
