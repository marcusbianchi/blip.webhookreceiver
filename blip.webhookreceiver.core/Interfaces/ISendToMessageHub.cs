using System.Threading.Tasks;
using blip.webhookreceiver.core.Models.Output;
using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.core.Interfaces
{
    /// <summary>
    /// Service to Send Message to MessageHub
    /// </summary>
    public interface ISendToMessageHub
    {
        /// <summary>
        /// Publish Event to Message Hub
        /// </summary>
        /// <param name="outputEvent">Event to send to the message hub</param>
        /// <returns></returns>
        Task PublishEvent(JObject outputEvent);
        /// <summary>
        /// Publish Message to Message Hub
        /// </summary>
        /// <param name="ouputMessage">Message to send to the message hub></param>
        /// <returns></returns>
        Task PublishMessage(JObject ouputMessage);
    }
}