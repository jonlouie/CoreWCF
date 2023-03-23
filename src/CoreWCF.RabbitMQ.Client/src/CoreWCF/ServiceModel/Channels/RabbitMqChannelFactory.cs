using System;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using CoreWCF.Runtime;
using CoreWCF.Security;

namespace CoreWCF.ServiceModel.Channels
{
    internal class RabbitMqChannelFactory : ChannelFactoryBase<IOutputChannel>
    {
        private BufferManager _bufferManager;
        private MessageEncoderFactory _messageEncoderFactory;
        private RabbitMqTransportBindingElement _transport;
        private SecurityCredentialsManager _channelCredentials;

        internal RabbitMqChannelFactory(RabbitMqTransportBindingElement transport, BindingContext context)
            : base(context.Binding)
        {
            _transport = transport;
            _bufferManager = BufferManager.CreateBufferManager(transport.MaxBufferPoolSize, int.MaxValue);

            var messageEncoderBindingElements = context.BindingParameters.OfType<MessageEncodingBindingElement>().ToList();
            if (messageEncoderBindingElements.Count > 1)
            {
                throw new InvalidOperationException("More than one MessageEncodingBindingElement was found in the BindingParameters of the BindingContext");
            }

            if (messageEncoderBindingElements.Count == 1)
            {
                _messageEncoderFactory = messageEncoderBindingElements[0].CreateMessageEncoderFactory();
            }
            else
            {
                _messageEncoderFactory = RabbitMqDefaults.DefaultMessageEncoderFactory;
            }

            _channelCredentials = context.BindingParameters.OfType<SecurityCredentialsManager>().FirstOrDefault();
        }

        public RabbitMqTransportBindingElement Transport => _transport;

        public BufferManager BufferManager => _bufferManager;

        public MessageEncoderFactory MessageEncoderFactory => _messageEncoderFactory;

        public override T GetProperty<T>()
        {
            T messageEncoderProperty = MessageEncoderFactory.Encoder.GetProperty<T>();
            if (messageEncoderProperty != null)
            {
                return messageEncoderProperty;
            }

            if (typeof(T) == typeof(MessageVersion))
            {
                return (T)(object)MessageEncoderFactory.Encoder.MessageVersion;
            }

            return base.GetProperty<T>();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return Task.CompletedTask;
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected override IOutputChannel OnCreateChannel(System.ServiceModel.EndpointAddress queueUrl, Uri via)
        {
            var connectionSettings = _transport.GetConnectionSettings();
            var factory = connectionSettings.GetConnectionFactory();
            var connection = factory.CreateConnection();
            var rabbitMqClient = connection.CreateModel();

            return new RabbitMqOutputChannel(
                this,
                rabbitMqClient,
                connectionSettings,
                MessageEncoderFactory.Encoder);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            _bufferManager.Clear();
        }
    }
}
