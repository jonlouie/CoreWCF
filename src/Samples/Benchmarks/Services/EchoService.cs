// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Benchmarks.Common.Helpers;
using Benchmarks.Common.DataContract;
using CoreWCF;

namespace Benchmarks.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class EchoService : ServiceContract.IEchoService
    {
        public IEnumerable<SampleData> EchoSampleData(IEnumerable<SampleData> echoData)
        {
            return echoData;
        }

        // Client receives data from this endpoint
        public IEnumerable<SampleData> ReceiveSampleData(int numRecords)
        {
            return DataGenerator.GenerateRecords(numRecords);
        }

        // Client sends data to this endpoint
        public bool SendSampleData(IEnumerable<SampleData> echo)
        {
            return true;
        }
    }
}
