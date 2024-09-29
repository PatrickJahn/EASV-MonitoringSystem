using EasyNetQ;
using Shared.Messages.Logs;

namespace MonitoringAPI.Services
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IBus _bus;

        public MessagePublisher(IBus bus)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public async Task PublishLogEventMessage(LogEventMessage logEventMessage)
        {
            try
            {
                await _bus.PubSub.PublishAsync(logEventMessage, "log-events");
                Console.WriteLine("Log event published.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing log event: {ex.Message}");
            }
        }
    }
}