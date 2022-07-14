using BenchmarkDotNet.Running;
using System;
using System.ServiceModel;
using Benchmarks.WCF.Services;
using Benchmarks.Common.Helpers;

namespace Benchmarks.WCF
{
    internal class Program
    {
        private static ServiceHost _host;

        static void Main(string[] args)
        {
            _host = new ServiceHost(typeof(EchoService));
            _host.Open();
            Console.WriteLine("Server started! Press Enter to close.");
            ServerRepl.Start();
            _host.Close();


            //var summary = BenchmarkRunner.Run(typeof(Program).Assembly);

            // Used for debugging
            //var httpBindingBenchmarks = new HttpBindingBenchmarks();
            //httpBindingBenchmarks.HttpBindingGlobalSetup();
            //httpBindingBenchmarks.HttpBindingEchoSampleDataAsync1000();
            //httpBindingBenchmarks.HttpBindingGlobalCleanup();
        }
    }
}
