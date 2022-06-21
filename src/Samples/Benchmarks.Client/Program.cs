using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using Benchmarks.Common.DataContract;
using System.Linq;

namespace Benchmarks.Client
{
    public class Program
    {
        public const string HostName = "localhost";
        public const string Port = "8080";

        static void Main(string[] args)
        {
            if (args.ToHashSet().Contains("--job"))
            {
                var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
            }
            else
            {
                var nonBenchmarkCalls = new NonBenchmarkCalls();
                Console.WriteLine("Setup");
                nonBenchmarkCalls.Setup();

                Console.WriteLine("Making the call");
                var command = args[0].ToLower();
                var size = int.Parse(args[1]);
                if (!(args.Length > 2 && int.TryParse(args[2], out int invocationsPerThread)))
                {
                    invocationsPerThread = 1000;
                }

                IEnumerable<SampleData> data;
                if (size == 100)
                {
                    data = nonBenchmarkCalls.DataList100;
                }
                else if (size == 1000)
                {
                    data = nonBenchmarkCalls.DataList1000;
                }
                else
                {
                    data = nonBenchmarkCalls.DataList1;
                    size = 1;
                }

                Console.WriteLine($"Command == {command}");
                Console.WriteLine($"Data size == {size}");
                Console.WriteLine($"Invocations per thread == {invocationsPerThread}");
                switch (command)
                {
                    case "echo":
                        nonBenchmarkCalls.EchoSampleDataStress(data, invocationsPerThread);
                        break;
                    case "receive":
                        nonBenchmarkCalls.ReceiveSampleDataStress(size, invocationsPerThread);
                        break;
                    case "send":
                        nonBenchmarkCalls.SendSampleDataStress(data, invocationsPerThread);
                        break;
                    default:
                        Console.WriteLine("Unrecognized arg");
                        break;
                }

                Console.WriteLine("Cleaning up");
                nonBenchmarkCalls.Cleanup();
            }
        }
    }
}
