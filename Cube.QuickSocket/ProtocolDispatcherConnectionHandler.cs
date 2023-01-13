using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cube.QuickSocket
{
    internal class ProtocolDispatcherConnectionHandler : ConnectionHandler
    {
        private readonly ILogger logger;
        public ProtocolDispatcherConnectionHandler(ILogger<ProtocolDispatcherConnectionHandler> logger)
        {
            this.logger = logger;
        }


        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            var closeReason = "Unknown";
            var ip_port = connection.RemoteEndPoint.ToString();

            logger.LogDebug("New connection: {0}, {1}  ", connection.ConnectionId, connection.RemoteEndPoint);

            try
            {
                while (true)
                {
                    var result = await connection.Transport.Input.ReadAsync();
                    var buffer = result.Buffer;

                    if (result.IsCompleted)
                    {
                        connection.Transport.Input.AdvanceTo(buffer.End, buffer.End);
                        closeReason = "Remote close";
                        break;
                    }

                    if (logger.IsEnabled(LogLevel.Trace))
                    {
                        logger.LogTrace("Recv({0}) < {1} << {2} ", buffer.Length, ip_port, ToHex(buffer));
                    }

                    SequencePosition consumed = buffer.End;

                    SequenceMarshal.TryGetReadOnlyMemory<byte>(buffer, out ReadOnlyMemory<byte> memory);

                    await connection.Transport.Output.WriteAsync(memory);

                    logger.LogTrace("Send({0}) > {1} >> {2} ", memory.Length, ip_port, ToHex(buffer));

                    connection.Transport.Input.AdvanceTo(consumed, buffer.End);

                }

                //logger.LogInformation("Disconnected: {0}, {1}, {2}", connection.ConnectionId, connection.LocalEndPoint, closeReason);
            }
            catch (ConnectionAbortedException abortedException)
            {
                closeReason = abortedException.Message;
                logger.LogTrace(abortedException.Message);
            }
            catch (ConnectionResetException resetException)
            {
                closeReason = resetException.Message;
                logger.LogTrace(resetException.Message);
            }
            catch (Exception ex)
            {
                closeReason = ex.Message;
                logger.LogError(ex, ex.Message);
            }
            finally
            {
                logger.LogInformation("exit......");
            }

        }


        private static readonly string[] hexArray = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
        public static string ToHex(ReadOnlySequence<byte> bytes)
        {
            if (bytes.IsEmpty) return string.Empty;

            var sb = new StringBuilder((int)bytes.Length * 2);
            foreach (var memory in bytes)
            {
                foreach (var b in memory.Span)
                {
                    sb.Append(hexArray[(b & 0xf0) >> 4] + hexArray[b & 0x0f]);
                }
            }
            return sb.ToString();
        }


    }
}
