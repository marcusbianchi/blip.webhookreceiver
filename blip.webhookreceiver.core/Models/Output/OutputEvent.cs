using System;

namespace blip.webhookreceiver.core.Models.Output
{
    public class OutputEvent
    {
        public string botIdentifier { get; set; }
        public string ownerIdentity { get; set; }
        public string identity { get; set; }
        public string externalId { get; set; }
        public string group { get; set; }
        public string source { get; set; }
        public string messageId { get; set; }
        public DateTime storageDate { get; set; }
        public string category { get; set; }
        public string action { get; set; }
        public string extras { get; set; }
        public string value { get; set; }
        public string label { get; set; }
    }
}