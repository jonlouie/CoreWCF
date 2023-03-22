using System;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace CoreWCF.ServiceModel.Channels
{
    public class RabbitMqOutputChannel : ChannelBase, System.ServiceModel.Channels.IOutputChannel
    {
        private RabbitMqChannelFactory _parent;
        private RabbitMqConnectionSettings _connectionSettings;
        private System.ServiceModel.EndpointAddress _baseAddress;
        private Uri _via;
        private System.ServiceModel.Channels.MessageEncoder _encoder;
        private IModel _rabbitMqClient;

        internal RabbitMqOutputChannel(
            RabbitMqChannelFactory factory,
            IModel rabbitMqClient,
            RabbitMqConnectionSettings connectionSettings,
            System.ServiceModel.Channels.MessageEncoder encoder)
            : base(factory)
        {
            _parent = factory;
            _rabbitMqClient = rabbitMqClient;
            _connectionSettings = connectionSettings;
            _encoder = encoder;
            _via = _connectionSettings.BaseAddress;
            _baseAddress = new System.ServiceModel.EndpointAddress(_via);
        }

        System.ServiceModel.EndpointAddress System.ServiceModel.Channels.IOutputChannel.RemoteAddress => _baseAddress;

        Uri System.ServiceModel.Channels.IOutputChannel.Via => _via;

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(System.ServiceModel.Channels.IOutputChannel))
            {
                return (T)(object)this;
            }

            T messageEncoderProperty = _encoder.GetProperty<T>();
            if (messageEncoderProperty != null)
            {
                return messageEncoderProperty;
            }

            return base.GetProperty<T>();
        }

        /// <summary>
        /// Open the channel for use. We do not have any blocking work to perform so this is a no-op
        /// </summary>
        protected override void OnOpen(TimeSpan timeout)
        { }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return Task.CompletedTask;
        }

        protected override void OnEndOpen(IAsyncResult result)
        { }

        protected override void OnAbort()
        { }

        protected override void OnClose(TimeSpan timeout)
        {
            _rabbitMqClient.Close();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        { return Task.CompletedTask; }

        protected override void OnEndClose(IAsyncResult result)
        { }

        /// <summary>
        /// Address the Message and serialize it into a byte array.
        /// </summary>
        private ArraySegment<byte> EncodeMessage(System.ServiceModel.Channels.Message message)
        {
            try
            {
                _baseAddress.ApplyTo(message);
                return _encoder.WriteMessage(
                    message,
                    (int)_parent.Transport.MaxReceivedMessageSize,
                    _parent.BufferManager);
            }
            finally
            {
                // We have consumed the message by serializing it, so clean up
                message.Close();
            }
        }

        /// <summary>
        /// Published a Message to RabbitMQ and waits for a Publisher confirm. Note that
        /// waiting for publisher confirms significantly slows down publishing
        /// </summary>
        public void Send(System.ServiceModel.Channels.Message message)
        {
            var messageBuffer = EncodeMessage(message);

            try
            {
                _rabbitMqClient.BasicPublish(
                    exchange: _connectionSettings.Exchange,
                    routingKey: _connectionSettings.RoutingKey,
                    body: messageBuffer);
            }
            finally
            {
                // Make sure buffers are always returned to the BufferManager
                _parent.BufferManager.ReturnBuffer(messageBuffer.Array);
            }
        }

        /// <summary>
        /// Published a Message to RabbitMQ and waits for a Publisher confirm. Note that
        /// waiting for publisher confirms significantly slows down publishing
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        public void Send(System.ServiceModel.Channels.Message message, TimeSpan timeout)
        {
            var messageBuffer = EncodeMessage(message);

            try
            {
                _rabbitMqClient.ConfirmSelect();
                _rabbitMqClient.BasicPublish(
                    exchange: _connectionSettings.Exchange,
                    routingKey: _connectionSettings.RoutingKey,
                    body: messageBuffer);
                _rabbitMqClient.WaitForConfirmsOrDie(timeout);
            }
            catch (OperationInterruptedException e)
            {
                throw new TimeoutException("Send message timeout exceeded.", e);
            }
            finally
            {
                // Make sure buffers are always returned to the BufferManager
                _parent.BufferManager.ReturnBuffer(messageBuffer.Array);
            }
        }

        public IAsyncResult BeginSend(System.ServiceModel.Channels.Message message, AsyncCallback callback, object state)
        { return Task.CompletedTask; }

        public IAsyncResult BeginSend(System.ServiceModel.Channels.Message message, TimeSpan timeout, AsyncCallback callback, object state)
        { return BeginSend(message, callback, state); }

        public void EndSend(IAsyncResult result)
        { }
    }
}
