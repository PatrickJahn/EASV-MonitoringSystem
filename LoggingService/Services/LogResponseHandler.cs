using EasyNetQ;
using Shared.Entities.Logs;
using Shared.Messages.Logs;

namespace LoggingService.Services
{
    public class LogResponsePublisher
    {
        private readonly IBus _bus;

        public LogResponsePublisher(IBus bus)
        {
            _bus = bus;
        }

        public async Task PublishLogResponseAsync(List<LogEvent> logs)
        {
            var responseMessage = new LogResponseMessage { LogEvents = logs };
            await _bus.PubSub.PublishAsync(responseMessage, "log-response");
        }
    }
}