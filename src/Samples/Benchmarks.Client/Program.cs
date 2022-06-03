using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using Benchmarks.Common.DataContract;

namespace Benchmarks.Client
{
    public class Program
    {
        public const string HostName = "localhost";
        public const string Port = "8080";

        static void Main(string[] args)
        {
            if (args.Length == 0)
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
                switch (command)
                {
                    case "echo":
                        nonBenchmarkCalls.EchoSampleDataStress(data);
                        break;
                    case "receive":
                        nonBenchmarkCalls.ReceiveSampleDataStress(size);
                        break;
                    case "send":
                        nonBenchmarkCalls.SendSampleDataStress(data);
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
