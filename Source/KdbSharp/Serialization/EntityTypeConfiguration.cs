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
using System.Reflection;
using KdbSharp.Types;

namespace KdbSharp.Serialization;

/// <summary>
/// Interface for entity type configurations.
/// </summary>
internal interface IEntityTypeConfiguration
{
    /// <summary>
    /// Applies the configuration to the serializer options.
    /// </summary>
    /// <param name="options">The serializer options to configure.</param>
    void Apply(KSerializerOptions options);
}

/// <summary>
/// Configuration for an entity type.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
internal class EntityTypeConfiguration<T> : IEntityTypeConfiguration where T : class
{
    private readonly KSerializationModelBuilder _builder;
    private readonly Dictionary<PropertyInfo, PropertyConfiguration> _propertyConfigurations = new();
    private readonly HashSet<PropertyInfo> _ignoredProperties = new();
    private KTypeConverter? _entityConverter;
    private KType? _entityKType;
    private SerializationStrategy _strategy = SerializationStrategy.Default;
    
    public EntityTypeConfiguration(KSerializationModelBuilder builder)
    {
        _builder = builder;
    }
    
    /// <summary>
    /// Configures a property of the entity.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertyInfo">The property info.</param>
    /// <param name="entityBuilder">The entity builder for chaining.</param>
    /// <returns>A PropertyBuilder for further configuration.</returns>
    public PropertyBuilder<T, TProperty> Property<TProperty>(PropertyInfo propertyInfo, EntityTypeBuilder<T> entityBuilder)
    {
        if (!_propertyConfigurations.TryGetValue(propertyInfo, out var config))
        {
            config = new PropertyConfiguration(propertyInfo);
            _propertyConfigurations[propertyInfo] = config;
        }
        return new PropertyBuilder<T, TProperty>(config, entityBuilder);
    }
    
    /// <summary>
    /// Marks a property to be ignored during serialization.
    /// </summary>
    /// <param name="propertyInfo">The property to ignore.</param>
    public void Ignore(PropertyInfo propertyInfo)
    {
        _ignoredProperties.Add(propertyInfo);
    }
    
    /// <summary>
    /// Sets a custom converter for the entity.
    /// </summary>
    /// <param name="converter">The converter to use.</param>
    public void HasConverter(KTypeConverter converter)
    {
        _entityConverter = converter;
    }
    
    /// <summary>
    /// Sets the target KType for the entity.
    /// </summary>
    /// <param name="kType">The target KType.</param>
    public void ToKType(KType kType)
    {
        _entityKType = kType;
    }
    
    /// <summary>
    /// Configures the entity to be serialized as a KDictionary.
    /// </summary>
    public void AsKDictionary()
    {
        _strategy = SerializationStrategy.KDictionary;
    }
    
    /// <summary>
    /// Configures the entity to be serialized as part of a KTable.
    /// </summary>
    public void AsKTable()
    {
        _strategy = SerializationStrategy.KTable;
    }
    
    /// <summary>
    /// Applies the configuration to the serializer options.
    /// </summary>
    /// <param name="options">The serializer options to configure.</param>
    public void Apply(KSerializerOptions options)
    {
        // If a custom converter is specified, use it directly
        if (_entityConverter != null)
        {
            if (_entityKType.HasValue)
            {
                options.RegisterDefaultConverter<T>(_entityKType.Value, _entityConverter);
            }
            else
            {
                options.RegisterDefaultWriteConverter(t => t == typeof(T), _entityConverter);
            }
            return;
        }
        
        // Apply property-level configurations
        ApplyPropertyConfigurations(options);
        
        // Create strategy-based converter if needed
        var converter = CreateStrategyConverter();
        if (converter != null)
        {
            if (_entityKType.HasValue)
            {
                options.RegisterDefaultConverter<T>(_entityKType.Value, converter);
            }
            else
            {
                options.RegisterDefaultWriteConverter(t => t == typeof(T), converter);
            }
        }
    }
    
    private void ApplyPropertyConfigurations(KSerializerOptions options)
    {
        foreach (var (propertyInfo, config) in _propertyConfigurations)
        {
            // If property has a custom converter, register it
            if (config.Converter != null)
            {
                // Register converter for the property type
                if (config.KType.HasValue)
                {
                    // For now, we'll register as write converter since RegisterDefaultConverter requires generic type
                    options.RegisterDefaultWriteConverter(t => t == propertyInfo.PropertyType, config.Converter);
                    options.RegisterDefaultReadConverter(kt => kt == config.KType.Value, config.Converter);
                }
                else
                {
                    options.RegisterDefaultWriteConverter(t => t == propertyInfo.PropertyType, config.Converter);
                }
            }
            // If property has a target KType but no custom converter, 
            // the system will use predefined converters from PropertyType to KType
            else if (config.KType.HasValue)
            {
                // The predefined converter lookup will happen automatically during serialization
                // We just need to ensure the mapping is available in the options
                // This is handled by the existing converter registration system
            }
        }
    }
    
    private KTypeConverter? CreateStrategyConverter()
    {
        return _strategy switch
        {
            SerializationStrategy.KDictionary => new DataClassToDictionaryConverter<T>(_propertyConfigurations, _ignoredProperties),
            SerializationStrategy.KTable => null, // Will be handled by collection converters
            SerializationStrategy.Default => null, // Use default behavior
            _ => null
        };
    }
}

/// <summary>
/// Configuration for a property.
/// </summary>
internal class PropertyConfiguration
{
    /// <summary>
    /// Gets the property info.
    /// </summary>
    public PropertyInfo PropertyInfo { get; }
    
    /// <summary>
    /// Gets or sets the target KType for this property.
    /// </summary>
    public KType? KType { get; set; }
    
    /// <summary>
    /// Gets or sets the KType for collection elements.
    /// </summary>
    public KType? ElementKType { get; set; }
    
    /// <summary>
    /// Gets or sets the custom converter for this property.
    /// </summary>
    public KTypeConverter? Converter { get; set; }
    
    /// <summary>
    /// Gets or sets the column name for this property.
    /// </summary>
    public string? ColumnName { get; set; }
    
    /// <summary>
    /// Gets or sets whether this property is required.
    /// </summary>
    public bool IsRequired { get; set; }
    
    public PropertyConfiguration(PropertyInfo propertyInfo)
    {
        PropertyInfo = propertyInfo;
    }
}

/// <summary>
/// Serialization strategy for entities.
/// </summary>
internal enum SerializationStrategy
{
    /// <summary>
    /// Use default serialization behavior.
    /// </summary>
    Default,
    
    /// <summary>
    /// Serialize as KDictionary (DataClass pattern).
    /// </summary>
    KDictionary,
    
    /// <summary>
    /// Serialize as part of KTable when used in collections.
    /// </summary>
    KTable
}
