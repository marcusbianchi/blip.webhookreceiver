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
        private readonly ILimeConverter _limeConverter;
        public ReceiveFromGoogleMessageHub(IMessageRepository messageRepository, IEventRepository eventRepository, ILogger<ReceiveFromGoogleMessageHub> logger, ILimeConverter limeConverter)
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
            _limeConverter = limeConverter;
            _logger.LogInformation("GCP Information set. projectId: {projectId} eventSubscriptionName: {eventSubscriptionName},messageSubscriptionName:{messageSubscriptionName}, ", projectId, eventSubscriptionName, messageSubscriptionName);

        }
        public async Task StartSubscribeEventHandler()
        {
            _logger.LogInformation("Event Listener starting for {eventSubscriptionName}", _eventSubscriptionName.SubscriptionId);

            // Pull messages from the subscription using SimpleSubscriber.
            _eventSubscriber = await SubscriberClient.CreateAsync(_eventSubscriptionName);
            await _eventSubscriber.StartAsync(async (msg, cancellationToken) =>
            {
                try
                {
                    _logger.LogDebug("Event receipt from {eventSubscriptionName}", _eventSubscriptionName.SubscriptionId);
                    string messageString = msg.Data.ToStringUtf8();
                    JObject json = JsonConvert.DeserializeObject<JObject>(messageString);
                    OutputEvent outEvent = _limeConverter.ConvertToOutputEvent(json);
                    _logger.LogInformation("Event receipt {id}", outEvent.id);

                    await _eventRepository.SaveEvent(outEvent);
                    // Return Reply.Ack to indicate this message has been handled.
                    return SubscriberClient.Reply.Ack;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " " + msg.Data.ToStringUtf8());
                    return SubscriberClient.Reply.Nack;
                }
            });

        }
        public async Task StartSubscribeMessageHandler()
        {
            _logger.LogInformation("Message Listener starting for {eventSubscriptionName}", _eventSubscriptionName.SubscriptionId);

            // Pull messages from the subscription using SimpleSubscriber.
            _messageSubscriber = await SubscriberClient.CreateAsync(_messageSubscriptionName);

            await _messageSubscriber.StartAsync(async (msg, cancellationToken) =>
            {
                try
                {
                    _logger.LogDebug("Message receipt from {messageSubscriptionName}", _messageSubscriptionName.SubscriptionId);
                    string messageString = msg.Data.ToStringUtf8();
                    JObject json = JsonConvert.DeserializeObject<JObject>(messageString);
                    OutputMessage outputMessage = _limeConverter.ConvertToOutputMessage(json);
                    _logger.LogInformation("Message receipt {id}", outputMessage.id);
                    await _messageRepository.SaveMessage(outputMessage);
                    // Return Reply.Ack to indicate this message has been handled.
                    return SubscriberClient.Reply.Ack;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " " + msg.Data.ToStringUtf8());
                    return SubscriberClient.Reply.Nack;
                }
            });
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


    }
}