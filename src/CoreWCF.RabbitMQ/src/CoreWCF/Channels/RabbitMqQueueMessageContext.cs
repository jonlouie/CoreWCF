// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CoreWCF.Queue.Common;

namespace CoreWCF.Channels
{
    public class RabbitMqQueueMessageContext : QueueMessageContext
    {
        /// <summary>
        /// Delivery confirmation number for a message read from a RabbitMQ queue
        /// </summary>
        public ulong DeliveryTag { get; set; }
    }
}
