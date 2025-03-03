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
/// Provides extension methods for working with streams.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Asynchronously populates the provided buffer with data read from the stream.
    /// </summary>
    /// <param name="stream">The stream to read data from.</param>
    /// <param name="buffer">The buffer to populate with data.</param>
    /// <param name="progress">An optional progress reporter to report the number of bytes read.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the stream is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the stream is not readable.</exception>
    /// <exception cref="EndOfStreamException">Thrown when the end of the stream is reached unexpectedly.</exception>
    public static async ValueTask PopulateBufferFromStreamAsync(Stream stream, Memory<byte> buffer, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (!stream.CanRead)
        {
            throw new InvalidOperationException("Stream is not readable.");
        }

        var bytesRead = 0;
        while (bytesRead < buffer.Length)
        {
            var bytesReadThisIteration = await stream.ReadAsync(buffer[bytesRead..], cancellationToken).ConfigureAwait(false);
            if (bytesReadThisIteration == 0)
            {
                throw new EndOfStreamException("Reached the end of stream unexpectedly.");
            }

            bytesRead += bytesReadThisIteration;

            // Update progress if a progress object is provided
            progress?.Report(bytesRead);
        }
    }

    /// <summary>
    /// Asynchronously populates the provided buffer with data read from the stream.
    /// </summary>
    /// <param name="stream">The stream to read data from.</param>
    /// <param name="buffer">The buffer to populate with data.</param>
    /// <param name="progress">An optional progress reporter to report the number of bytes read.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    public static ValueTask PopulateMemoryAsync(this Stream stream, Memory<byte> buffer, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
        => PopulateBufferFromStreamAsync(stream, buffer, progress, cancellationToken);

    /// <summary>
    /// Asynchronously populates the provided buffer with data read from the stream.
    /// </summary>
    /// <param name="stream">The stream to read data from.</param>
    /// <param name="buffer">The buffer to populate with data.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    public static ValueTask PopulateMemoryAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
        => PopulateBufferFromStreamAsync(stream, buffer, null, cancellationToken);
}
