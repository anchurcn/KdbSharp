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

public static class StreamExtensions
{
    public static async ValueTask PopulateBufferFromStreamAsync(Stream stream, Memory<byte> buffer, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (!stream.CanRead)
        {
            throw new InvalidOperationException("Stream is not readable.");
        }

        int bytesRead = 0;
        while (bytesRead < buffer.Length)
        {
            int bytesReadThisIteration = await stream.ReadAsync(buffer[bytesRead..], cancellationToken);
            if (bytesReadThisIteration == 0)
            {
                throw new EndOfStreamException("Reached the end of stream unexpectedly.");
            }
            bytesRead += bytesReadThisIteration;

            // Update progress if a progress object is provided
            progress?.Report(bytesRead);
        }
    }

    public static ValueTask PopulateMemory(this Stream stream, Memory<byte> buffer, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        return PopulateBufferFromStreamAsync(stream, buffer, progress, cancellationToken);
    }

    public static ValueTask PopulateMemory(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return PopulateBufferFromStreamAsync(stream, buffer, null, cancellationToken);
    }
}
