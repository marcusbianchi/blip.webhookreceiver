namespace blip.webhookreceiver.core.Models
{
    public class Content
    {
        public string target { get; set; }
        public string uri { get; set; }
        public string previewUri { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public string type { get; set; }
        public string aspectRatio { get; set; }
        public int size { get; set; }
        public string previewType { get; set; }
    }
}