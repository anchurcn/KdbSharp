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
using System.Text;

namespace KdbSharp;

/// <summary>
/// Represents the options for a KdbConnection.
/// </summary>
public class KdbConnectionOptions
{
    /// <summary>
    /// Gets or sets the host name or IP address of the Kdb+ server. Default is "localhost".
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the port number of the Kdb+ server. Default is 5000.
    /// </summary>
    public int Port { get; set; } = 5000;

    /// <summary>
    /// Gets or sets the username for authentication. Can be null if not required.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for authentication. Can be null if not required.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use compression for network communication. Default is false.
    /// </summary>
    public bool UseCompression { get; set; }

    /// <summary>
    /// Gets or sets the encoding used for text-based communication. Default is UTF-8.
    /// </summary>
    public Encoding TextEncoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// Gets or sets the maximum chunk size for reading data from the network. Default is 65536 bytes.
    /// </summary>
    public int MaxReadingChunk { get; set; } = 0x10000;

    /// <summary>
    /// Gets or sets the maximum chunk size for writing data to the network. Default is 65536 bytes.
    /// </summary>
    public int MaxWritingChunk { get; set; } = 0x10000;
}
