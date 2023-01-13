using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cube.QuickSocket
{
    public static class WebHostBuilderExtension
    {
        public static void UseQuickSocket(this IWebHostBuilder builder)
        {
            builder.ConfigureServices((ctx, services) =>
            {
                services.AddSingleton<QuickSocketHostedService>();
                services.AddSingleton<ProtocolDispatcherConnectionHandler>();
                services.AddOptions<QuickSocketOptions>();
            });

            // TODO read config file

            // https://www.cnblogs.com/artech/p/inside-asp-net-core-6-31.html

            //ConnectionBuilderExtensions.UseConnectionHandler<MyConnectionHandler>();
            //builder.UseKestrel();
            //builder.UseKestrel((ctx, options) =>
            //{
            //    options.Listen(System.Net.IPAddress.Any, 9910, listenOption =>
            //    {
            //        ConnectionBuilderExtensions.UseConnectionHandler<MyConnectionHandler>(listenOption);
            //    });
            //});

        }
    }
}
