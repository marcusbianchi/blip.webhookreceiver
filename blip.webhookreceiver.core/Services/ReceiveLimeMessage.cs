using System;
using System.Threading.Tasks;
using blip.webhookreceiver.core.Interfaces;
using blip.webhookreceiver.core.Models.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.core.Services
{
    public class ReceiveLimeMessage : IReceiveLimeMessage
    {
        public void ProcessEvent(JObject json)
        {
            var cblipEvent = json.ToObject<Event>();
        }

        public void ProcessMessage(JObject json)
        {
            if (json["type"].ToString() != "text/plain")
            {
                var cBlipMmessage = json.ToObject<MessageObjectContent>();
            }
            else
            {
                var cBlipMmessage = json.ToObject<MessageTextContent>();
            }
        }
    }
}