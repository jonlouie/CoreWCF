// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net;
using System.Threading.Tasks;
using Contracts;
using CoreWCF.Channels;
using CoreWCF.Channels.Configuration;
using CoreWCF.Configuration;
using CoreWCF.Queue.Common.Configuration;
using CoreWCF.RabbitMQ.Tests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Xunit;
using Xunit.Abstractions;

namespace CoreWCF.RabbitMQ.Tests
{
    public class IntegrationTests : IClassFixture<ConnectionSettingsFixture>
    {
        private readonly ITestOutputHelper _output;
        public static ConnectionSettingsFixture ConnectionSettingsFixture;

        public IntegrationTests(ConnectionSettingsFixture connectionSettingsFixture, ITestOutputHelper output)
        {
            _output = output;
            ConnectionSettingsFixture = connectionSettingsFixture;
        }

        [Fact]
        public void ClassicQueueWithTls_ReceiveMessage_Success()
        {
            IWebHost host = ServiceHelper.CreateWebHostBuilder<ClassicQueueWithTLSStartup>(_output).Build();
            using (host)
            {
                // When the host is started, queues are created if they do not already exist.
                // By waiting for the host to initialize completely, we avoid the race condition
                // of trying to access a queue that has not finished creation.
                Task.WaitAll(Task.Run(() => host.Start()));

                MessageQueueHelper.SendMessageToQueue(ClassicQueueWithTLSStartup.ConnectionSettings);

                var resolver = new DependencyResolverHelper(host);
                var testService = resolver.GetService<TestService>();
                Assert.True(testService.ManualResetEvent.Wait(System.TimeSpan.FromSeconds(5)));
            }
        }

        [Fact]
        public void DefaultClassicQueueConfiguration_ReceiveMessage_Success()
        {
            IWebHost host = ServiceHelper.CreateWebHostBuilder<DefaultClassicQueueStartup>(_output).Build();
            using (host)
            {
                // When the host is started, queues are created if they do not already exist.
                // By waiting for the host to initialize completely, we avoid the race condition
                // of trying to access a queue that has not finished creation.
                Task.WaitAll(Task.Run(() => host.Start()));

                MessageQueueHelper.SendMessageToQueue(DefaultClassicQueueStartup.ConnectionSettings);

                var resolver = new DependencyResolverHelper(host);
                var testService = resolver.GetService<TestService>();
                Assert.True(testService.ManualResetEvent.Wait(System.TimeSpan.FromSeconds(5)));
            }
        }

        [Fact]
        public void DefaultQuorumQueueConfiguration_ReceiveMessage_Success()
        {
            IWebHost host = ServiceHelper.CreateWebHostBuilder<DefaultQuorumQueueStartup>(_output).Build();
            using (host)
            {
                // When the host is started, queues are created if they do not already exist.
                // By waiting for the host to initialize completely, we avoid the race condition
                // of trying to access a queue that has not finished creation.
                Task.WaitAll(Task.Run(() => host.Start()));

                MessageQueueHelper.SendMessageToQueue(DefaultQuorumQueueStartup.ConnectionSettings);

                var resolver = new DependencyResolverHelper(host);
                var testService = resolver.GetService<TestService>();
                Assert.True(testService.ManualResetEvent.Wait(System.TimeSpan.FromSeconds(5)));
            }
        }

        [Fact]
        public void DefaultQueueConfiguration_ReceiveMessage_Success()
        {
            IWebHost host = ServiceHelper.CreateWebHostBuilder<DefaultQueueStartup>(_output).Build();
            using (host)
            {
                // When the host is started, queues are created if they do not already exist.
                // By waiting for the host to initialize completely, we avoid the race condition
                // of trying to access a queue that has not finished creation.
                Task.WaitAll(Task.Run(() => host.Start()));

                MessageQueueHelper.SendMessageToQueue(DefaultQueueStartup.ConnectionSettings);

                var resolver = new DependencyResolverHelper(host);
                var testService = resolver.GetService<TestService>();
                Assert.True(testService.ManualResetEvent.Wait(System.TimeSpan.FromSeconds(5)));
            }
        }
    }
    
    public class ClassicQueueWithTLSStartup
    {
        private static readonly string _userName = IntegrationTests.ConnectionSettingsFixture.UserNameForSecureUri;
        private static readonly string _password = IntegrationTests.ConnectionSettingsFixture.PasswordForSecureUri;
        public static readonly Uri Uri = IntegrationTests.ConnectionSettingsFixture.SecureUri;
        public static RabbitMqConnectionSettings ConnectionSettings =>
            RabbitMqConnectionSettings.FromUri(Uri, new NetworkCredential(_userName, _password));

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TestService>();
            services.AddServiceModelServices();
            services.AddQueueTransport();
            services.AddServiceModelRabbitMqSupport();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseServiceModel(services =>
            {
                services.AddService<TestService>();
                services.AddServiceEndpoint<TestService, ITestContract>(
                    new RabbitMqBinding
                    {
                        SslOption = new SslOption
                        {
                            ServerName = Uri.Host,
                            Enabled = true
                        },
                        Credentials = new NetworkCredential(_userName, _password),
                        QueueConfiguration = new ClassicQueueConfiguration().AsTemporaryQueue()
                    },
                    Uri);
            });
        }
    }
    
    public class DefaultClassicQueueStartup
    {
        private static readonly string _queueName = "corewcf-test-default-classic-queue";
        private static readonly string _queueKey = "corewcf-test-default-classic-key";
        private static readonly string _host = IntegrationTests.ConnectionSettingsFixture.StandardHost;
        private static readonly int _port = IntegrationTests.ConnectionSettingsFixture.StandardHostPort;
        private static readonly string _userName = IntegrationTests.ConnectionSettingsFixture.UserNameForStandardHost;
        private static readonly string _password = IntegrationTests.ConnectionSettingsFixture.PasswordForStandardHost;
        public static Uri Uri = new($"net.amqp://{_host}:{_port}/amq.direct/{_queueName}#{_queueKey}");
        public static RabbitMqConnectionSettings ConnectionSettings => RabbitMqConnectionSettings.FromUri(Uri);

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TestService>();
            services.AddServiceModelServices();
            services.AddQueueTransport();
            services.AddServiceModelRabbitMqSupport();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseServiceModel(services =>
            {
                services.AddService<TestService>();
                services.AddServiceEndpoint<TestService, ITestContract>(
                    new RabbitMqBinding
                    {
                        Credentials = new NetworkCredential(_userName, _password),
                        QueueConfiguration = new QuorumQueueConfiguration()
                    },
                    Uri);
            });
        }
    }
    
    public class DefaultQuorumQueueStartup
    {
        private static readonly string _queueName = "corewcf-test-default-quorum-queue";
        private static readonly string _queueKey = "corewcf-test-default-quorum-key";
        private static readonly string _host = IntegrationTests.ConnectionSettingsFixture.StandardHost;
        private static readonly int _port = IntegrationTests.ConnectionSettingsFixture.StandardHostPort;
        private static readonly string _userName = IntegrationTests.ConnectionSettingsFixture.UserNameForStandardHost;
        private static readonly string _password = IntegrationTests.ConnectionSettingsFixture.PasswordForStandardHost;
        public static Uri Uri = new($"net.amqp://{_host}:{_port}/amq.direct/{_queueName}#{_queueKey}");
        public static RabbitMqConnectionSettings ConnectionSettings => RabbitMqConnectionSettings.FromUri(Uri);

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TestService>();
            services.AddServiceModelServices();
            services.AddQueueTransport();
            services.AddServiceModelRabbitMqSupport();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseServiceModel(services =>
            {
                services.AddService<TestService>();
                services.AddServiceEndpoint<TestService, ITestContract>(
                    new RabbitMqBinding
                    {
                        Credentials = new NetworkCredential(_userName, _password),
                        QueueConfiguration = new QuorumQueueConfiguration()
                    },
                    Uri);
            });
        }
    }
    
    public class DefaultQueueStartup
    {
        private static readonly string _queueName = "corewcf-test-default-queue";
        private static readonly string _queueKey = "corewcf-test-default-key";
        private static readonly string _host = IntegrationTests.ConnectionSettingsFixture.StandardHost;
        private static readonly int _port = IntegrationTests.ConnectionSettingsFixture.StandardHostPort;
        private static readonly string _userName = IntegrationTests.ConnectionSettingsFixture.UserNameForStandardHost;
        private static readonly string _password = IntegrationTests.ConnectionSettingsFixture.PasswordForStandardHost;
        public static Uri Uri = new($"net.amqp://{_host}:{_port}/amq.direct/{_queueName}#{_queueKey}");
        public static RabbitMqConnectionSettings ConnectionSettings => RabbitMqConnectionSettings.FromUri(Uri);

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TestService>();
            services.AddServiceModelServices();
            services.AddQueueTransport();
            services.AddServiceModelRabbitMqSupport();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseServiceModel(services =>
            {
                services.AddService<TestService>();
                services.AddServiceEndpoint<TestService, ITestContract>(
                    new RabbitMqBinding
                    {
                        Credentials = new NetworkCredential(_userName, _password),
                        QueueConfiguration = new QuorumQueueConfiguration()
                    },
                    Uri);
            });
        }
    }
}
