using System;
namespace blip.webhookreceiver.core.Models.Output
{
    public class OutputMessage
    {
        public string botIdentifier { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string metadata { get; set; }
        public string content { get; set; }
        public string target { get; set; }
        public string uri { get; set; }
        public string previewUri { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public string direction { get; set; }
        public DateTime storageDate {get;set;}
    }
}