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

using KdbSharp.Serialization;
using KdbSharp.Types;
using Xunit;

namespace KdbSharp.Test;

/// <summary>
/// Tests for function type serialization and deserialization.
/// </summary>
public class FunctionSerializationTest
{
    [Fact]
    public void KLambda_BasicSerialization_ShouldWork()
    {
        // Arrange
        var lambda = new KLambda("{x+y}");
        var options = new KSerializerOptions();

        // Act & Assert - Basic creation should work
        Assert.Equal(KType.Lambda, lambda.FunctionType);
        Assert.Equal("{x+y}", lambda.Expression);
        Assert.Null(lambda.Context);
    }

    [Fact]
    public void KLambda_WithContext_ShouldWork()
    {
        // Arrange
        var lambda = new KLambda("{x+y}", "mycontext");

        // Act & Assert
        Assert.Equal(KType.Lambda, lambda.FunctionType);
        Assert.Equal("{x+y}", lambda.Expression);
        Assert.Equal("mycontext", lambda.Context);
    }

    [Fact]
    public void KLambda_InvalidExpression_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new KLambda("invalid"));
        Assert.Throws<ArgumentException>(() => new KLambda(""));
        Assert.Throws<ArgumentException>(() => new KLambda(null!));
    }

    [Fact]
    public void KProjection_BasicCreation_ShouldWork()
    {
        // Arrange
        var parameters = new object?[] { 1, "test", null };
        var projection = new KProjection(parameters);

        // Act & Assert
        Assert.Equal(KType.Projection, projection.FunctionType);
        Assert.Equal(3, projection.Count);
        Assert.Equal(1, projection[0]);
        Assert.Equal("test", projection[1]);
        Assert.Null(projection[2]);
    }

    [Fact]
    public void KComposition_BasicCreation_ShouldWork()
    {
        // Arrange
        var functions = new object?[] { new KLambda("{x+1}"), new KLambda("{x*2}") };
        var composition = new KComposition(functions);

        // Act & Assert
        Assert.Equal(KType.Composition, composition.FunctionType);
        Assert.Equal(2, composition.Count);
        Assert.IsType<KLambda>(composition[0]);
        Assert.IsType<KLambda>(composition[1]);
    }

    [Fact]
    public void KFunction_GenericCreation_ShouldWork()
    {
        // Arrange & Act
        var function = KFunction.Create(KType.UnaryPrimitive);

        // Assert
        Assert.Equal(KType.UnaryPrimitive, function.FunctionType);
        Assert.Equal((byte)KType.UnaryPrimitive, function.TypeCode);
    }

    [Fact]
    public void KLambda_Equality_ShouldWork()
    {
        // Arrange
        var lambda1 = new KLambda("{x+y}");
        var lambda2 = new KLambda("{x+y}");
        var lambda3 = new KLambda("{x*y}");

        // Act & Assert
        Assert.Equal(lambda1, lambda2);
        Assert.NotEqual(lambda1, lambda3);
        Assert.True(lambda1.Equals(lambda2));
        Assert.False(lambda1.Equals(lambda3));
    }

    [Fact]
    public void KProjection_Equality_ShouldWork()
    {
        // Arrange
        var proj1 = new KProjection(1, 2, 3);
        var proj2 = new KProjection(1, 2, 3);
        var proj3 = new KProjection(1, 2, 4);

        // Act & Assert
        Assert.Equal(proj1, proj2);
        Assert.NotEqual(proj1, proj3);
    }

    [Fact]
    public void KLambda_IsValidExpression_ShouldWork()
    {
        // Act & Assert
        Assert.True(KLambda.IsValidExpression("{x+y}"));
        Assert.True(KLambda.IsValidExpression("k){x+y}"));
        Assert.True(KLambda.IsValidExpression("  {x+y}  "));
        
        Assert.False(KLambda.IsValidExpression("invalid"));
        Assert.False(KLambda.IsValidExpression(""));
        Assert.False(KLambda.IsValidExpression(null));
    }

    [Fact]
    public void KProjection_ToArray_ShouldWork()
    {
        // Arrange
        var original = new object?[] { 1, "test", null };
        var projection = new KProjection(original);

        // Act
        var copy = projection.ToArray();

        // Assert
        Assert.Equal(original, copy);
        Assert.NotSame(original, copy); // Should be a copy, not the same reference
    }

    [Fact]
    public void KComposition_EmptyFunctions_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new KComposition());
        Assert.Throws<ArgumentException>(() => new KComposition(new object[0]));
    }
}
