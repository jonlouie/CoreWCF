// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using CoreWCF.Channels.Configuration;
using RabbitMQ.Client;

namespace CoreWCF.Channels
{
    public class RabbitMqBinding : Binding
    {
        public const long DefaultMaxMessageSize = 8192L;
        private RabbitMqTransportBindingElement _transport;
        private TextMessageEncodingBindingElement _textMessageEncodingBindingElement;
        private BinaryMessageEncodingBindingElement _binaryMessageEncodingBindingElement;

        /// <summary>
        /// Specifies the maximum encoded message size
        /// </summary>
        public long MaxMessageSize { get; set; }

        /// <summary>
        /// Specifies the version of the AMQP protocol that should be used to communicate with the broker
        /// </summary>
        public IProtocol BrokerProtocol { get; set; }
        public SslOption SslOption { get; set; }
        public string VirtualHost { get; set; }

        /// <summary>
        /// Credentials to access the RabbitMQ host
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// Configuration used for declaring a queue
        /// </summary>
        public QueueDeclareConfiguration QueueConfiguration { get; set; }

        /// <summary>
        /// Gets the scheme used by the binding
        /// </summary>
        public override string Scheme
        {
            get { return CurrentVersion.Scheme; }
        }
        
        public RabbitMqMessageEncoding MessageEncoding { get; set; } = RabbitMqMessageEncoding.Text;

        public RabbitMqBinding()
        {
            Name = "RabbitMQBinding";
            Namespace = "http://schemas.rabbitmq.com/2007/RabbitMQ/";

            MaxMessageSize = DefaultMaxMessageSize;
            BrokerProtocol = Protocols.DefaultProtocol;
            SslOption = new SslOption();
            VirtualHost = ConnectionFactory.DefaultVHost;
            QueueConfiguration = new DefaultQueueConfiguration();

            _textMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
            _binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
            _transport = new RabbitMqTransportBindingElement();
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection elements = new BindingElementCollection();

            switch (MessageEncoding)
            {
                case RabbitMqMessageEncoding.Binary:
                    elements.Add(_binaryMessageEncodingBindingElement);
                    break;
                case RabbitMqMessageEncoding.Text:
                    elements.Add(_textMessageEncodingBindingElement);
                    break;
                default:
                    elements.Add(_textMessageEncodingBindingElement);
                    break;
            }

            elements.Add(GetTransport());

            return elements;
        }

        private RabbitMqTransportBindingElement GetTransport()
        {
            _transport.SslOption = SslOption;
            _transport.VirtualHost = VirtualHost;
            _transport.MaxReceivedMessageSize = MaxMessageSize;
            _transport.BrokerProtocol = BrokerProtocol;
            _transport.Credentials = Credentials;
            _transport.QueueConfiguration = QueueConfiguration;

            return _transport;
        }
    }
}
