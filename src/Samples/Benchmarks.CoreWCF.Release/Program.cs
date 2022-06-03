using BenchmarkDotNet.Running;

namespace Benchmarks.CoreWCF.Release
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);

            // Used for debugging
            //var httpBindingBenchmarks = new HttpBindingBenchmarks();
            //httpBindingBenchmarks.HttpBindingGlobalSetup();
            ////httpBindingBenchmarks.HttpBindingEchoSampleDataAsync1000();
            ////httpBindingBenchmarks.HttpBindingReceiveSampleDataAsync1000();
            //httpBindingBenchmarks.HttpBindingSendSampleDataAsync1000();
            //httpBindingBenchmarks.HttpBindingGlobalCleanup();
        }
    }
}
