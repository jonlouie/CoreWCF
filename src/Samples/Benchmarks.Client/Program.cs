using BenchmarkDotNet.Running;
using System;

namespace Benchmarks.Client
{
    public class Program
    {
        public const string HostName = "ec2-35-86-186-181.us-west-2.compute.amazonaws.com";
        public const string Port = "8080";

        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run(typeof(Program).Assembly);

            // Used for debugging
            var debugCalls = new DebugCalls();
            Console.WriteLine("Setup");
            debugCalls.DebugSetup();
            Console.WriteLine("Making the call");
            debugCalls.DebugEchoSampleData1();
            Console.WriteLine("Cleaning up");
            debugCalls.DebugCleanup();
        }
    }
}
