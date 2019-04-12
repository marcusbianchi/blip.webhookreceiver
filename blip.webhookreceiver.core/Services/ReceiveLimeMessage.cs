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
            string botIdentifier = "";
            if (json["from"] != null && json["from"].ToString().Split('@')[1] == "msging.net")
            {
                botIdentifier = json["from"].ToString().Split('@')[1].Split('/')[0];
            }
            if (json["to"] != null && json["to"].ToString().Split('@')[1] == "msging.net")
            {
                botIdentifier = json["to"].ToString().Split('@')[1].Split('/')[0];
            }
            OutputMessage outputMessage;
            if (json["type"].ToString() != "text/plain")
            {
                var blipMmessage = json.ToObject<MessageObjectContent>();
                outputMessage = new OutputMessage
                {
                    botIdentifier = botIdentifier,
                    type = blipMmessage.type,
                    id = blipMmessage.id,
                    from = blipMmessage.from,
                    to = blipMmessage.to,
                    metadata = blipMmessage.metadata.ToString(),
                    text = blipMmessage.text,
                    target = blipMmessage.target,
                    previewUri = blipMmessage.previewUri,
                    uri = blipMmessage.uri,
                    title = blipMmessage.title,
                    storageDate = DateTime.Now
                };
                await _sendToMessageHub.PublishMessage(outputMessage);
            }
            else
            {
                var cBlipMmessage = json.ToObject<MessageTextContent>();
                outputMessage = new OutputMessage
                {
                    botIdentifier = botIdentifier,
                    type = cBlipMmessage.type,
                    id = cBlipMmessage.id,
                    from = cBlipMmessage.from,
                    to = cBlipMmessage.to,
                    metadata = cBlipMmessage.metadata.ToString(),
                    content = cBlipMmessage.content,
                    storageDate = DateTime.Now
                };
                await _sendToMessageHub.PublishMessage(outputMessage);
            }
        }
    }
}