using System;
using System.IO;
using System.Threading.Tasks;
using blip.webhookreceiver.core.Interfaces;
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
            _messageSubscriber = await SubscriberClient.CreateAsync(_messageSubscriptionName);
            await _messageSubscriber.StartAsync(async (msg, cancellationToken) =>
            {
                string messageString = msg.Data.ToStringUtf8();
                OutputMessage outmessage = JsonConvert.DeserializeObject<OutputMessage>(messageString);
                await _messageRepository.SaveMessage(outmessage);
                // Return Reply.Ack to indicate this message has been handled.
                return SubscriberClient.Reply.Ack;
            });
        }
        public async Task StartSubscribeMessageHandler()
        {
            // Pull messages from the subscription using SimpleSubscriber.
            _eventSubscriber = await SubscriberClient.CreateAsync(_eventSubscriptionName);

            await _eventSubscriber.StartAsync(async (msg, cancellationToken) =>
            {
                string eventString = msg.Data.ToStringUtf8();
                OutputEvent outEvent = JsonConvert.DeserializeObject<OutputEvent>(eventString);
                await _eventRepository.SaveEvent(outEvent);
                // Return Reply.Ack to indicate this message has been handled.
                return SubscriberClient.Reply.Ack;
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
    }
}