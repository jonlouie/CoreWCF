﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Benchmarks.Common.Configs;
using Benchmarks.Common.DataContract;
using Benchmarks.Common.Helpers;

namespace Benchmarks.Client
{
    [Config(typeof(HttpBindingBenchmarksConfig))]
    [SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 0, targetCount: 100)]
    public class HttpBindingBenchmarks
    {
        //private const string HostName = "localhost";
        //private const string Port = "8080";
        private readonly string _benchmarkEndpointAddress = $"http://{Program.HostName}:{Program.Port}/BasicWcfService/basichttp.svc";
        private readonly IEnumerable<SampleData> _dataList1 = DataGenerator.GenerateRecords(1);
        private readonly IEnumerable<SampleData> _dataList100 = DataGenerator.GenerateRecords(100);
        private readonly IEnumerable<SampleData> _dataList1000 = DataGenerator.GenerateRecords(1000);
        private ClientContract.IEchoService _channel;

        [GlobalSetup]
        public void HttpBindingGlobalSetup()
        {
            var httpBinding = new BasicHttpBinding
            {
                SendTimeout = TimeSpan.FromMinutes(20.0),
                ReceiveTimeout = TimeSpan.FromMinutes(20.0),
                OpenTimeout = TimeSpan.FromMinutes(20.0),
                CloseTimeout = TimeSpan.FromMinutes(20.0),
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue
            };
            var endpointAddress = new EndpointAddress(new Uri(_benchmarkEndpointAddress));
            var factory = new ChannelFactory<ClientContract.IEchoService>(httpBinding, endpointAddress);
            _channel = factory.CreateChannel();
            ((System.ServiceModel.Channels.IChannel)_channel).Open();
        }

        [GlobalCleanup]
        public void HttpBindingGlobalCleanup()
        {
            ((System.ServiceModel.Channels.IChannel)_channel).Close();
        }

        [Benchmark]
        public void HttpBindingEchoSampleData1()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.EchoSampleData(_dataList1);
        }

        [Benchmark]
        public void HttpBindingEchoSampleData100()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.EchoSampleData(_dataList100);
        }

        [Benchmark]
        public void HttpBindingEchoSampleData1000()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.EchoSampleData(_dataList1000);
        }

        [Benchmark]
        public void HttpBindingReceiveSampleData1()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.ReceiveSampleData(1);
        }

        [Benchmark]
        public void HttpBindingReceiveSampleData100()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.ReceiveSampleData(100);
        }

        [Benchmark]
        public void HttpBindingReceiveSampleData1000()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.ReceiveSampleData(1000);
        }

        [Benchmark]
        public void HttpBindingSendSampleData1()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.SendSampleData(_dataList1);
        }

        [Benchmark]
        public void HttpBindingSendSampleData100()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.SendSampleData(_dataList100);
        }

        [Benchmark]
        public void HttpBindingSendSampleData1000()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.SendSampleData(_dataList1000);
        }
    }
    public class NonBenchmarkCalls
    {
        private readonly string _benchmarkEndpointAddress = $"http://{Program.HostName}:{Program.Port}/BasicWcfService/basichttp.svc";
        public readonly IEnumerable<SampleData> DataList1 = DataGenerator.GenerateRecords(1);
        public readonly IEnumerable<SampleData> DataList100 = DataGenerator.GenerateRecords(100);
        public readonly IEnumerable<SampleData> DataList1000 = DataGenerator.GenerateRecords(1000);
        private ClientContract.IEchoService _channel;

        public void Setup()
        {
            var httpBinding = new BasicHttpBinding
            {
                SendTimeout = TimeSpan.FromMinutes(20.0),
                ReceiveTimeout = TimeSpan.FromMinutes(20.0),
                OpenTimeout = TimeSpan.FromMinutes(20.0),
                CloseTimeout = TimeSpan.FromMinutes(20.0),
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue
            };
            var endpointAddress = new EndpointAddress(new Uri(_benchmarkEndpointAddress));
            var factory = new ChannelFactory<ClientContract.IEchoService>(httpBinding, endpointAddress);
            _channel = factory.CreateChannel();
            ((System.ServiceModel.Channels.IChannel)_channel).Open();
        }

        public void Cleanup()
        {
            ((System.ServiceModel.Channels.IChannel)_channel).Close();
        }

        public void DebugEchoSampleData1()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.EchoSampleData(DataList1);
        }

        public void EchoSampleDataForever(IEnumerable<SampleData> dataToEcho, int maxThreads = 20)
        {
            var tasks = new List<Task>();
            foreach (int i in Enumerable.Range(0, maxThreads))
            {
                tasks.Add(Task.Run(() =>
                {
                    int threadId = i;
                    while (true)
                    {
                        // Always save the returned value or the call will be optimized away, preventing benchmark execution
                        var result = _channel.EchoSampleData(dataToEcho);
                        Console.WriteLine($"Thread {threadId}: Echo received");
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        public void ReceiveSampleDataForever(int numRecordsToReceive, int maxThreads = 20)
        {
            var tasks = new List<Task>();
            foreach (int i in Enumerable.Range(0, maxThreads))
            {
                tasks.Add(Task.Run(() =>
                {
                    int threadId = i;
                    while (true)
                    {
                        // Always save the returned value or the call will be optimized away, preventing benchmark execution
                        var result = _channel.ReceiveSampleData(numRecordsToReceive);
                        Console.WriteLine($"Thread {threadId}: Data received");
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        public void SendSampleDataForever(IEnumerable<SampleData> recordsToSend, int maxThreads = 20)
        {
            var tasks = new List<Task>();
            foreach (int i in Enumerable.Range(0, maxThreads))
            {
                tasks.Add(Task.Run(() =>
                {
                    int threadId = i;
                    while (true)
                    {
                        // Always save the returned value or the call will be optimized away, preventing benchmark execution
                        var result = _channel.SendSampleData(recordsToSend);
                        Console.WriteLine($"Thread {threadId}: Delivery receipt received");
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}
