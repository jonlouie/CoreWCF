// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace CoreWCF.Channels.Configuration
{
    public class QuorumQueueConfiguration : QueueDeclareConfiguration
    {
        public override string QueueType
        {
            get
            {
                return RabbitMqQueueType.Quorum;
            }
            set
            {
                if(!string.Equals(value, RabbitMqQueueType.Quorum))
                {
                    throw new ArgumentException($"The property {nameof(QueueType)} cannot be set because {nameof(QuorumQueueConfiguration)} " +
                                                $"only creates queues of type {RabbitMqQueueType.Quorum}.");
                }
            }
        }

        public override bool Durable
        {
            get
            {
                return true;
            }
            set
            {
                if (!value)
                {
                    throw new ArgumentException($"The property {nameof(Durable)} cannot be set to false because " +
                                                "non-durable quorum queues are not supported by RabbitMQ.");
                }
            }
        }

        public override bool Exclusive
        {
            get
            {
                return false;
            }
            set
            {
                if (value)
                {
                    throw new ArgumentException($"The property {nameof(Exclusive)} cannot be set to true because " +
                                                "exclusive quorum queues are not supported by RabbitMQ.");
                }
            }
        }
        
        public override bool GlobalQosPrefetch
        {
            get
            {
                return false;
            }
            set
            {
                if (value)
                {
                    throw new ArgumentException($"The property {nameof(GlobalQosPrefetch)} cannot be set to true because " +
                                                "quorum queues do not supported global Qos prefetch.");
                }
            }
        }
    }
}
