using System.Threading.Tasks;

namespace blip.webhookreceiver.core.Interfaces
{
    /// <summary>
    /// Handle the receipt of messages on any message hub
    /// </summary>
    public interface IReceiveFromMessageHub
    {
        /// <summary>
        /// Start the Handling of Events that comes throught the MessageHUb
        /// </summary>
        /// <returns></returns>
        Task StartSubscribeEventHandler();
        /// <summary>
        /// Start the Handling of Messages that comes throught the MessageHUb
        /// </summary>
        /// <returns></returns>
        Task StartSubscribeMessageHandler();
        /// <summary>
        /// Stop the Handling of Events that comes throught the MessageHUb
        /// </summary>
        /// <returns></returns>
        Task StopSubscribeEventHandler();
        /// <summary>
        /// Stop the Handling of Messages that comes throught the MessageHUb
        /// </summary>
        /// <returns></returns>
        Task StopSubscribeMessageHandler();
    }
}