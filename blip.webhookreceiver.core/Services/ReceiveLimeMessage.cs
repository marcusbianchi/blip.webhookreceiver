using System;
using System.Threading.Tasks;
using blip.webhookreceiver.core.Interfaces;
using blip.webhookreceiver.core.Models.Input;
using blip.webhookreceiver.core.Models.Output;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.core.Services
{
    public class ReceiveLimeMessage : IReceiveLimeMessage
    {
        private readonly ISendToMessageHub _sendToMessageHub;
        public ReceiveLimeMessage(ISendToMessageHub sendToMessageHub)
        {
            _sendToMessageHub = sendToMessageHub;
        }
        public async Task ProcessEvent(JObject json)
        {
            var cblipEvent = json.ToObject<Event>();
            OutputEvent outputEvent = new OutputEvent
            {
                botIdentifier = cblipEvent.ownerIdentity?.Split('@')[0],
                ownerIdentity = cblipEvent.ownerIdentity,
                Identity = cblipEvent.contact?.Identity,
                messageId = cblipEvent.messageId,
                storageDate = cblipEvent.storageDate,
                category = cblipEvent.category,
                action = cblipEvent.action,
                extras = cblipEvent.extras.ToString()

            };
            await _sendToMessageHub.PublishEvent(outputEvent);
        }

        public async Task ProcessMessage(JObject json)
        {
            OutputMessage outputMessage;
            if (json["type"].ToString() != "text/plain")
            {
                var blipMmessage = json.ToObject<MessageObjectContent>();
                outputMessage = new OutputMessage
                {
                    botIdentifier = blipMmessage.to.Split('@')[0],
                    type = blipMmessage.type,
                    id = blipMmessage.id,
                    from = blipMmessage.from,
                    metadata = blipMmessage.metadata.ToString(),
                    text = blipMmessage.text,
                    target = blipMmessage.target,
                    previewUri = blipMmessage.previewUri,
                    uri = blipMmessage.uri,
                    title = blipMmessage.title
                };
                await _sendToMessageHub.PublishMessage(outputMessage);
            }
            else
            {
                var cBlipMmessage = json.ToObject<MessageTextContent>();
                outputMessage = new OutputMessage
                {
                    botIdentifier = cBlipMmessage.to.Split('@')[0],
                    type = cBlipMmessage.type,
                    id = cBlipMmessage.id,
                    from = cBlipMmessage.from,
                    metadata = cBlipMmessage.metadata.ToString(),
                    content = cBlipMmessage.content
                };
                await _sendToMessageHub.PublishMessage(outputMessage);
            }
        }
    }
}