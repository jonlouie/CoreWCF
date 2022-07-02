// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Threading;

namespace Benchmarks.Common.Helpers
{
    public static class CounterService
    {
        private static Stopwatch _stopwatch = new Stopwatch();
        private static int _count = 0;
        private static bool _isRunning = false;

        public static double Rps { get; set; }

        public static void Start()
        {
            _isRunning = true;
            _stopwatch.Start();
            _count = 0;
        }

        public static void Stop()
        {
            _isRunning = false;
            _stopwatch.Stop();

            Console.WriteLine($"Time elapsed: {_stopwatch.Elapsed.TotalSeconds}s");
            Console.WriteLine($"Requests: {_count}");
            Console.WriteLine($"RPS: {(int)(_count / _stopwatch.Elapsed.TotalSeconds)}");
        }

        public static void Increment()
        {
            if (_isRunning)
            {
                Interlocked.Increment(ref _count);
            }
        }
    }
}
