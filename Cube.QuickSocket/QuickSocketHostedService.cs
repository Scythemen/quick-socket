using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Net;
using Bedrock.Framework;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


namespace Cube.QuickSocket
{
    public class QuickSocketHostedService : IHostedService
    {
        private static readonly ConcurrentDictionary<string, ConnectionContext> connections = new ConcurrentDictionary<string, ConnectionContext>();

        private readonly ILogger logger;
        private readonly QuickSocketOptions options;
        private readonly IServiceProvider serviceProvider;
        private readonly object lockObject = new object();

        public QuickSocketHostedService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            this.logger = serviceProvider.GetService<ILogger<QuickSocketHostedService>>();
            this.logger ??= NullLogger<QuickSocketHostedService>.Instance;

            var _opts = serviceProvider.GetService<IOptions<QuickSocketOptions>>();
            options = _opts == null ? new QuickSocketOptions() : _opts.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            if (!options.EnableClient && !options.EnableServer)
            {
                logger.LogWarning("Service is not start, {0}={1}, {2}={3}",
                    nameof(QuickSocketOptions.EnableClient), options.EnableClient, nameof(QuickSocketOptions.EnableServer), options.EnableServer);
                return;
            }

            if (options.EnableServer)
            {
                await StartListener(cancellationToken).ConfigureAwait(false);
            }

            if (options.EnableClient)
            {
                await ConnectToRemote(cancellationToken).ConfigureAwait(false);
            }

        }

        private async Task ConnectToRemote(CancellationToken cancellationToken)
        {
            var endpointProvider = serviceProvider.GetService<IRemoteEndPointProvider>();
            if (endpointProvider == null || endpointProvider.EndPoints?.Length < 1)
            {
                return;
            }

            var client = new ClientBuilder(serviceProvider)
                                 .UseSockets()
                                 .UseConnectionLogging()
                                 //.UseConnectionHandler<>
                                 .Build();


            foreach (var item in endpointProvider.EndPoints)
            {
                ConnectToRemoteOne(client, item, cancellationToken);
            }

        }

        private async Task ConnectToRemoteOne(Client client, EndPoint endpoint, CancellationToken cancellationToken)
        {
            try
            {
                var cnn = await client.ConnectAsync(endpoint, cancellationToken);
                connections.TryAdd(endpoint.ToString(), cnn);
            }
            catch (Exception ex)
            {
            }

        }

        private async Task StartListener(CancellationToken cancellationToken)
        {
            var ipports = options.ServerIpPorts.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (ipports.Length == 0)
            {
                return;
            }

            var serverBuilder = new ServerBuilder(serviceProvider);
            foreach (var item in ipports)
            {
                var r = item.Trim().Split(':');
                var ip = IPAddress.Parse(r[0]);
                var port = int.Parse(r[1]);
                serverBuilder.UseSockets((sockets) =>
                {
                    sockets.Listen(ip, port,
                                 (builder) =>
                                 {
                                     if (options.ServerConnectionLimit > 0)
                                     {
                                         builder.UseConnectionLimits(options.ServerConnectionLimit);
                                     }
                                     if (logger.IsEnabled(LogLevel.Debug) || logger.IsEnabled(LogLevel.Trace))
                                     {
                                         builder.UseConnectionLogging();
                                     }

                                     //var f = serviceProvider.GetService<FirstMiddleware>();
                                     //builder.Use(next => new FirstMiddleware(next, logger, connectionLimit).OnConnectionAsync);

                                     builder.UseConnectionHandler<ProtocolDispatcherConnectionHandler>();
                                 });
                    logger.LogInformation("Listenning: {0}", item.Trim());
                });
            }

            await serverBuilder.Build().StartAsync(cancellationToken).ConfigureAwait(false);

        }


        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var cnn in connections)
            {
                cnn.Value?.Abort();
            }
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
        }



    }
}