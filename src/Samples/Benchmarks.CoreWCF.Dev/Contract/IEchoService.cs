// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using Benchmarks.Common.DataContract;
using CoreWCF;

namespace Benchmarks.CoreWCF.Dev.Contract
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
        [OperationContract(Name = "EchoSampleData", Action = Constants.OPERATION_BASE + "EchoSampleData",
            ReplyAction = Constants.OPERATION_BASE + "EchoSampleDataResponse")]
        [System.ServiceModel.OperationContract(Name = "EchoSampleData", Action = Constants.OPERATION_BASE + "EchoSampleData",
            ReplyAction = Constants.OPERATION_BASE + "EchoSampleDataResponse")]
        Task<IEnumerable<SampleData>> EchoSampleData(IEnumerable<SampleData> sampleData);

        [OperationContract(Name = "ReceiveSampleData", Action = Constants.OPERATION_BASE + "ReceiveSampleData",
            ReplyAction = Constants.OPERATION_BASE + "ReceiveSampleDataResponse")]
        [System.ServiceModel.OperationContract(Name = "ReceiveSampleData", Action = Constants.OPERATION_BASE + "ReceiveSampleData",
            ReplyAction = Constants.OPERATION_BASE + "ReceiveSampleDataResponse")]
        Task<IEnumerable<SampleData>> ReceiveSampleData(int numRecords);

        [OperationContract(Name = "SendSampleData", Action = Constants.OPERATION_BASE + "SendSampleData",
            ReplyAction = Constants.OPERATION_BASE + "SendSampleDataResponse")]
        [System.ServiceModel.OperationContract(Name = "SendSampleData", Action = Constants.OPERATION_BASE + "SendSampleData",
            ReplyAction = Constants.OPERATION_BASE + "SendSampleDataResponse")]
        Task<bool> SendSampleData(Task<IEnumerable<SampleData>> echo);
    }
}
