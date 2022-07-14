// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using Benchmarks.Common.Helpers;
using Benchmarks.Common.DataContract;
using CoreWCF;

namespace Benchmarks.CoreWCF.Release.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class EchoService : ServiceContract.IEchoService
    {
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
