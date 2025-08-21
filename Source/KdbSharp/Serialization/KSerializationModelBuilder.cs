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
using System.Linq.Expressions;
using System.Reflection;
using KdbSharp.Types;

namespace KdbSharp.Serialization;

/// <summary>
/// Provides a fluent API for configuring KDB serialization model similar to EF Core Model Builder.
/// </summary>
public class KSerializationModelBuilder
{
    private readonly KSerializerOptions _options;
    private readonly Dictionary<Type, IEntityTypeConfiguration> _entityConfigurations = new();
    
    internal KSerializationModelBuilder(KSerializerOptions options)
    {
        _options = options;
    }
    
    /// <summary>
    /// Configures the entity of type T.
    /// </summary>
    /// <typeparam name="T">The entity type to configure.</typeparam>
    /// <returns>An EntityTypeBuilder for further configuration.</returns>
    public EntityTypeBuilder<T> Entity<T>() where T : class
    {
        if (!_entityConfigurations.TryGetValue(typeof(T), out var config))
        {
            config = new EntityTypeConfiguration<T>(this);
            _entityConfigurations[typeof(T)] = config;
        }
        return new EntityTypeBuilder<T>((EntityTypeConfiguration<T>)config);
    }
    
    /// <summary>
    /// Configures the entity of type T using a configuration action.
    /// </summary>
    /// <typeparam name="T">The entity type to configure.</typeparam>
    /// <param name="buildAction">The configuration action.</param>
    /// <returns>This builder for method chaining.</returns>
    public KSerializationModelBuilder Entity<T>(Action<EntityTypeBuilder<T>> buildAction) where T : class
    {
        buildAction(Entity<T>());
        return this;
    }
    
    /// <summary>
    /// Applies all entity configurations to the serializer options.
    /// </summary>
    internal void ApplyConfigurations()
    {
        foreach (var config in _entityConfigurations.Values)
        {
            config.Apply(_options);
        }
    }
}

/// <summary>
/// Provides a fluent API for configuring an entity type.
/// </summary>
/// <typeparam name="T">The entity type being configured.</typeparam>
public class EntityTypeBuilder<T> where T : class
{
    private readonly EntityTypeConfiguration<T> _configuration;
    
    internal EntityTypeBuilder(EntityTypeConfiguration<T> configuration)
    {
        _configuration = configuration;
    }
    
    /// <summary>
    /// Configures a property of the entity.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertyExpression">Expression identifying the property.</param>
    /// <returns>A PropertyBuilder for further configuration.</returns>
    public PropertyBuilder<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
    {
        var propertyInfo = GetPropertyInfo(propertyExpression);
        return _configuration.Property<TProperty>(propertyInfo, this);
    }
    
    /// <summary>
    /// Configures a property to be ignored during serialization.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertyExpression">Expression identifying the property.</param>
    /// <returns>This builder for method chaining.</returns>
    public EntityTypeBuilder<T> Ignore<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
    {
        var propertyInfo = GetPropertyInfo(propertyExpression);
        _configuration.Ignore(propertyInfo);
        return this;
    }
    
    /// <summary>
    /// Configures a custom converter for this entity type.
    /// </summary>
    /// <typeparam name="TConverter">The converter type.</typeparam>
    /// <returns>This builder for method chaining.</returns>
    public EntityTypeBuilder<T> HasConverter<TConverter>() where TConverter : KTypeConverter, new()
    {
        _configuration.HasConverter(new TConverter());
        return this;
    }
    
    /// <summary>
    /// Configures a custom converter for this entity type.
    /// </summary>
    /// <param name="converter">The converter instance.</param>
    /// <returns>This builder for method chaining.</returns>
    public EntityTypeBuilder<T> HasConverter(KTypeConverter converter)
    {
        _configuration.HasConverter(converter);
        return this;
    }
    
    /// <summary>
    /// Configures the target KType for this entity.
    /// </summary>
    /// <param name="kType">The target KType.</param>
    /// <returns>This builder for method chaining.</returns>
    public EntityTypeBuilder<T> ToKType(KType kType)
    {
        _configuration.ToKType(kType);
        return this;
    }
    
    /// <summary>
    /// Configures this entity to be serialized as a KDictionary (DataClass pattern).
    /// </summary>
    /// <returns>This builder for method chaining.</returns>
    public EntityTypeBuilder<T> AsKDictionary()
    {
        _configuration.AsKDictionary();
        return this;
    }
    
    /// <summary>
    /// Configures this entity to be serialized as part of a KTable when used in collections.
    /// </summary>
    /// <returns>This builder for method chaining.</returns>
    public EntityTypeBuilder<T> AsKTable()
    {
        _configuration.AsKTable();
        return this;
    }
    
    private PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression memberExpression && 
            memberExpression.Member is PropertyInfo propertyInfo)
        {
            return propertyInfo;
        }
        throw new ArgumentException("Expression must be a property access", nameof(propertyExpression));
    }
}
