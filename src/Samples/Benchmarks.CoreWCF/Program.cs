using BenchmarkDotNet.Running;
using Benchmarks.CoreWCF;

namespace Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run(typeof(Program).Assembly);


            var httpBindingBenchmarks = new HttpBindingBenchmarks();
            httpBindingBenchmarks.HttpBindingGlobalSetup();
            //httpBindingBenchmarks.HttpBindingEchoSampleData1000();
            //httpBindingBenchmarks.HttpBindingReceiveSampleData1000();
            httpBindingBenchmarks.HttpBindingSendSampleData1000();
            httpBindingBenchmarks.HttpBindingGlobalCleanup();
        }
    }
}
