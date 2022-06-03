// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Benchmarks.Common.DataContract;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Benchmarks.WCF.ServiceContract
{
    internal static class Constants
    {
        public const string NS = "http://tempuri.org/";
        public const string ECHOSERVICE_NAME = nameof(IEchoService);
        public const string OPERATION_BASE = NS + ECHOSERVICE_NAME + "/";
    }

    [ServiceContract(Namespace = Constants.NS, Name = Constants.ECHOSERVICE_NAME)]
    public interface IEchoService
    {
        [OperationContract(Name = "EchoSampleDataAsync", Action = Constants.OPERATION_BASE + "EchoSampleDataAsync",
            ReplyAction = Constants.OPERATION_BASE + "EchoSampleDataAsyncResponse")]
        Task<IEnumerable<SampleData>> EchoSampleDataAsync(IEnumerable<SampleData> sampleData);

        [OperationContract(Name = "ReceiveSampleDataAsync", Action = Constants.OPERATION_BASE + "ReceiveSampleDataAsync",
            ReplyAction = Constants.OPERATION_BASE + "ReceiveSampleDataAsyncResponse")]
        Task<IEnumerable<SampleData>> ReceiveSampleDataAsync(int numRecords);

        [OperationContract(Name = "SendSampleDataAsync", Action = Constants.OPERATION_BASE + "SendSampleDataAsync",
            ReplyAction = Constants.OPERATION_BASE + "SendSampleDataAsyncResponse")]
        Task<bool> SendSampleDataAsync(IEnumerable<SampleData> echo);
    }
}
