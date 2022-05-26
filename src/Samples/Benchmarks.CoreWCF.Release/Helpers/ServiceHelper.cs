// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Diagnostics;
using System.Net;

namespace Benchmarks.CoreWCF.Release.Helpers
{
    public static class ServiceHelper
    {
        public static IWebHostBuilder CreateWebHostBuilder<TStartup>() where TStartup : class =>
            WebHost.CreateDefaultBuilder(Array.Empty<string>())
                .UseKestrel(options =>
                {
                    options.Limits.MaxRequestBufferSize = null;
                    options.Limits.MaxRequestBodySize = null;
                    options.Limits.MaxResponseBufferSize = null;
                    options.AllowSynchronousIO = true;
                    options.Listen(IPAddress.Loopback, 8080, listenOptions =>
                    {
                        if (Debugger.IsAttached)
                        {
                            listenOptions.UseConnectionLogging();
                        }
                    });
                })
                .UseStartup<TStartup>();
    }
}
