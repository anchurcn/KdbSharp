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

namespace KdbSharp.Serialization;

/// <summary>
/// Provides a fluent API for configuring a property.
/// </summary>
/// <typeparam name="TProperty">The property type.</typeparam>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class PropertyBuilder<TEntity, TProperty> where TEntity : class
{
    private readonly PropertyConfiguration _configuration;
    private readonly EntityTypeBuilder<TEntity> _entityBuilder;

    internal PropertyBuilder(PropertyConfiguration configuration, EntityTypeBuilder<TEntity> entityBuilder)
    {
        _configuration = configuration;
        _entityBuilder = entityBuilder;
    }
    
    /// <summary>
    /// Configures the target KType for this property.
    /// When specified, the system will use the predefined converter from PropertyType to the specified KType.
    /// </summary>
    /// <param name="kType">The target KType.</param>
    /// <returns>The entity builder for method chaining.</returns>
    public EntityTypeBuilder<TEntity> HasKType(KType kType)
    {
        _configuration.KType = kType;
        return _entityBuilder;
    }

    /// <summary>
    /// Configures a custom converter for this property.
    /// This takes precedence over HasKType configuration.
    /// </summary>
    /// <typeparam name="TConverter">The converter type.</typeparam>
    /// <returns>The entity builder for method chaining.</returns>
    public EntityTypeBuilder<TEntity> HasConverter<TConverter>() where TConverter : KTypeConverter, new()
    {
        _configuration.Converter = new TConverter();
        return _entityBuilder;
    }

    /// <summary>
    /// Configures a custom converter for this property.
    /// This takes precedence over HasKType configuration.
    /// </summary>
    /// <param name="converter">The converter instance.</param>
    /// <returns>The entity builder for method chaining.</returns>
    public EntityTypeBuilder<TEntity> HasConverter(KTypeConverter converter)
    {
        _configuration.Converter = converter;
        return _entityBuilder;
    }

    /// <summary>
    /// Configures the column name for this property when serialized.
    /// </summary>
    /// <param name="name">The column name.</param>
    /// <returns>The entity builder for method chaining.</returns>
    public EntityTypeBuilder<TEntity> HasColumnName(string name)
    {
        _configuration.ColumnName = name;
        return _entityBuilder;
    }

    /// <summary>
    /// Configures whether this property is required.
    /// </summary>
    /// <param name="required">True if the property is required.</param>
    /// <returns>The entity builder for method chaining.</returns>
    public EntityTypeBuilder<TEntity> IsRequired(bool required = true)
    {
        _configuration.IsRequired = required;
        return _entityBuilder;
    }

    /// <summary>
    /// Configures the KType for elements when this property is an array or collection.
    /// </summary>
    /// <param name="elementKType">The KType for collection elements.</param>
    /// <returns>The entity builder for method chaining.</returns>
    public EntityTypeBuilder<TEntity> HasElementKType(KType elementKType)
    {
        _configuration.ElementKType = elementKType;
        return _entityBuilder;
    }
}
