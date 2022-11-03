﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading.Tasks;
using CoreWCF.Configuration;
using CoreWCF.Queue;
using CoreWCF.Queue.Common;
using CoreWCF.RabbitMQ.CoreWCF.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoreWCF.RabbitMQ.Tests.Fakes
{
    public class Interceptor
    {
        public string Name { get; private set; }

        public void SetName(string name)
        {
            Name = name;
        }
    }
}
