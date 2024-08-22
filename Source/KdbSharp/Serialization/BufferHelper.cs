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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KdbSharp.Serialization
{
    /// <summary>
    /// Make MemoryMarshal easier to use. (Extension methods)
    /// </summary>
    public static class BufferHelper
    {
        // As : Different underlying type.
        // Get: The same underlying type.
        public static ref T AsRef<T>(this Span<byte> span) where T : struct
            => ref MemoryMarshal.AsRef<T>(span);
        public static ref T GetRef<T>(this Span<T> span) where T : struct
            => ref MemoryMarshal.GetReference(span);

        public static Span<T> GetSpan<T>(this ref T value) where T : struct
            => MemoryMarshal.CreateSpan(ref value, 1);
        public static Span<byte> AsSpan<T>(this ref T value) where T : struct
            => MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1));

        public static Span<TTo> Cast<TFrom, TTo>(this Span<TFrom> span) where TFrom : struct where TTo : struct
            => MemoryMarshal.Cast<TFrom, TTo>(span);
        public static Span<byte> CastToBytes<T>(this Span<T> span) where T : struct
            => MemoryMarshal.Cast<T, byte>(span);

        public static ReadOnlySpan<TTo> Cast<TFrom, TTo>(this ReadOnlySpan<TFrom> span) where TFrom : struct where TTo : struct
            => MemoryMarshal.Cast<TFrom, TTo>(span);
    }

    public static class SerializationHelper
    {
        public static void FlipGuidTop3Parts(this ref Guid guid)
        {
            var span = guid.AsSpan();
            span.Slice(0, 4).Reverse();
            span.Slice(4, 2).Reverse();
            span.Slice(6, 2).Reverse();
        }
    }
}
