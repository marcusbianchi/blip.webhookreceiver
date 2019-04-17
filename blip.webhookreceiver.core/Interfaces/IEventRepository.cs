using System.Collections.Generic;
using System.Threading.Tasks;
using blip.webhookreceiver.core.Models.Output;

namespace blip.webhookreceiver.core.Interfaces
{
    /// <summary>
    /// Repository so save events
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Save the event on the permanent Data.
        /// </summary>
        /// <param name="outputEvent">Flattened event</param>
        Task SaveEvent(OutputEvent outputEvent);

        /// <summary>
        /// Save batch of the events on the permanent Data.
        /// </summary>
        /// <param name="outputEvent">List of Flattened event</param>
        Task SaveEventBatch(IList<OutputEvent> outputEvent);

    }
}