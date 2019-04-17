using System;
using System.Threading.Tasks;
using blip.webhookreceiver.bigquery.Services;
using blip.webhookreceiver.core.Interfaces;
using blip.webhookreceiver.core.Services;
using blip.webhookreceiver.daemon.Services;
using blip.webhookreceiver.pubsub.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace blip.webhookreceiver.daemon
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config.AddEnvironmentVariables();

                   if (args != null)
                   {
                       config.AddCommandLine(args);
                   }
               })
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddTransient<ILimeConverter, LimeConverter>();
                   services.AddTransient<IReceiveFromMessageHub, ReceiveFromGoogleMessageHub>();
                   services.AddTransient<IEventRepository, BigQueryEventRespository>();
                   services.AddTransient<IMessageRepository, BigQueryMessageRespository>();
                   services.AddSingleton<IHostedService, DaemonService>();

               })
               .ConfigureLogging((hostingContext, logging) =>
               {
                   logging.AddConsole();
               });

            await builder.RunConsoleAsync();
        }
    }
}
