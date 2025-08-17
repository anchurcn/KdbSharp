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

namespace KdbSharp.Extensions;

/// <summary>
/// Options for configuring DataTable conversion behavior.
/// </summary>
public class DataTableConversionOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the DataTable should be optimized for display purposes.
    /// When true, certain data transformations are applied to make the data more suitable for UI display,
    /// such as converting char[] to string.
    /// </summary>
    public bool ForDisplay { get; set; } = false;

    /// <summary>
    /// Gets the default options instance with ForDisplay set to false.
    /// </summary>
    public static DataTableConversionOptions Default => new();

    /// <summary>
    /// Gets an options instance optimized for display with ForDisplay set to true.
    /// </summary>
    public static DataTableConversionOptions ForDisplayPurpose => new() { ForDisplay = true };
}
