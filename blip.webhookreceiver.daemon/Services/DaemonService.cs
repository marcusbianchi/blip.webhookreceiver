using System;
using System.Threading;
using System.Threading.Tasks;
using blip.webhookreceiver.core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace blip.webhookreceiver.daemon.Services
{
    public class DaemonService : IHostedService, IDisposable
    {
        private readonly IReceiveFromMessageHub _receiveFromMessageHub;
        private readonly ILogger _logger;

        public DaemonService(IReceiveFromMessageHub receiveFromMessageHub, ILogger<DaemonService> logger)
        {
            _receiveFromMessageHub = receiveFromMessageHub;
            _logger = logger;
        }
        public void Dispose()
        {
            _logger.LogInformation("Service Disposed");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
                await _receiveFromMessageHub.StartSubscribeMessageHandler()
            );
            Task.Run(async () =>
                await _receiveFromMessageHub.StartSubscribeEventHandler()
            );
            _logger.LogInformation("Service Started");
            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _receiveFromMessageHub.StopSubscribeEventHandler();
            _receiveFromMessageHub.StopSubscribeMessageHandler();
            _logger.LogInformation("Service Stopped");
            return Task.CompletedTask;

        }
    }
}