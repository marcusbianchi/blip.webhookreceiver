using System;
using System.Threading.Tasks;
using blip.webhookreceiver.core.Interfaces;
using blip.webhookreceiver.core.Models.Input;
using blip.webhookreceiver.core.Models.Output;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.core.Services
{
    public class ReceiveLimeMessage : IReceiveLimeMessage
    {
        private readonly ISendToMessageHub _sendToMessageHub;
        private readonly ILogger _logger;

        public ReceiveLimeMessage(ISendToMessageHub sendToMessageHub, ILogger<ReceiveLimeMessage> logger)
        {
            _sendToMessageHub = sendToMessageHub;
            _logger = logger;
        }
        public async Task ProcessEvent(JObject json)
        {
            _logger.LogInformation("Event Received");
            await _sendToMessageHub.PublishEvent(json);
        }

        public async Task ProcessMessage(JObject json)
        {
            _logger.LogInformation("Message Received");
            await _sendToMessageHub.PublishMessage(json);
        }
    }
}