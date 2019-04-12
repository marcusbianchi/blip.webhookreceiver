using System.Threading.Tasks;
using blip.webhookreceiver.core.Models.Output;

namespace blip.webhookreceiver.core.Interfaces
{
    public interface IMessageRepository
    {
        void SaveMessage(OutputMessage ouputMessage);
    }
}
