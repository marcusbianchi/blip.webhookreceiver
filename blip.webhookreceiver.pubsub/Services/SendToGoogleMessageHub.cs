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
    public class SendToGoogleMessageHub : ISendToMessageHub
    {
        private readonly PublisherServiceApiClient _publisher;
        private readonly TopicName _eventTopicName;
        private readonly TopicName _messageTopicName;
        public SendToGoogleMessageHub()
        {
            // Get projectId fron config
            string googleCredentialsText = File.ReadAllText(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
            JObject googleCredentials = JObject.Parse(googleCredentialsText);
            string projectId = googleCredentials["project_id"].ToString();

            // Get Topic Names
            string eventTopicName = Environment.GetEnvironmentVariable("EVENT_TOPIC_NAME");
            string messageTopicName = Environment.GetEnvironmentVariable("MESSAGE_TOPIC_NAME");

            //Create the topic name reference
            _eventTopicName = new TopicName(projectId, eventTopicName);
            _messageTopicName = new TopicName(projectId, messageTopicName);

            //Create Publisher
            _publisher = PublisherServiceApiClient.Create();
        }
        public async Task PublishEvent(OutputEvent outputEvent)
        {
            // Convert object to string
            string jsonOutputEvent = JsonConvert.SerializeObject(outputEvent);
            // Publish a message to the topic.
            PubsubMessage message = new PubsubMessage
            {
                // The data is any arbitrary ByteString. Here, we're using text.
                Data = ByteString.CopyFromUtf8(jsonOutputEvent),
                // The attributes provide metadata in a string-to-string dictionary.
                Attributes =
                            {
                                { "botIdentifier", outputEvent.botIdentifier }
                            }
            };
            await _publisher.PublishAsync(_eventTopicName, new[] { message });
        }

        public async Task PublishMessage(OutputMessage ouputMessage)
        {
            // Convert object to string
            string jsonOutputMessage = JsonConvert.SerializeObject(ouputMessage);
            // Publish a message to the topic.
            PubsubMessage message = new PubsubMessage
            {
                // The data is any arbitrary ByteString. Here, we're using text.
                Data = ByteString.CopyFromUtf8(jsonOutputMessage),
                // The attributes provide metadata in a string-to-string dictionary.
                Attributes =
                    {
                        { "botIdentifier", ouputMessage.botIdentifier }
                    }
            };
            await _publisher.PublishAsync(_messageTopicName, new[] { message });
        }
    }
}