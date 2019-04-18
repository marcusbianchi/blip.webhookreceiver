using System;
using blip.webhookreceiver.core.Interfaces;
using blip.webhookreceiver.core.Models.Input;
using blip.webhookreceiver.core.Models.Output;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.core.Services
{
    public class LimeConverter : ILimeConverter
    {
        private readonly ILogger _logger;
        public LimeConverter(ILogger<LimeConverter> logger)
        {
            _logger = logger;
        }

        public OutputEvent ConvertToOutputEvent(JObject json)
        {
            var cblipEvent = json.ToObject<Event>();

            OutputEvent outputEvent = new OutputEvent
            {
                botIdentifier = cblipEvent.ownerIdentity?.Split('@')[0],
                ownerIdentity = cblipEvent.ownerIdentity,
                identity = cblipEvent.contact?.Identity,
                externalId = cblipEvent.contact?.ExternalId,
                group = cblipEvent.contact?.Group,
                source = cblipEvent.contact?.Source,
                messageId = cblipEvent.messageId,
                storageDate = cblipEvent.storageDate.ToUniversalTime().ToString(),
                category = cblipEvent.category,
                action = cblipEvent.action,
                extras = cblipEvent.extras.ToString(),
                value = cblipEvent.value,
                label = cblipEvent.label,
                id = cblipEvent.id,
            };
            _logger.LogDebug("Message Identified AS event for botIdentifier: {botIdentifier}", outputEvent.botIdentifier);
            return outputEvent;
        }

        public OutputMessage ConvertToOutputMessage(JObject json)
        {
            string botIdentifier = "";
            var direction = "";
            if (json["from"] != null && json["from"].ToString().Contains("@msging.net"))
            {
                botIdentifier = json["from"].ToString().Split('@')[0];
                direction = "sent";
            }
            else if (json["to"] != null && json["to"].ToString().Contains("@msging.net"))
            {
                botIdentifier = json["to"].ToString().Split('@')[0];
                direction = "receipt";
            }
            OutputMessage outputMessage;
            if (json["type"].ToString() != "text/plain")
            {
                var blipMmessage = json.ToObject<MessageObjectContent>();
                outputMessage = new OutputMessage
                {
                    botIdentifier = botIdentifier,
                    direction = direction,
                    type = blipMmessage.type,
                    id = blipMmessage.id,
                    from = blipMmessage.from,
                    to = blipMmessage.to,
                    metadata = blipMmessage.metadata?.ToString(),
                    text = blipMmessage.text,
                    target = blipMmessage.target,
                    previewUri = blipMmessage.previewUri,
                    uri = blipMmessage.uri,
                    title = blipMmessage.title,
                    storageDate = DateTime.Now.ToUniversalTime().ToString()
                };
                _logger.LogDebug("Message Identified as NOT plain text for botIdentifier: {botIdentifier}, Direction: {direction}", botIdentifier, direction);
                return outputMessage;
            }
            else
            {
                var cBlipMmessage = json.ToObject<MessageTextContent>();
                outputMessage = new OutputMessage
                {
                    botIdentifier = botIdentifier,
                    type = cBlipMmessage.type,
                    direction = direction,
                    id = cBlipMmessage.id,
                    from = cBlipMmessage.from,
                    to = cBlipMmessage.to,
                    metadata = cBlipMmessage.metadata?.ToString(),
                    content = cBlipMmessage.content,
                    storageDate = DateTime.Now.ToUniversalTime().ToString()
                };
                _logger.LogDebug("Message Identified AS plain text for botIdentifier: {botIdentifier}, Direction: {direction}", botIdentifier, direction);
                return outputMessage;
            }
        }
    }
}