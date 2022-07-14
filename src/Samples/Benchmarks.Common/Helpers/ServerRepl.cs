// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Benchmarks.Common.Helpers
{
    public class ServerRepl
    {
        public static void Start()
        {
            var isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("Awaiting next command...");
                var input = Console.ReadLine().Trim().ToLower();
                Console.WriteLine($"Input: {input}");

                switch (input)
                {
                    case "":
                    case null:
                        Console.WriteLine("Exiting...");
                        isRunning = false;
                        break;
                    case "g":
                        Console.WriteLine("Running garbage collector...");
                        GC.Collect();
                        break;
                    case "s":
                        Console.WriteLine("Starting counter...");
                        CounterService.Start();
                        break;
                    case "t":
                        Console.WriteLine("Stopping counter...");
                        CounterService.Stop();
                        break;
                    default:
                        break;
                }
                Console.WriteLine();
            }
        }
    }
}
