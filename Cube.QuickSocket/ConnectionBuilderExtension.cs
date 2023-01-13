using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cube.QuickSocket
{
    public static class ConnectionBuilderExtension
    {
        public static void AddMiddleware<TMiddleware>(this IConnectionBuilder builder) where TMiddleware : IConnectionMiddleware
        {
            //loggerFactory ??= builder.ApplicationServices.GetRequiredService<ILoggerFactory>();
            //var logger = loggerName == null ? loggerFactory.CreateLogger<LoggingConnectionMiddleware>() : loggerFactory.CreateLogger(loggerName);
            //builder.Use(next => new LoggingConnectionMiddleware(next, logger, loggingFormatter).OnConnectionAsync);


            //ILoggerFactory loggerFactory = builder.ApplicationServices.GetRequiredService<ILoggerFactory>();
            //var logger = loggerFactory.CreateLogger<TMiddleware>();
            //var middleware = ActivatorUtilities.CreateInstance<TMiddleware>(builder.ApplicationServices);
            //if (middleware == null)
            //{
            //    throw new NotSupportedException($"Failed to create the instance of middleware {nameof(middleware)}");
            //}
            //ConnectionDelegate d = (ctx) => middleware.OnConnectionAsync(ctx);

            //builder.Use(next => d.Invoke(connection)) ;
        }

    }
}
