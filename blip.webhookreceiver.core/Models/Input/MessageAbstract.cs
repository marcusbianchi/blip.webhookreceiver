using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.core.Models.Input
{
    public abstract class MessageAbstract
    {
        public string type { get; set; }
        public string id { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public JObject metadata { get; set; }
    }
}