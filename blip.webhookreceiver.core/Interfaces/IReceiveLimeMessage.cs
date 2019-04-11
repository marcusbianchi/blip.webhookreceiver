using System.Threading.Tasks;
using blip.webhookreceiver.core.Models;
using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.core.Interfaces
{
    /// <summary>
    /// Receive Lime Messages to process and format to send to pub/sub endpoint
    /// </summary>
    public interface IReceiveLimeMessage
    {
        /// <summary>
        /// Process Message and send to PubSub
        /// </summary>
        /// <param name="json"> Message JSON received from webhook</param>
        Task ProcessMessage(JObject json);
        /// <summary>
        /// Processs Events and send to PubSub
        /// </summary>
        /// <param name="json">Event JSON received from blip</param>
        Task ProcessEvent(JObject json);
    }
}