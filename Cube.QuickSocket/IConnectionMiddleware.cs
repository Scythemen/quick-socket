using Microsoft.AspNetCore.Connections;

namespace Cube.QuickSocket
{
    public interface IConnectionMiddleware
    {
        public Task OnConnectionAsync(ConnectionContext connectionContext);
    }
}
