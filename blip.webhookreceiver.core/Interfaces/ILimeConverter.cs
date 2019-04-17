using blip.webhookreceiver.core.Models.Output;
using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.core.Interfaces
{
    public interface ILimeConverter
    {
        /// <summary>
        /// Convert Lime message to Flattned Message
        /// </summary>
        /// <param name="json">Blip Message JSON</param>
        /// <returns></returns>
        OutputMessage ConvertToOutputMessage(JObject json);

        /// <summary>
        /// Convert Blip Event to Flattned Event
        /// </summary>
        /// <param name="json">Blip Event JSON</param>
        /// <returns></returns>
        OutputEvent ConvertToOutputEvent(JObject json);
    }
}