// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using Contracts;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Queue;
using CoreWCF.Queue.Common.Configuration;
using CoreWCF.Queue.Common;
using CoreWCF.RabbitMQ.Tests.Fakes;
using CoreWCF.RabbitMQ.Tests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Xunit.Abstractions;

namespace CoreWCF.RabbitMQ.Tests
{
    public class IntegrationTests
    {
        private readonly ITestOutputHelper _output;
        public const string QueueName = "wcfQueue";

        public IntegrationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task ReceiveMessage_ServiceCall_Success()
        {
            IWebHost host = ServiceHelper.CreateWebHostBuilder<Startup>(_output).Build();
            using (host)
            {
                host.Start();
                MessageQueueHelper.SendMessageInQueue();
                var resolver = new DependencyResolverHelper(host);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var testService = resolver.GetService<TestService>();
                Assert.True(testService.ManualResetEvent.Wait(System.TimeSpan.FromSeconds(5)));
            }
        }

        /*
        [Fact(Skip = "Need rabbitmq")]
        public async Task ReceiveMessage()
        {
            var handler = new TestConnectionHandler();
            var factory = new RabbitMqTransportFactory(new NullLoggerFactory(), handler);
            var settings = new QueueOptions { QueueName = QueueName };
            var transport = factory.Create(settings);
            _ = transport.StartAsync();
            MessageQueueHelper.SendMessageInQueue();
            await Task.Delay(1000);
            await transport.StopAsync();
            Assert.Equal(1, handler.CallCount);
        }*/
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Interceptor>();
            services.AddSingleton<TestService>();
            services.AddServiceModelServices();
            services.AddQueueTransport(x =>
                x.QueueName = IntegrationTests.QueueName);
            services.AddServiceModelRabbitMqSupport();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseServiceModel(services =>
            {
                services.AddService<TestService>();
                services.AddServiceEndpoint<TestService, ITestContract>(new RabbitMqBinding(),
                    $"soap.amqp://{IntegrationTests.QueueName}");
            });
        }
    }
}
