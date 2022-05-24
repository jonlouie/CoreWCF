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
            httpBindingBenchmarks.BasicHttpRequestReplyEchoString();
            httpBindingBenchmarks.HttpBindingEchoSampleData1();
        }
    }
}
