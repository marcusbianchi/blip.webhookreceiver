using System.Threading.Tasks;
using blip.webhookreceiver.core.Models.Output;

namespace blip.webhookreceiver.core.Interfaces
{
    public interface IEventRepository
    {
        Task SaveEvent(OutputEvent outputEvent);

    }
}