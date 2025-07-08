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

using System;
using System.Buffers;
using KdbSharp.Serialization;
using KdbSharp.Serialization.Converters;
using KdbSharp.Types;
using Xunit;

namespace KdbSharp.Test;

public class TupleConverterTest
{
    [Fact]
    public void GetConverter_Type_ShouldReturnTupleConverterForTupleTypes()
    {
        // Arrange
        var options = new KSerializerOptions();
        
        // Act
        var tupleConverter = options.GetConverter(typeof(Tuple<int, string>));
        var valueTupleConverter = options.GetConverter(typeof(ValueTuple<int, string>));
        
        // Assert
        Assert.NotNull(tupleConverter);
        Assert.NotNull(valueTupleConverter);
        Assert.IsType<ValueTupleConverter<ValueTuple<int, string>>>(valueTupleConverter);
    }
    
    [Fact]
    public void GetConverter_KType_ShouldReturnConverterForGeneralList()
    {
        // Arrange
        var options = new KSerializerOptions();
        
        // Act
        var converter = options.GetConverter(KType.GeneralList);
        
        // Assert
        Assert.NotNull(converter);
    }
    
    
    [Fact]
    public void ValueTupleConverterFactory_CanConvert_ShouldReturnTrueForValueTupleTypes()
    {
        // Arrange
        var factory = new ValueTupleConverterFactory();
        
        // Act & Assert
        Assert.True(factory.CanConvert(typeof(ValueTuple<int>), KType.GeneralList));
        Assert.True(factory.CanConvert(typeof(ValueTuple<int, string>), KType.GeneralList));
        Assert.True(factory.CanConvert(typeof(ValueTuple<int, string, bool>), KType.GeneralList));
        Assert.False(factory.CanConvert(typeof(Tuple<int, string>), KType.GeneralList));
        Assert.False(factory.CanConvert(typeof(string), KType.GeneralList));
    }
    
    [Fact]
    public void ValueTupleConverterFactory_GetConverter_ShouldReturnCorrectConverter()
    {
        // Arrange
        var factory = new ValueTupleConverterFactory();
        var options = new KSerializerOptions();
        
        // Act
        var converter = factory.GetConverter(typeof(ValueTuple<int, string>), KType.GeneralList, options);
        
        // Assert
        Assert.NotNull(converter);
        Assert.IsType<ValueTupleConverter<ValueTuple<int, string>>>(converter);
        Assert.Equal(typeof(ValueTuple<int, string>), converter.TypeToReadBack);
        Assert.Equal(KType.GeneralList, converter.TypeToWriteTo);
    }
    
    [Fact]
    public void GetConverter_WithFactoryInDefaultConverters_ShouldCreateActualConverter()
    {
        // Arrange
        var options = new KSerializerOptions();

        // Act - Test that factory converters are properly instantiated
        var tupleConverter = options.GetConverter(typeof(Tuple<int, string, bool>));
        var valueTupleConverter = options.GetConverter(typeof(ValueTuple<double, char>));

        // Assert
        Assert.NotNull(tupleConverter);
        Assert.NotNull(valueTupleConverter);

        // Verify they are actual converters, not factories
        Assert.False(tupleConverter is KTypeConverterFactory);
        Assert.False(valueTupleConverter is KTypeConverterFactory);

        // Verify correct types
        Assert.IsType<ValueTupleConverter<ValueTuple<double, char>>>(valueTupleConverter);
    }

    [Fact]
    public void ValueTupleConverter_SerializeDeserialize_ShouldRoundTrip()
    {
        // Arrange
        var options = new KSerializerOptions();
        var originalTuple = (42, "hello");

        // Act - Serialize
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new KSerializationWriter(buffer);
        KSerializer.Serialize(ref writer, originalTuple, options);

        // Act - Deserialize
        var reader = new KSerializationReader(buffer.WrittenMemory) { IsLittleEndian = true };
        var deserializedTuple = KSerializer.Deserialize<(int, string)>(ref reader, options);

        // Assert
        Assert.Equal(originalTuple.Item1, deserializedTuple.Item1);
        Assert.Equal(originalTuple.Item2, deserializedTuple.Item2);
    }
}
