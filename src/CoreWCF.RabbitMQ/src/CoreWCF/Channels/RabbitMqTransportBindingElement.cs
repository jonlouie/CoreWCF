// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net;
using CoreWCF.Channels.Configuration;
using CoreWCF.Configuration;
using CoreWCF.Queue.Common;
using CoreWCF.Queue.Common.Configuration;
using RabbitMQ.Client;

namespace CoreWCF.Channels
{
    public class RabbitMqTransportBindingElement : QueueBaseTransportBindingElement
    {
        public RabbitMqTransportBindingElement()
        {
        }
        
        private RabbitMqTransportBindingElement(RabbitMqTransportBindingElement other)
        {
            MaxReceivedMessageSize = other.MaxReceivedMessageSize;
            BrokerProtocol = other.BrokerProtocol;
            SslOption = other.SslOption;
            VirtualHost = other.VirtualHost;
            Credentials = other.Credentials;
            QueueConfiguration = other.QueueConfiguration;
        }

        public override BindingElement Clone()
        {
            return new RabbitMqTransportBindingElement(this);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return null;
            }

            return base.GetProperty<T>(context);
        }

        public override QueueTransportPump BuildQueueTransportPump(BindingContext context)
        {
            var serviceProvider = context.BindingParameters.Find<IServiceProvider>();
            var serviceDispatcher = context.BindingParameters.Find<IServiceDispatcher>();
            return new RabbitMqTransportPump(serviceProvider, serviceDispatcher, SslOption, QueueConfiguration, Credentials, VirtualHost);
        }

        /// <summary>
        /// Gets the scheme used by the binding, soap.amqp
        /// </summary>
        public override string Scheme
        {
            get { return CurrentVersion.Scheme; }
        }

        /// <summary>
        /// The largest receivable encoded message
        /// </summary>
        public override long MaxReceivedMessageSize { get; set; }

        /// <summary>
        /// Specifies the version of the AMQP protocol that should be used to
        /// communicate with the broker
        /// </summary>
        public IProtocol BrokerProtocol { get; set; }

        /// <summary>
        /// SSL configuration for the RabbitMQ queue
        /// </summary>
        public SslOption SslOption { get; set; }

        /// <summary>
        /// Virtual host for the RabbitMQ queue
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// Credentials used for accessing the RabbitMQ host
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// Configuration used for declaring a queue
        /// </summary>
        public QueueDeclareConfiguration QueueConfiguration { get; set; }
    }
}
