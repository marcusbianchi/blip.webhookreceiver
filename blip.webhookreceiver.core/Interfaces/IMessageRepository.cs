using System.Threading.Tasks;
using blip.webhookreceiver.core.Models.Output;

namespace blip.webhookreceiver.core.Interfaces
{
    /// <summary>
    /// Repository so save Messages
    /// </summary>
    public interface IMessageRepository
    {
        /// <summary>
        /// Save the message on the permanent Data.
        /// </summary>
        /// <param name="OutputMessage">Flattened Message</param>
        void SaveMessage(OutputMessage ouputMessage);
    }
}
