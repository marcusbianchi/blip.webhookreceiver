using System.Threading.Tasks;
using blip.webhookreceiver.core.Models;
using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.core.Interfaces
{
    /// <summary>
    /// Receive Lime Messages to process to Pub/Sub endoint
    /// </summary>
    public interface IReceiveLimeMessage
    {
        /// <summary>
        /// Process Message and send to PubSub
        /// </summary>
        /// <param name="json"> Message JSON received from webhook</param>
        void ProcessMessage(JObject json);
        /// <summary>
        /// Processs Events and send to PubSub
        /// </summary>
        /// <param name="json">Event JSON received from blip</param>
        void ProcessEvent(JObject json);
    }
}