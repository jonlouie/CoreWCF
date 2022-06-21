using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using CoreWCF.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Benchmarks.CoreWCF.Helpers;

namespace Benchmarks.CoreWCF.Dev
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IWebHost host = WebHost.CreateDefaultBuilder(Array.Empty<string>())
                .UseKestrel(options =>
                {
                    options.Limits.MaxRequestBufferSize = null;
                    options.Limits.MaxRequestBodySize = null;
                    options.Limits.MaxResponseBufferSize = null;
                    //options.Listen(IPAddress.Loopback, 8080, listenOptions =>{});
                    options.ListenAnyIP(8080);
                })
                .UseStartup<Startup>()
                .Build();

            using (host)
            {
                host.Start();
                Console.WriteLine("Server started! Press Enter to close.");
                Console.ReadLine();
            }
        }

        internal class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddServiceModelServices();
            }

            public void Configure(IApplicationBuilder app)
            {
                var binding = ServiceBindingFactory.GetStandardBasicHttpBinding();
                app.UseServiceModel(builder =>
                {
                    builder.AddService<Services.EchoService>();
                    builder.AddServiceEndpoint<Services.EchoService, ServiceContract.IEchoService>(binding, "/BasicWcfService/basichttp.svc");
                });
            }
        }
    }
}
