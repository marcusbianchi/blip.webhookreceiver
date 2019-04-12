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
        public DaemonService(IReceiveFromMessageHub receiveFromMessageHub)
        {
            _receiveFromMessageHub = receiveFromMessageHub;
        }
        public void Dispose()
        {

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _receiveFromMessageHub.StartSubscribeMessageHandler();
            _receiveFromMessageHub.StartSubscribeEventHandler();
            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _receiveFromMessageHub.StopSubscribeEventHandler();
            _receiveFromMessageHub.StopSubscribeMessageHandler();
            return Task.CompletedTask;

        }
    }
}