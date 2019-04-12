using System;
using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.core.Models.Input
{
    public class Event
    {
        public string ownerIdentity { get; set; }
        public Contact contact { get; set; }
        public string messageId { get; set; }
        public DateTime storageDate { get; set; }
        public string category { get; set; }
        public string action { get; set; }
        public string value { get; set; }
        public string label { get; set; }

        public JObject extras { get; set; }
    }
}