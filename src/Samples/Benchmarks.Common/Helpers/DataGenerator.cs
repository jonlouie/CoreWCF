// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using Benchmarks.Common.DataContract;

namespace Benchmarks.Common.Helpers
{
    public class DataGenerator
    {
        public static IEnumerable<SampleData> GenerateRecords(int numRecords)
        {
            var echoDataList = new List<SampleData>();
            if (numRecords <= 0)
            {
                return echoDataList;
            }

            for (int i = 0; i < numRecords; i++)
            {
                echoDataList.Add(new SampleData
                {
                    Guid = Guid.NewGuid().ToString(),
                    Id = i,
                    Flag = true,
                    //Message = new string('a', 10)
                    Message = new string('a', 3)
                });
            }

            return echoDataList;
        }
    }
}
