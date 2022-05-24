// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel.Description;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Benchmarks.Http.Helpers;
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks.Http.Benchmarks
{
    [SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 10, targetCount: 100)]
    public class WsHttpImpersonationBenchmark
    {
        private readonly string _testString = new string('a', 3000);
        private IWebHost _host;
        private Binding _binding;
        private ClientContract.IEchoService _channel;

        [GlobalSetup(Targets = new[] { nameof(WsHttpImpersonation) })]
        public void GlobalSetupImpersonation()
        {
            _host = ServiceHelper.CreateHttpsWebHostBuilder<WSHttpTransportWithImpersonation>().Build();
            _host.Start();

            System.ServiceModel.WSHttpBinding wsHttpBinding = ClientHelper.GetBufferedModeWSHttpBinding(System.ServiceModel.SecurityMode.Transport);
            var factory = new System.ServiceModel.ChannelFactory<ClientContract.IEchoService>(wsHttpBinding,
                new System.ServiceModel.EndpointAddress(new Uri("https://localhost:8443/WSHttpWcfService/basichttp.svc")));
            factory.Credentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication = new System.ServiceModel.Security.X509ServiceCertificateAuthentication
            {
                CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None
            };
            _channel = factory.CreateChannel();
            ((System.ServiceModel.Channels.IChannel)_channel).Open();
        }

        [Benchmark]
        public void WsHttpImpersonation()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.EchoForImpersonation(_testString);
        }

        [GlobalSetup(Targets = new[] { nameof(WsHttpMessageCertificate) })]
        public void GlobalSetupMessageCertificate()
        {
            _host = ServiceHelper.CreateHttpsWebHostBuilder<WSHttpTransportWithMessageCredentialWithCertificate>().Build();
            _host.Start();

            System.ServiceModel.WSHttpBinding wsHttpBinding = ClientHelper.GetBufferedModeWSHttpBinding(System.ServiceModel.SecurityMode.TransportWithMessageCredential);
            wsHttpBinding.Security.Message.ClientCredentialType = System.ServiceModel.MessageCredentialType.Certificate;
            var factory = new System.ServiceModel.ChannelFactory<ClientContract.IEchoService>(wsHttpBinding,
                new System.ServiceModel.EndpointAddress(new Uri("https://localhost:8443/WSHttpWcfService/basichttp.svc")));
            ClientCredentials clientCredentials = (ClientCredentials)factory.Endpoint.EndpointBehaviors[typeof(ClientCredentials)];
            clientCredentials.ClientCertificate.Certificate = ServiceHelper.GetServiceCertificate();
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication = new System.ServiceModel.Security.X509ServiceCertificateAuthentication
            {
                CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None
            };
            _channel = factory.CreateChannel();
            ((System.ServiceModel.Channels.IChannel)_channel).Open();
        }

        [Benchmark]
        public void WsHttpMessageCertificate()
        {
            // Always save the returned value or the call will be optimized away, preventing benchmark execution
            var result = _channel.EchoString(_testString);
        }
        
        [GlobalCleanup(Targets = new[] { nameof(WsHttpImpersonation) })]
        public void GlobalCleanupImpersonation()
        {
            ((System.ServiceModel.Channels.IChannel)_channel).Close();
        }

        #region Startups
        internal class WSHttpTransportWithImpersonation : StartupWSHttpBase
        {
            public WSHttpTransportWithImpersonation() :
                base(SecurityMode.Transport, MessageCredentialType.None)
            {
            }

            public override CoreWCF.Channels.Binding ChangeBinding(WSHttpBinding wsBinding)
            {
                wsBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                return wsBinding;
            }
        }

        internal class WSHttpTransportWithMessageCredentialWithCertificate : StartupWSHttpBase
        {
            public WSHttpTransportWithMessageCredentialWithCertificate() :
                base(SecurityMode.TransportWithMessageCredential, MessageCredentialType.Certificate)
            {
            }

            public override void ChangeHostBehavior(ServiceHostBase host)
            {
                var srvCredentials = new CoreWCF.Description.ServiceCredentials();
                srvCredentials.ClientCertificate.Authentication.CertificateValidationMode
                    = CoreWCF.Security.X509CertificateValidationMode.Custom;
                srvCredentials.ClientCertificate.Authentication.CustomCertificateValidator
                    = new MyX509CertificateValidator();
                srvCredentials.ServiceCertificate.Certificate = ServiceHelper.GetServiceCertificate();
                host.Description.Behaviors.Add(srvCredentials);
            }

            public class MyX509CertificateValidator : CoreWCF.IdentityModel.Selectors.X509CertificateValidator
            {
                public MyX509CertificateValidator()
                {
                }

                public override void Validate(X509Certificate2 certificate)
                {
                    // just Check that there is a certificate.
                    if (certificate == null)
                    {
                        throw new ArgumentNullException("certificate");
                    }
                }
            }
        }

        internal abstract class StartupWSHttpBase
        {
            private readonly CoreWCF.SecurityMode _wsHttpSecurityMode;
            private readonly MessageCredentialType _credentialType;
            public StartupWSHttpBase(CoreWCF.SecurityMode securityMode, MessageCredentialType credentialType)
            {
                _wsHttpSecurityMode = securityMode;
                _credentialType = credentialType;
            }
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddServiceModelServices();
                services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                    .AddNegotiate();
            }

            public virtual void ChangeHostBehavior(ServiceHostBase host)
            {

            }

            public virtual CoreWCF.Channels.Binding ChangeBinding(WSHttpBinding wsBInding)
            {
                return wsBInding;
            }

            public void Configure(IApplicationBuilder app)
            {
                CoreWCF.WSHttpBinding serverBinding = new CoreWCF.WSHttpBinding(_wsHttpSecurityMode);
                serverBinding.Security.Message.ClientCredentialType = _credentialType;
                app.UseServiceModel(builder =>
                {
                    builder.AddService<Services.EchoService>();
                    builder.AddServiceEndpoint<Services.EchoService, ServiceContract.IEchoService>(ChangeBinding(serverBinding), "/WSHttpWcfService/basichttp.svc");
                    Action<ServiceHostBase> serviceHost = host => ChangeHostBehavior(host);
                    builder.ConfigureServiceHostBase<Services.EchoService>(serviceHost);
                });
            }
        }
        #endregion
    }
}
