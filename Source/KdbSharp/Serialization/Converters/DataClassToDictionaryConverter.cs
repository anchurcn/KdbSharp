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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KdbSharp.Types;

namespace KdbSharp.Serialization;

/// <summary>
/// Converter that serializes data classes to KDictionary and vice versa.
/// </summary>
/// <typeparam name="T">The data class type.</typeparam>
public class DataClassToDictionaryConverter<T> : KTypeConverter<T> where T : class
{
    private readonly Dictionary<PropertyInfo, PropertyConfiguration> _propertyConfigurations;
    private readonly HashSet<PropertyInfo> _ignoredProperties;
    private readonly PropertyInfo[] _serializableProperties;

    /// <summary>
    /// Initializes a new instance of the DataClassToDictionaryConverter.
    /// </summary>
    /// <param name="propertyConfigurations">Property configurations.</param>
    /// <param name="ignoredProperties">Properties to ignore during serialization.</param>
    internal DataClassToDictionaryConverter(
        Dictionary<PropertyInfo, PropertyConfiguration> propertyConfigurations,
        HashSet<PropertyInfo> ignoredProperties)
    {
        _propertyConfigurations = propertyConfigurations;
        _ignoredProperties = ignoredProperties;
        
        // Cache serializable properties
        _serializableProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && !_ignoredProperties.Contains(p))
            .ToArray();
    }
    
    /// <summary>
    /// Gets the target KType for writing (KDictionary).
    /// </summary>
    public override KType? TypeToWriteTo => KType.Dictionary;

    /// <summary>
    /// Determines if this converter can convert the specified type and KType combination.
    /// </summary>
    /// <param name="t">The .NET type.</param>
    /// <param name="kt">The KType.</param>
    /// <returns>True if conversion is supported.</returns>
    public override bool CanConvert(Type t, KType kt)
    {
        return t == typeof(T) && kt == KType.Dictionary;
    }
    
    /// <summary>
    /// Writes the data class as a KDictionary.
    /// </summary>
    /// <param name="writer">The serialization writer.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(ref KSerializationWriter writer, T? value, KSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteUnit();
            return;
        }
        
        var keys = new string[_serializableProperties.Length];
        var values = new object?[_serializableProperties.Length];
        
        for (int i = 0; i < _serializableProperties.Length; i++)
        {
            var property = _serializableProperties[i];
            var config = _propertyConfigurations.GetValueOrDefault(property);
            
            // Use configured column name or property name
            keys[i] = config?.ColumnName ?? property.Name;
            
            // Get property value
            var propertyValue = property.GetValue(value);
            
            // Apply property-specific serialization if configured
            if (config?.Converter != null)
            {
                // Use custom converter for this property
                // Note: This would require a more complex implementation to handle
                // property-level converters within the dictionary context
                values[i] = propertyValue;
            }
            else if (config?.KType.HasValue == true)
            {
                // The value will be serialized using the predefined converter
                // for PropertyType -> KType during dictionary serialization
                values[i] = propertyValue;
            }
            else
            {
                // Use default serialization
                values[i] = propertyValue;
            }
        }
        
        // Create and serialize KSimpleDictionary
        var dictionary = new KSimpleDictionary(keys, values);
        var dictTypeInfo = (KTypeInfo<KSimpleDictionary>)options.GetTypeInfo(typeof(KSimpleDictionary));
        dictTypeInfo.Serialize(ref writer, dictionary);
    }
    
    /// <summary>
    /// Reads a KDictionary and converts it to the data class.
    /// </summary>
    /// <param name="reader">The serialization reader.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The deserialized data class instance.</returns>
    public override T Read(ref KSerializationReader reader, KSerializerOptions options)
    {
        // Read as KSimpleDictionary first
        var dictTypeInfo = (KTypeInfo<KSimpleDictionary>)options.GetTypeInfo(typeof(KSimpleDictionary));
        var dictionary = dictTypeInfo.Deserialize(ref reader);
        
        if (dictionary == null)
            return default(T)!;
        
        // Create instance of T
        var instance = Activator.CreateInstance<T>();
        
        // Create lookup for properties by column name
        var propertyLookup = new Dictionary<string, PropertyInfo>();
        foreach (var property in _serializableProperties)
        {
            var config = _propertyConfigurations.GetValueOrDefault(property);
            var columnName = config?.ColumnName ?? property.Name;
            propertyLookup[columnName] = property;
        }
        
        // Set property values from dictionary
        for (int i = 0; i < dictionary.Keys.Length; i++)
        {
            var key = (string)dictionary.Keys.GetValue(i)!;
            var value = dictionary.Values.GetValue(i);
            
            if (propertyLookup.TryGetValue(key, out var property))
            {
                var config = _propertyConfigurations.GetValueOrDefault(property);
                
                // Apply property-specific deserialization if configured
                if (config?.Converter != null)
                {
                    // Handle custom converter
                    // This would require more complex implementation
                    SetPropertyValue(property, instance, value);
                }
                else
                {
                    // Use default conversion
                    SetPropertyValue(property, instance, value);
                }
            }
        }
        
        return instance;
    }
    
    private static void SetPropertyValue(PropertyInfo property, T instance, object? value)
    {
        if (value == null)
        {
            if (property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) == null)
            {
                // Cannot set null to non-nullable value type
                return;
            }
            property.SetValue(instance, null);
        }
        else
        {
            // Handle type conversion if needed
            var targetType = property.PropertyType;
            var valueType = value.GetType();
            
            if (targetType.IsAssignableFrom(valueType))
            {
                property.SetValue(instance, value);
            }
            else
            {
                // Try to convert the value
                try
                {
                    var convertedValue = Convert.ChangeType(value, targetType);
                    property.SetValue(instance, convertedValue);
                }
                catch
                {
                    // Conversion failed, skip this property
                    // In a production system, you might want to log this or throw an exception
                }
            }
        }
    }
}
