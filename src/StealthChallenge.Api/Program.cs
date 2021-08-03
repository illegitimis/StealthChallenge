using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using StealthChallenge.Abstractions.Infrastructure.Configuration;
using StealthChallenge.Logging;
using System;

namespace StealthChallenge.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel((WebHostBuilderContext ctx, KestrelServerOptions kso) =>
                    {
                        var kestrelConfiguration = ctx.Configuration.GetSection("Kestrel").Get<KestrelConfiguration>();
                        kso.ListenAnyIP(kestrelConfiguration.HttpPort);
                        kso.ListenAnyIP(
                            kestrelConfiguration.TcpPort,
                            tcpListenOptions => tcpListenOptions
                                .UseConnectionLogging()
                                .UseConnectionHandler<TcpConnectionHandler>());

                    });
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((HostBuilderContext ctx, IServiceProvider sp, LoggerConfiguration lc) => lc
                    .BasicConfiguration()
                );
    }
}
