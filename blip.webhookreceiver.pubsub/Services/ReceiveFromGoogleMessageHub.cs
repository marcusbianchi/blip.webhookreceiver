using System;
using System.IO;
using System.Threading.Tasks;
using blip.webhookreceiver.core.Interfaces;
using blip.webhookreceiver.core.Models.Input;
using blip.webhookreceiver.core.Models.Output;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.pubsub.Services
{
    public class ReceiveFromGoogleMessageHub : IReceiveFromMessageHub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IEventRepository _eventRepository;
        private readonly SubscriptionName _eventSubscriptionName;
        private readonly SubscriptionName _messageSubscriptionName;
        private SubscriberClient _eventSubscriber;
        private SubscriberClient _messageSubscriber;
        public ReceiveFromGoogleMessageHub(IMessageRepository messageRepository, IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _messageRepository = messageRepository;

            // Get projectId fron config
            string googleCredentialsText = File.ReadAllText(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
            JObject googleCredentials = JObject.Parse(googleCredentialsText);
            string projectId = googleCredentials["project_id"].ToString();

            // Get Topic Names
            string eventSubscriptionName = Environment.GetEnvironmentVariable("EVENT_SUBSCRIPTION_NAME");
            string messageSubscriptionName = Environment.GetEnvironmentVariable("MESSAGE_SUBSCRIPTION_NAME");

            _eventSubscriptionName = new SubscriptionName(projectId, eventSubscriptionName);
            _messageSubscriptionName = new SubscriptionName(projectId, messageSubscriptionName);
        }
        public async Task StartSubscribeEventHandler()
        {
            // Pull messages from the subscription using SimpleSubscriber.
            _eventSubscriber = await SubscriberClient.CreateAsync(_messageSubscriptionName);
            await _eventSubscriber.StartAsync((msg, cancellationToken) =>
            {
                string messageString = msg.Data.ToStringUtf8();
                JObject json = JsonConvert.DeserializeObject<JObject>(messageString);
                OutputEvent outEvent  = ConvertToOutputEvent(json); 
                _eventRepository.SaveEvent(outEvent);
                // Return Reply.Ack to indicate this message has been handled.
                return Task.FromResult(SubscriberClient.Reply.Ack);
            });
        }
        public async Task StartSubscribeMessageHandler()
        {
            // Pull messages from the subscription using SimpleSubscriber.
            _messageSubscriber = await SubscriberClient.CreateAsync(_eventSubscriptionName);

            await _messageSubscriber.StartAsync((msg, cancellationToken) =>
            {
                string messageString = msg.Data.ToStringUtf8();
                JObject json = JsonConvert.DeserializeObject<JObject>(messageString);
                OutputMessage outputMessage = ConvertToOutputMessage(json); 
                _messageRepository.SaveMessage(outputMessage);
                // Return Reply.Ack to indicate this message has been handled.
                return Task.FromResult(SubscriberClient.Reply.Ack);
            });
        }

        public async Task StopSubscribeEventHandler()
        {
            if (_eventSubscriber != null)
            {
                await _eventSubscriber.StopAsync(TimeSpan.FromSeconds(15));
            }
        }

        public async Task StopSubscribeMessageHandler()
        {
            if (_messageSubscriber != null)
            {
                await _messageSubscriber.StopAsync(TimeSpan.FromSeconds(15));
            }
        }

        private OutputMessage ConvertToOutputMessage(JObject json)
        {
            string botIdentifier = "";
            if (json["from"] != null && json["from"].ToString().Split('@')[1] == "msging.net")
            {
                botIdentifier = json["from"].ToString().Split('@')[0];
            }
            if (json["to"] != null && json["to"].ToString().Split('@')[1] == "msging.net")
            {
                botIdentifier = json["to"].ToString().Split('@')[0];
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
                return outputMessage;
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
                return outputMessage;
            }
        }

        private OutputEvent ConvertToOutputEvent(JObject json)
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
                storageDate = cblipEvent.storageDate,
                category = cblipEvent.category,
                action = cblipEvent.action,
                extras = cblipEvent.extras.ToString(),
                value = cblipEvent.value,
                label = cblipEvent.label

            };
            return outputEvent;
        }
    }
}