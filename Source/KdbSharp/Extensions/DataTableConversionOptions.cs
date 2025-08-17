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

using System.Text.Json;

namespace KdbSharp.Extensions;

/// <summary>
/// Options for configuring DataTable conversion behavior.
/// </summary>
public class DataTableConversionOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the DataTable should be optimized for display purposes.
    /// When true, certain data transformations are applied to make the data more suitable for UI display,
    /// such as converting char[] to string and arrays to JSON strings.
    /// </summary>
    public bool ForDisplay { get; set; } = false;

    /// <summary>
    /// Gets or sets the JSON serializer options used when converting arrays to strings for display.
    /// If null, default options will be used.
    /// </summary>
    public JsonSerializerOptions? JsonOptions { get; set; }

    /// <summary>
    /// Gets the default options instance with ForDisplay set to false.
    /// </summary>
    public static DataTableConversionOptions Default => new();

    /// <summary>
    /// Gets an options instance optimized for display with ForDisplay set to true.
    /// </summary>
    public static DataTableConversionOptions ForDisplayPurpose => new() { ForDisplay = true };

    /// <summary>
    /// Converts a value to a display-friendly format if ForDisplay is enabled.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted value or the original value if no conversion is needed.</returns>
    public object? ConvertForDisplay(object? value)
    {
        if (!ForDisplay || value == null)
            return value;

        // Convert char[] to string
        if (value is char[] charArray)
        {
            return new string(charArray);
        }

        // Convert arrays (except char[] and string) to JSON string
        if (value.GetType().IsArray && value is not char[] && value is not string)
        {
            try
            {
                return JsonSerializer.Serialize(value, JsonOptions);
            }
            catch (JsonException)
            {
                // If JSON serialization fails, fall back to ToString()
                return value.ToString();
            }
        }

        return value;
    }

    /// <summary>
    /// Gets the display-friendly type for a given type when ForDisplay is enabled.
    /// </summary>
    /// <param name="originalType">The original type.</param>
    /// <returns>The display-friendly type or the original type if no conversion is needed.</returns>
    public Type GetDisplayType(Type originalType)
    {
        if (!ForDisplay)
            return originalType;

        // Convert char[] to string type
        if (originalType == typeof(char[]))
        {
            return typeof(string);
        }

        // Convert array types (except char[] and string) to string type
        if (originalType.IsArray && originalType != typeof(char[]) && originalType != typeof(string))
        {
            return typeof(string);
        }

        return originalType;
    }
}
