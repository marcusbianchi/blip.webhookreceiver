using System.Threading.Tasks;
using blip.webhookreceiver.core.Models.Output;

namespace blip.webhookreceiver.core.Interfaces
{
    public interface IMessageRepository
    {
        Task SaveMessage(OutputMessage ouputMessage);
    }
}
