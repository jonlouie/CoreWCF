// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Benchmarks.Common.Helpers;
using Benchmarks.Common.DataContract;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Benchmarks.WCF.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class EchoService : ServiceContract.IEchoService
    {
        private const int MaxRequests = 50000;
        private static int _counter = 0;
        public static int Counter => _counter;

        public async Task<IEnumerable<SampleData>> EchoSampleDataAsync(IEnumerable<SampleData> echoData)
        {
            var result = await Task.FromResult(echoData);
            CounterService.Increment();
            return result;
        }

        // Client receives data from this endpoint
        public async Task<IEnumerable<SampleData>> ReceiveSampleDataAsync(int numRecords)
        {
            var result = await Task.FromResult(DataGenerator.GenerateRecords(numRecords));
            CounterService.Increment();
            return result;
        }

        // Client sends data to this endpoint
        public async Task<bool> SendSampleDataAsync(IEnumerable<SampleData> echo)
        {
            var result = await Task.FromResult(true);
            CounterService.Increment();
            return result;
        }
    }
}
