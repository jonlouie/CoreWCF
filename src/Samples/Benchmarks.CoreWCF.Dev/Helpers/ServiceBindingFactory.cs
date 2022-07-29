// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using CoreWCF;
using CoreWCF.Channels;

namespace Benchmarks.CoreWCF.Helpers
{
    public class ServiceBindingFactory
    {
        public static Binding GetStandardBasicHttpBinding()
        {
            var basicBinding = new BasicHttpBinding
            {
                SendTimeout = TimeSpan.FromMinutes(20.0),
                ReceiveTimeout = TimeSpan.FromMinutes(20.0),
                OpenTimeout = TimeSpan.FromMinutes(20.0),
                CloseTimeout = TimeSpan.FromMinutes(20.0),
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue,
            };
            var binding = new CustomBinding(basicBinding);
            var tbe = binding.Elements.Find<HttpTransportBindingElement>();
            tbe.MaxBufferPoolSize = int.MaxValue;

            return binding;
        }
    }
}
