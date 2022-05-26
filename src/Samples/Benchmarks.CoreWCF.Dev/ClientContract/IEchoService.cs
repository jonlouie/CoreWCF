// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.ServiceModel;
using Benchmarks.Common.DataContract;

namespace Benchmarks.CoreWCF.Dev.ClientContract
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
        IEnumerable<SampleData> EchoSampleData(IEnumerable<SampleData> sampleData);

        [OperationContract(Name = "ReceiveSampleData", Action = Constants.OPERATION_BASE + "ReceiveSampleData",
            ReplyAction = Constants.OPERATION_BASE + "ReceiveSampleDataResponse")]
        IEnumerable<SampleData> ReceiveSampleData(int numRecords);

        [OperationContract(Name = "SendSampleData", Action = Constants.OPERATION_BASE + "SendSampleData",
            ReplyAction = Constants.OPERATION_BASE + "SendSampleDataResponse")]
        bool SendSampleData(IEnumerable<SampleData> echo);
    }
}
