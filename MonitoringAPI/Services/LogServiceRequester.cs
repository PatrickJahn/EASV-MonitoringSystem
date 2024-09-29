using EasyNetQ;
using Shared.Entities.Logs;
using Shared.Messages.Logs;

namespace MonitoringAPI.Services
{
    public class LogServiceRequester
    {
        private readonly IBus _bus;
        private readonly TaskCompletionSource<List<LogEvent>> _taskCompletionSource;

        public LogServiceRequester(IBus bus)
        {
            _bus = bus;
            _taskCompletionSource = new TaskCompletionSource<List<LogEvent>>();
        }

        public async Task<List<LogEvent>> RequestLogsAsync(int page, int pageSize)
        {
            var requestMessage = new LogRequestMessage
            {
                Page = page,
                PageSize = pageSize
            };

            await _bus.PubSub.PublishAsync(requestMessage, "log-requests");

            // Wait for the response (consider adding a timeout mechanism)
            return await _taskCompletionSource.Task;
        }

        // Handle the response from LoggingService
        public void OnLogResponseReceived(LogResponseMessage response)
        {
            _taskCompletionSource.SetResult(response.LogEvents);
        }
    }
}