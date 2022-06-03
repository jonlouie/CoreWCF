using BenchmarkDotNet.Running;
using System;

namespace Benchmarks.Client
{
    public class Program
    {
        public const string HostName = "localhost";
        public const string Port = "8080";

        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run(typeof(Program).Assembly);

            // Used for debugging
            var nonBenchmarkCalls = new NonBenchmarkCalls();
            Console.WriteLine("Setup");
            nonBenchmarkCalls.Setup();

            Console.WriteLine("Making the call");
            nonBenchmarkCalls.EchoSampleDataStress(nonBenchmarkCalls.DataList100);
            nonBenchmarkCalls.ReceiveSampleDataStress(100);
            nonBenchmarkCalls.SendSampleDataStress(nonBenchmarkCalls.DataList100);

            Console.WriteLine("Cleaning up");
            nonBenchmarkCalls.Cleanup();
        }
    }
}
