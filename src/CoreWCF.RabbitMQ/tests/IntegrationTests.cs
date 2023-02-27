// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net;
using System.ServiceModel;
using Contracts;
using CoreWCF.Channels;
using CoreWCF.Channels.Configuration;
using CoreWCF.Configuration;
using CoreWCF.Queue.Common.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Xunit;
using Xunit.Abstractions;

namespace CoreWCF.RabbitMQ.Tests
{
    public class IntegrationTests
    {
        private readonly ITestOutputHelper _output;

        public IntegrationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(Skip = "Requires RabbitMQ host with SSL")]
        public void ClassicQueueWithTls_SendReceiveMessage_Success()
        {
            IWebHost host = ServiceHelper.CreateWebHostBuilder<ClassicQueueWithTLSStartup>(_output).Build();
            using (host)
            {
                host.Start();

                // Send a message with the client
                var endpointAddress = new System.ServiceModel.EndpointAddress(ClassicQueueWithTLSStartup.Uri);
                var rabbitMqBinding = new ServiceModel.Channels.RabbitMqBinding(ClassicQueueWithTLSStartup.Uri)
                {
                    SslOption = ClassicQueueWithTLSStartup.SslOption,
                    Credentials = ClassicQueueWithTLSStartup.Credentials
                };
                var factory = new ChannelFactory<ITestContract>(rabbitMqBinding, endpointAddress);
                var channel = factory.CreateChannel();
                ((System.ServiceModel.Channels.IChannel)channel).Open();
                channel.Create($"IntegrationTestMessage: {nameof(ClassicQueueWithTls_SendReceiveMessage_Success)}");

                // Ensure the message is processed
                var resolver = new DependencyResolverHelper(host);
                var testService = resolver.GetService<TestService>();
                Assert.True(testService.ManualResetEvent.Wait(System.TimeSpan.FromSeconds(5)));
            }
        }

        // Automated tests use a Linux container to host RabbitMQ so this test is Linux-only
        [Fact]
        [Trait("Category", "LinuxOnly")]
        public void DefaultClassicQueueConfiguration_SendReceiveMessage_Success()
        {
            IWebHost host = ServiceHelper.CreateWebHostBuilder<DefaultClassicQueueStartup>(_output).Build();
            using (host)
            {
                host.Start();

                // Send a message with the client
                var endpointAddress = new System.ServiceModel.EndpointAddress(DefaultClassicQueueStartup.Uri);
                var rabbitMqBinding = new ServiceModel.Channels.RabbitMqBinding(DefaultClassicQueueStartup.Uri)
                {
                    Credentials = DefaultClassicQueueStartup.Credentials
                };
                var factory = new ChannelFactory<ITestContract>(rabbitMqBinding, endpointAddress);
                var channel = factory.CreateChannel();
                ((System.ServiceModel.Channels.IChannel)channel).Open();
                channel.Create($"IntegrationTestMessage: {nameof(DefaultClassicQueueConfiguration_SendReceiveMessage_Success)}");

                // Ensure the message is processed
                var resolver = new DependencyResolverHelper(host);
                var testService = resolver.GetService<TestService>();
                Assert.True(testService.ManualResetEvent.Wait(System.TimeSpan.FromSeconds(5)));
            }
        }

        // Automated tests use a Linux container to host RabbitMQ so this test is Linux-only
        [Fact]
        [Trait("Category", "LinuxOnly")]
        public void DefaultQuorumQueueConfiguration_SendReceiveMessage_Success()
        {
            IWebHost host = ServiceHelper.CreateWebHostBuilder<DefaultQuorumQueueStartup>(_output).Build();
            using (host)
            {
                host.Start();

                // Send a message with the client
                var endpointAddress = new System.ServiceModel.EndpointAddress(DefaultQuorumQueueStartup.Uri);
                var rabbitMqBinding = new ServiceModel.Channels.RabbitMqBinding(DefaultQuorumQueueStartup.Uri)
                {
                    Credentials = DefaultQuorumQueueStartup.Credentials
                };
                var factory = new ChannelFactory<ITestContract>(rabbitMqBinding, endpointAddress);
                var channel = factory.CreateChannel();
                ((System.ServiceModel.Channels.IChannel)channel).Open();
                channel.Create($"IntegrationTestMessage: {nameof(DefaultQuorumQueueConfiguration_SendReceiveMessage_Success)}");

                // Ensure the message is processed
                var resolver = new DependencyResolverHelper(host);
                var testService = resolver.GetService<TestService>();
                Assert.True(testService.ManualResetEvent.Wait(System.TimeSpan.FromSeconds(5)));
            }
        }

        // Automated tests use a Linux container to host RabbitMQ so this test is Linux-only
        [Fact]
        [Trait("Category", "LinuxOnly")]
        public void DefaultQueueConfiguration_ReceiveMessage_Success()
        {
            IWebHost host = ServiceHelper.CreateWebHostBuilder<DefaultQueueStartup>(_output).Build();
            using (host)
            {
                host.Start();

                // Send a message with the client
                var endpointAddress = new System.ServiceModel.EndpointAddress(DefaultQueueStartup.Uri);
                var rabbitMqBinding = new ServiceModel.Channels.RabbitMqBinding(DefaultQueueStartup.Uri)
                {
                    Credentials = DefaultQueueStartup.Credentials
                };
                var factory = new ChannelFactory<ITestContract>(rabbitMqBinding, endpointAddress);
                var channel = factory.CreateChannel();
                ((System.ServiceModel.Channels.IChannel)channel).Open();
                channel.Create($"IntegrationTestMessage: {nameof(DefaultQueueConfiguration_ReceiveMessage_Success)}");

                // Ensure the message is processed
                var resolver = new DependencyResolverHelper(host);
                var testService = resolver.GetService<TestService>();
                Assert.True(testService.ManualResetEvent.Wait(System.TimeSpan.FromSeconds(5)));
            }
        }
    }

    public class ClassicQueueWithTLSStartup
    {
        //public static Uri Uri = new("net.amqps://HOST:PORT/amq.direct/QUEUE_NAME#ROUTING_KEY");
        public static Uri Uri = new("net.amqps://b-cd784bef-9902-4b70-ba88-a6379182e96b.mq.us-west-2.amazonaws.com:5671/amq.direct/corewcf-test-default-queue#corewcf-test-default-key");
        //private static readonly ICredentials s_credentials = new NetworkCredential(ConnectionFactory.DefaultUser, ConnectionFactory.DefaultPass);
        public static readonly ICredentials Credentials = new NetworkCredential("admin", "Password1234");
        public static readonly SslOption SslOption = new SslOption
        {
            ServerName = Uri.Host,
            Enabled = true
        };

        public static RabbitMqConnectionSettings ConnectionSettings => RabbitMqConnectionSettings.FromUri(Uri, Credentials, SslOption);

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TestService>();
            services.AddServiceModelServices();
            services.AddQueueTransport();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseServiceModel(services =>
            {
                services.AddService<TestService>();
                services.AddServiceEndpoint<TestService, ITestContract>(
                    new RabbitMqBinding
                    {
                        SslOption = SslOption,
                        Credentials = Credentials,
                        QueueConfiguration = new ClassicQueueConfiguration().AsTemporaryQueue()
                    },
                    Uri);
            });
        }
    }
    
    public class DefaultClassicQueueStartup
    {
        public static Uri Uri = new("net.amqp://localhost:5672/amq.direct/corewcf-test-default-classic-queue#corewcf-test-default-classic-key");
        public static readonly ICredentials Credentials = new NetworkCredential(ConnectionFactory.DefaultUser, ConnectionFactory.DefaultPass);

        public static RabbitMqConnectionSettings ConnectionSettings => RabbitMqConnectionSettings.FromUri(Uri, Credentials);

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TestService>();
            services.AddServiceModelServices();
            services.AddQueueTransport();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseServiceModel(services =>
            {
                services.AddService<TestService>();
                services.AddServiceEndpoint<TestService, ITestContract>(
                    new RabbitMqBinding
                    {
                        Credentials = Credentials,
                        QueueConfiguration = new QuorumQueueConfiguration()
                    },
                    Uri);
            });
        }
    }
    
    public class DefaultQuorumQueueStartup
    {
        public static Uri Uri = new("net.amqp://localhost:5672/amq.direct/corewcf-test-default-quorum-queue#corewcf-test-default-quorum-key");
        public static readonly ICredentials Credentials = new NetworkCredential(ConnectionFactory.DefaultUser, ConnectionFactory.DefaultPass);

        public static RabbitMqConnectionSettings ConnectionSettings =>
            RabbitMqConnectionSettings.FromUri(Uri, Credentials);

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TestService>();
            services.AddServiceModelServices();
            services.AddQueueTransport();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseServiceModel(services =>
            {
                services.AddService<TestService>();
                services.AddServiceEndpoint<TestService, ITestContract>(
                    new RabbitMqBinding
                    {
                        Credentials = Credentials,
                        QueueConfiguration = new QuorumQueueConfiguration()
                    },
                    Uri);
            });
        }
    }

    public class DefaultQueueStartup
    {
        public static Uri Uri = new("net.amqp://localhost:5672/amq.direct/corewcf-test-default-queue#corewcf-test-default-key");
        public static readonly ICredentials Credentials = new NetworkCredential(ConnectionFactory.DefaultUser, ConnectionFactory.DefaultPass);

        public static RabbitMqConnectionSettings ConnectionSettings => RabbitMqConnectionSettings.FromUri(Uri, Credentials);

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TestService>();
            services.AddServiceModelServices();
            services.AddQueueTransport();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseServiceModel(services =>
            {
                services.AddService<TestService>();
                services.AddServiceEndpoint<TestService, ITestContract>(
                    new RabbitMqBinding
                    {
                        Credentials = Credentials,
                        QueueConfiguration = new QuorumQueueConfiguration()
                    },
                    Uri);
            });
        }
    }
}
