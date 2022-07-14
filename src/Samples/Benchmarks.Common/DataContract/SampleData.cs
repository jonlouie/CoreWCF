// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace Benchmarks.Common.DataContract
{
    [DataContract]
    public class SampleData
    {
        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public bool Flag { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
