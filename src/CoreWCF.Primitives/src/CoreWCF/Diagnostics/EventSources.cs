// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.Tracing;

namespace CoreWCF.Diagnostics
{
    [EventSource(Name = "Demo")]
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
        public void BufferTaken(int bufferSize) => WriteEvent(BufferEvent.Taken, bufferSize);

        [Event(BufferEvent.Returned)]
        public void BufferReturned(int bufferSize) => WriteEvent(BufferEvent.Returned, bufferSize);

        [Event(BufferEvent.Miss)]
        public void BufferMiss(int bufferSize) => WriteEvent(BufferEvent.Miss, bufferSize);

        [Event(BufferEvent.NotFound)]
        public void BufferNotFound(int bufferSize) => WriteEvent(BufferEvent.NotFound, bufferSize);

        [Event(BufferEvent.PoolNotFound)]
        public void BufferPoolNotFound(int bufferSize) => WriteEvent(BufferEvent.PoolNotFound, bufferSize);

        [Event(BufferEvent.ReturnFailed)]
        public void BufferReturnFailed(int bufferSize) => WriteEvent(BufferEvent.ReturnFailed, bufferSize);
    }
}
