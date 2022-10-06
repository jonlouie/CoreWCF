// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace CoreWCF.Channels.Configuration
{
    public class ClassicQueueConfiguration : QueueDeclareConfiguration
    {
        public override string QueueType
        {
            get
            {
                return RabbitMqQueueType.Classic;
            }
            set
            {
                if (!string.Equals(value, RabbitMqQueueType.Classic))
                {
                    throw new ArgumentException($"The property {nameof(QueueType)} cannot be set because {nameof(ClassicQueueConfiguration)} " +
                                                $"only creates queues of type {RabbitMqQueueType.Classic}.");
                }
                
            }
        }

        public ClassicQueueConfiguration AsTemporaryQueue()
        {
            AutoDelete = true;
            return this;
        }
    }
}
