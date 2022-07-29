// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.Tracing;

namespace CoreWCF.Diagnostics
{
    [EventSource(Name = "BufferEvents")]
    public class BufferEventSource : EventSource
    {
        private class BufferEvent
        {
            public const int Taken = 1;
            public const int Returned = 2;
            public const int Miss = 3;
            public const int NotFound = 4;
            public const int PoolNotFound = 5;
            public const int ReturnFailed = 6;
        }
        public static BufferEventSource Log { get; } = new();

        [Event(BufferEvent.Taken)]
        public void BufferTaken(int poolHashCode, int arrayHashCode, int bufferSize, int bufferPeak, int bufferLimit, int misses)
            => WriteEvent(BufferEvent.Taken, poolHashCode, arrayHashCode, bufferSize, bufferPeak, bufferLimit, misses);

        [Event(BufferEvent.Returned)]
        public void BufferReturned(int poolHashCode, int arrayHashCode, int bufferSize, int bufferPeak, int bufferLimit, int misses)
            => WriteEvent(BufferEvent.Returned, poolHashCode, arrayHashCode, bufferSize, bufferPeak, bufferLimit, misses);

        [Event(BufferEvent.Miss)]
        public void BufferMiss(int poolHashCode, int bufferSize, int bufferPeak, int bufferLimit, int misses)
            => WriteEvent(BufferEvent.Miss, poolHashCode, bufferSize, bufferPeak, bufferLimit, misses);

        [Event(BufferEvent.NotFound)]
        public void BufferNotFound(int poolHashCode, int bufferSize, int bufferPeak, int bufferLimit, int misses)
            => WriteEvent(BufferEvent.NotFound, poolHashCode, bufferSize, bufferPeak, bufferLimit, misses);

        [Event(BufferEvent.PoolNotFound)]
        public void BufferPoolNotFound(int bufferSize)
            => WriteEvent(BufferEvent.PoolNotFound, bufferSize);

        [Event(BufferEvent.ReturnFailed)]
        public void BufferReturnFailed(int poolHashCode, int arrayHashCode, int bufferSize, int bufferPeak, int bufferLimit, int misses)
            => WriteEvent(BufferEvent.ReturnFailed, poolHashCode, arrayHashCode, bufferSize, bufferPeak, bufferLimit, misses);
    }
}
