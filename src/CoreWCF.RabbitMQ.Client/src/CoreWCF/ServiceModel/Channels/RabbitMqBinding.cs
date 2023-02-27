using System;
using System.Net;
using CoreWCF.Channels;
using RabbitMQ.Client;
using BinaryMessageEncodingBindingElement = System.ServiceModel.Channels.BinaryMessageEncodingBindingElement;
using Binding = System.ServiceModel.Channels.Binding;
using BindingElementCollection = System.ServiceModel.Channels.BindingElementCollection;
using TextMessageEncodingBindingElement = System.ServiceModel.Channels.TextMessageEncodingBindingElement;

namespace CoreWCF.ServiceModel.Channels
{
    public class RabbitMqBinding : Binding
    {
        private RabbitMqTransportBindingElement _transport;
        private TextMessageEncodingBindingElement _textMessageEncodingBindingElement;
        private BinaryMessageEncodingBindingElement _binaryMessageEncodingBindingElement;
        private Uri _baseAddress;

        public RabbitMqBinding(string uri)
            : this(new Uri(uri))
        {
        }

        public RabbitMqBinding(Uri uri)
        {
            Name = "RabbitMQBinding";
            Namespace = "http://schemas.rabbitmq.com/2007/RabbitMQ/";

            _baseAddress = uri;
            _textMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
            _binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();

            var sslOption = new SslOption();
            var virtualHost = ConnectionFactory.DefaultVHost;
            var credentials = new NetworkCredential(
                ConnectionFactory.DefaultUser,
                ConnectionFactory.DefaultPass);

            _transport = new RabbitMqTransportBindingElement
            {
                BaseAddress = _baseAddress,
                Credentials = credentials,
                SslOption = sslOption,
                VirtualHost = virtualHost
            };
        }

        /// <summary>
        /// Specifies the maximum encoded message size
        /// </summary>
        public long MaxMessageSize
        {
            get => _transport.MaxReceivedMessageSize;
            set => _transport.MaxReceivedMessageSize = value;
        }

        /// <summary>
        /// Specifies the version of the AMQP protocol that should be used to communicate with the broker
        /// </summary>
        public IProtocol BrokerProtocol
        {
            get => _transport.BrokerProtocol;
            set => _transport.BrokerProtocol = value;
        }

        public Uri BaseAddress
        {
            get => _transport.BaseAddress;
            set => _transport.BaseAddress = value;
        }

        public SslOption SslOption
        {
            get => _transport.SslOption;
            set => _transport.SslOption = value;
        }

        public string VirtualHost
        {
            get => _transport.VirtualHost;
            set => _transport.VirtualHost = value;
        }

        /// <summary>
        /// Credentials to access the RabbitMQ host
        /// </summary>
        public ICredentials Credentials
        {
            get => _transport.Credentials;
            set => _transport.Credentials = value;
        }

        /// <summary>
        /// Gets the scheme used by the binding
        /// </summary>
        public override string Scheme => _transport.Scheme;

        public RabbitMqMessageEncoding MessageEncoding { get; set; } = RabbitMqMessageEncoding.Text;

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

            elements.Add(_transport);

            return elements;
        }
    }
}
