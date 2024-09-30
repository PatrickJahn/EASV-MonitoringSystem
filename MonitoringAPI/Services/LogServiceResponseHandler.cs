using EasyNetQ;
using Shared.Messages.Logs;

namespace MonitoringAPI.Services
{
    public class LogServiceResponseHandler
    {
        public LogServiceResponseHandler(LogServiceRequester logServiceRequester, IBus bus)
        {
            // Subscribe to log-response messages
            bus.PubSub.SubscribeAsync<LogResponseMessage>("log-response", logServiceRequester.OnLogResponseReceived);
        }
    }
}