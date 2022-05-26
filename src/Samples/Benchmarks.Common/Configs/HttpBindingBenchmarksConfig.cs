// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace Benchmarks.Common.Configs
{
    public class HttpBindingBenchmarksConfig : ManualConfig
    {
        public HttpBindingBenchmarksConfig()
        {
            AddColumn(
                StatisticColumn.Min,
                StatisticColumn.Max,
                StatisticColumn.Mean,
                StatisticColumn.Median,
                StatisticColumn.P50,
                StatisticColumn.P67,
                StatisticColumn.P80,
                StatisticColumn.P85,
                StatisticColumn.P90,
                StatisticColumn.P95,
                StatisticColumn.StdDev,
                StatisticColumn.StdErr,
                StatisticColumn.Error);
        }
    }
}
