using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Cube.QuickSocket
{
    public class FirstMiddleware
    {
        private readonly ConnectionDelegate _next;
        //private readonly SemaphoreSlim _limiter;
        private readonly ILogger _logger;

        public FirstMiddleware(ConnectionDelegate next, ILogger<FirstMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task OnConnectionAsync(ConnectionContext connectionContext)
        {
            _logger.LogDebug("{0}: -> ", nameof(FirstMiddleware));

            await _next(connectionContext).ConfigureAwait(false);

        }
    }
}
