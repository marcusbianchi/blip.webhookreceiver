using System;
using System.IO;
using System.Threading.Tasks;
using blip.webhookreceiver.core.Interfaces;
using blip.webhookreceiver.core.Models.Input;
using blip.webhookreceiver.core.Models.Output;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;
        public ReceiveFromGoogleMessageHub(IMessageRepository messageRepository, IEventRepository eventRepository, ILogger<ReceiveFromGoogleMessageHub> logger)
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
            _logger = logger;
            _logger.LogInformation("GCP Information set. projectId: {projectId} eventSubscriptionName: {eventSubscriptionName},messageSubscriptionName:{messageSubscriptionName}, ", projectId, eventSubscriptionName, messageSubscriptionName);

        }
        public async Task StartSubscribeEventHandler()
        {
            _logger.LogInformation("Event Listener starting for {eventSubscriptionName}", _eventSubscriptionName.SubscriptionId);

            // Pull messages from the subscription using SimpleSubscriber.
            _eventSubscriber = await SubscriberClient.CreateAsync(_eventSubscriptionName);
            await _eventSubscriber.StartAsync((msg, cancellationToken) =>
            {
                try
                {
                    _logger.LogInformation("Event receipt from {eventSubscriptionName}", _eventSubscriptionName.SubscriptionId);
                    string messageString = msg.Data.ToStringUtf8();
                    JObject json = JsonConvert.DeserializeObject<JObject>(messageString);
                    OutputEvent outEvent = ConvertToOutputEvent(json);
                    _eventRepository.SaveEvent(outEvent);
                    // Return Reply.Ack to indicate this message has been handled.
                    return Task.FromResult(SubscriberClient.Reply.Ack);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return Task.FromResult(SubscriberClient.Reply.Nack);
                }
            });
            _logger.LogInformation("Event Listener started for {eventSubscriptionName}", _eventSubscriptionName.SubscriptionId);

        }
        public async Task StartSubscribeMessageHandler()
        {
            _logger.LogInformation("Message Listener starting for {eventSubscriptionName}", _eventSubscriptionName.SubscriptionId);

            // Pull messages from the subscription using SimpleSubscriber.
            _messageSubscriber = await SubscriberClient.CreateAsync(_messageSubscriptionName);

            await _messageSubscriber.StartAsync((msg, cancellationToken) =>
            {
                try
                {
                    _logger.LogInformation("Event receipt from {messageSubscriptionName}", _messageSubscriptionName.SubscriptionId);
                    string messageString = msg.Data.ToStringUtf8();
                    JObject json = JsonConvert.DeserializeObject<JObject>(messageString);
                    OutputMessage outputMessage = ConvertToOutputMessage(json);
                    _messageRepository.SaveMessage(outputMessage);
                    // Return Reply.Ack to indicate this message has been handled.
                    return Task.FromResult(SubscriberClient.Reply.Ack);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return Task.FromResult(SubscriberClient.Reply.Nack);
                }
            });
            _logger.LogInformation("Message Listener started for {eventSubscriptionName}", _eventSubscriptionName.SubscriptionId);
        }

        public async Task StopSubscribeEventHandler()
        {
            if (_eventSubscriber != null)
            {
                await _eventSubscriber.StopAsync(TimeSpan.FromSeconds(15));
                _logger.LogInformation("Event Listener stoped for {eventSubscriptionName}", _eventSubscriptionName.SubscriptionId);
            }
        }

        public async Task StopSubscribeMessageHandler()
        {
            if (_messageSubscriber != null)
            {
                await _messageSubscriber.StopAsync(TimeSpan.FromSeconds(15));
                _logger.LogInformation("Event Listener stoped for {messageSubscriptionName}", _messageSubscriptionName.SubscriptionId);
            }
        }

        private OutputMessage ConvertToOutputMessage(JObject json)
        {
            string botIdentifier = "";
            var direction = "";
            if (json["from"] != null && json["from"].ToString().Split('@')[1] == "msging.net")
            {
                botIdentifier = json["from"].ToString().Split('@')[0];
                direction = "sent";
            }
            if (json["to"] != null && json["to"].ToString().Split('@')[1] == "msging.net")
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
                    metadata = blipMmessage.metadata.ToString(),
                    text = blipMmessage.text,
                    target = blipMmessage.target,
                    previewUri = blipMmessage.previewUri,
                    uri = blipMmessage.uri,
                    title = blipMmessage.title,
                    storageDate = DateTime.Now
                };
                _logger.LogInformation("Message Identified as NOT plain text for botIdentifier: {botIdentifier}, Direction: {direction}", botIdentifier, direction);
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
                _logger.LogInformation("Message Identified AS plain text for botIdentifier: {botIdentifier}, Direction: {direction}", botIdentifier, direction);
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
            _logger.LogInformation("Message Identified AS event for botIdentifier: {botIdentifier}", outputEvent.botIdentifier);
            return outputEvent;
        }
    }
}