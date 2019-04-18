using System.Collections.Generic;
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
        Task SaveMessage(OutputMessage ouputMessage);

        /// <summary>
        /// Process Batch Messages
        /// </summary>
        /// <param name="ouputMessage">List Flattened Message</param>
        Task SaveMessageBatch(IList<OutputMessage> ouputMessage);

    }
}
