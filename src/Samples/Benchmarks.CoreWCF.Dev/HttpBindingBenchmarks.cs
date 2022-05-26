// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Benchmarks.Common.DataContract;
using Benchmarks.Common.Helpers;
using Benchmarks.CoreWCF.Dev.Helpers;
using Benchmarks.CoreWCF.Helpers;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks.CoreWCF.Dev
{
    [SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 10, targetCount: 100)]
    public class HttpBindingBenchmarks
    {
        private readonly IEnumerable<SampleData> _dataList1 = DataGenerator.GenerateRecords(1);
        private readonly IEnumerable<SampleData> _dataList100 = DataGenerator.GenerateRecords(100);
        private readonly IEnumerable<SampleData> _dataList1000 = DataGenerator.GenerateRecords(1000);
        private IWebHost _host;
        private ClientContract.IEchoService _channel;

        [GlobalSetup]
        public void HttpBindingGlobalSetup()
        {
            _host = ServiceHelper.CreateWebHostBuilder<Startup>().Build();
            _host.Start();
            var httpBinding = ClientBindingFactory.GetStandardBasicHttpBinding();
            var factory = new System.ServiceModel.ChannelFactory<ClientContract.IEchoService>(httpBinding,
                new System.ServiceModel.EndpointAddress(new Uri("http://localhost:8080/BasicWcfService/basichttp.svc")));
            _channel = factory.CreateChannel();
            ((System.ServiceModel.Channels.IChannel)_channel).Open();
        }

        [GlobalCleanup]
        public void HttpBindingGlobalCleanup()
        {
            ((System.ServiceModel.Channels.IChannel)_channel).Close();
            _host.Dispose();
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

        #region Startups
        internal class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddServiceModelServices();
            }

            public void Configure(IApplicationBuilder app)
            {
                var binding = ServiceBindingFactory.GetStandardBasicHttpBinding();
                app.UseServiceModel(builder =>
                {
                    builder.AddService<Services.EchoService>();
                    builder.AddServiceEndpoint<Services.EchoService, ServiceContract.IEchoService>(binding, "/BasicWcfService/basichttp.svc");
                });
            }
        }
        #endregion
    }
}
