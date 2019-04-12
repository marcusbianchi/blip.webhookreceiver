using System.Threading.Tasks;

namespace blip.webhookreceiver.core.Interfaces
{
    public interface IReceiveFromMessageHub
    {
        Task StartSubscribeEventHandler();
        Task StartSubscribeMessageHandler();
        Task StopSubscribeEventHandler();
        Task StopSubscribeMessageHandler();
    }
}