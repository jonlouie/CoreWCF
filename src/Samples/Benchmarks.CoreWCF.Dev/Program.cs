using BenchmarkDotNet.Running;

namespace Benchmarks.CoreWCF.Dev
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);

            // Used for debugging
            //var httpBindingBenchmarks = new HttpBindingBenchmarks();
            //httpBindingBenchmarks.HttpBindingGlobalSetup();
            //httpBindingBenchmarks.HttpBindingEchoSampleDataAsync1000();
            //httpBindingBenchmarks.HttpBindingGlobalCleanup();
        }
    }
}
