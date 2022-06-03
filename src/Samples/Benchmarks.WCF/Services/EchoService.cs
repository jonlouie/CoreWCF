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
        public async Task<IEnumerable<SampleData>> EchoSampleData(IEnumerable<SampleData> echoData)
        {
            return await Task.FromResult(echoData);
        }

        // Client receives data from this endpoint
        public async Task<IEnumerable<SampleData>> ReceiveSampleData(int numRecords)
        {
            return await Task.FromResult(DataGenerator.GenerateRecords(numRecords));
        }

        // Client sends data to this endpoint
        public async Task<bool> SendSampleData(IEnumerable<SampleData> echo)
        {
            return await Task.FromResult(true);
        }
    }
}
