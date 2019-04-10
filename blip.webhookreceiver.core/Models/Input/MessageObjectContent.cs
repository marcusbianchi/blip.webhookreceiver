namespace blip.webhookreceiver.core.Models.Input
{
    public class MessageObjectContent : MessageAbstract
    {
        public string target { get; set; }
        public string uri { get; set; }
        public string previewUri { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        
    }
}