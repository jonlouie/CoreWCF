using BenchmarkDotNet.Running;
using Benchmarks;

namespace Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run(typeof(Program).Assembly);


            var httpBindingBenchmarks = new HttpBindingBenchmarks();
            httpBindingBenchmarks.HttpBindingGlobalSetup();
            httpBindingBenchmarks.HttpBindingEchoSampleData1000();
            httpBindingBenchmarks.HttpBindingGlobalCleanup();
        }
    }
}
