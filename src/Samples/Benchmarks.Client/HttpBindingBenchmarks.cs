// Licensed to the .NET Foundation under one or more agreements.
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
        public void HttpBindingEchoSampleDataAsync1()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.EchoSampleDataAsync(_dataList1);
        }

        [Benchmark]
        public void HttpBindingEchoSampleDataAsync100()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.EchoSampleDataAsync(_dataList100);
        }

        [Benchmark]
        public void HttpBindingEchoSampleDataAsync1000()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.EchoSampleDataAsync(_dataList1000);
        }

        [Benchmark]
        public void HttpBindingReceiveSampleDataAsync1()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.ReceiveSampleDataAsync(1);
        }

        [Benchmark]
        public void HttpBindingReceiveSampleDataAsync100()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.ReceiveSampleDataAsync(100);
        }

        [Benchmark]
        public void HttpBindingReceiveSampleDataAsync1000()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.ReceiveSampleDataAsync(1000);
        }

        [Benchmark]
        public void HttpBindingSendSampleDataAsync1()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.SendSampleDataAsync(_dataList1);
        }

        [Benchmark]
        public void HttpBindingSendSampleDataAsync100()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.SendSampleDataAsync(_dataList100);
        }

        [Benchmark]
        public void HttpBindingSendSampleDataAsync1000()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.SendSampleDataAsync(_dataList1000);
        }
    }
    public class NonBenchmarkCalls
    {
        private readonly string _benchmarkEndpointAddress = $"http://{Program.HostName}:{Program.Port}/BasicWcfService/basichttp.svc";
        public readonly IEnumerable<SampleData> DataList1 = DataGenerator.GenerateRecords(1);
        public readonly IEnumerable<SampleData> DataList100 = DataGenerator.GenerateRecords(100);
        public readonly IEnumerable<SampleData> DataList1000 = DataGenerator.GenerateRecords(1000);
        private ChannelFactory<ClientContract.IEchoService> _factory;
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
            _factory = new ChannelFactory<ClientContract.IEchoService>(httpBinding, endpointAddress);
            _channel = _factory.CreateChannel();
            ((System.ServiceModel.Channels.IChannel)_channel).Open();
        }

        public void Cleanup()
        {
            ((System.ServiceModel.Channels.IChannel)_channel).Close();
        }

        public void DebugEchoSampleDataAsync1()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.EchoSampleDataAsync(DataList1);
        }

        public void EchoSampleDataStress(IEnumerable<SampleData> dataToEcho, int invocationsPerThread = 1000, int maxThreads = 20)
        {
            var tasks = new List<Task>();
            foreach (int threadId in Enumerable.Range(0, maxThreads))
            {
                var channel = _factory.CreateChannel();
                var t = new Task(async () =>
                {
                    foreach (int invocationNumber in Enumerable.Range(0, invocationsPerThread))
                    {
                        try
                        {
                            ((IClientChannel)channel).Open();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error on channel open: Thread {threadId}, Invocation: {invocationNumber}");
                        }

                        try
                        {
                            // Always save the returned value or the call will be optimized away, preventing benchmark execution
                            var result = await channel.EchoSampleDataAsync(dataToEcho);
                            Console.WriteLine($"Thread {threadId}: Echo {invocationNumber} received");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error on Echo: Thread {threadId}, Invocation: {invocationNumber}");
                        }

                        try
                        {
                            ((IClientChannel)channel).Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error on channel close: Thread {threadId}, Invocation: {invocationNumber}");
                        }
                    }
                });
                tasks.Add(t);
            }
            tasks.ForEach(t => t.Start());
            Task.WaitAll(tasks.ToArray());
        }

        public void ReceiveSampleDataStress(int numRecordsToReceive, int invocationsPerThread = 1000, int maxThreads = 20)
        {
            var tasks = new List<Task>();
            foreach (int threadId in Enumerable.Range(0, maxThreads))
            {
                var channel = _factory.CreateChannel();
                var t = new Task(async () =>
                {
                    foreach (int invocationNumber in Enumerable.Range(0, invocationsPerThread))
                    {
                        try
                        {
                            ((IClientChannel)channel).Open();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error on channel open: Thread {threadId}, Invocation: {invocationNumber}");
                        }

                        try
                        {
                            // Always save the returned value or the call will be optimized away, preventing benchmark execution
                            var result = await _channel.ReceiveSampleDataAsync(numRecordsToReceive);
                            Console.WriteLine($"Thread {threadId}: Data received {invocationNumber}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error on Receive: Thread {threadId}, Invocation: {invocationNumber}");
                        }

                        try
                        {
                            ((IClientChannel)channel).Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error on channel close: Thread {threadId}, Invocation: {invocationNumber}");
                        }
                    }
                });
                tasks.Add(t);
            }
            tasks.ForEach(t => t.Start());
            Task.WaitAll(tasks.ToArray());
        }

        public void SendSampleDataStress(IEnumerable<SampleData> recordsToSend, int invocationsPerThread = 1000, int maxThreads = 20)
        {
            var tasks = new List<Task>();
            foreach (int threadId in Enumerable.Range(0, maxThreads))
            {
                var channel = _factory.CreateChannel();
                var t = new Task(async () =>
                {
                    foreach (int invocationNumber in Enumerable.Range(0, invocationsPerThread))
                    {
                        try
                        {
                            ((IClientChannel)channel).Open();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error on channel open: Thread {threadId}, Invocation: {invocationNumber}");
                        }

                        try
                        {
                            // Always save the returned value or the call will be optimized away, preventing benchmark execution
                            var result = await _channel.SendSampleDataAsync(recordsToSend);
                            Console.WriteLine($"Thread {threadId}: Delivery receipt received");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error on Send: Thread {threadId}, Invocation: {invocationNumber}");
                        }

                        try
                        {
                            ((IClientChannel)channel).Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error on channel close: Thread {threadId}, Invocation: {invocationNumber}");
                        }
                    }
                });
                tasks.Add(t);
            }
            tasks.ForEach(t => t.Start());
            Task.WaitAll(tasks.ToArray());
        }
    }
}
