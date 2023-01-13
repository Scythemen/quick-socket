using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cube.QuickSocket.Sample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    //builder.SetMinimumLevel(LogLevel.Trace);
                    //builder.ClearProviders();
                    builder.AddConfiguration(config.GetSection("Logging"));
                    builder.AddConsole();
                    builder.AddDebug();
                })
                //.AddHostedService<QuickSocketHostedService>()
                //.ConfigureOptions(new QuickSocketOptions() { EnableClient = true, EnableServer = true, ServerIpPorts = "0.0.0.0:9910" })
                .Configure<QuickSocketOptions>(config.GetSection("QuickSocketOptions"))
                .Configure<IConfiguration>(config)
                .BuildServiceProvider();

            var cancellationToken = new CancellationTokenSource();

            var hostedService = new QuickSocketHostedService(serviceProvider);

            await hostedService.StartAsync(cancellationToken.Token);

            var tcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, e) =>
            {
                tcs.TrySetResult(null);
                e.Cancel = true;
            };

            await tcs.Task;

            cancellationToken.Cancel();

            await hostedService.StopAsync(cancellationToken.Token);

        }
    }
}