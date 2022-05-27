using BenchmarkDotNet.Running;

namespace Benchmarks.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);

            // Used for debugging
            //var httpBindingBenchmarks = new HttpBindingBenchmarks();
            //httpBindingBenchmarks.HttpBindingGlobalSetup();
            ////httpBindingBenchmarks.HttpBindingEchoSampleData1000();
            ////httpBindingBenchmarks.HttpBindingReceiveSampleData1000();
            //httpBindingBenchmarks.HttpBindingSendSampleData1000();
            //httpBindingBenchmarks.HttpBindingGlobalCleanup();
        }
    }
}
