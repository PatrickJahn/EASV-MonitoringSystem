using EasyNetQ;
using Shared.Messages.Logs;

namespace LoggingService.Services;

public class LogRequestHandler
{
    private readonly ILogService _logService;
    private readonly IBus _bus;

    public LogRequestHandler(ILogService logService, IBus bus)
    {
        _logService = logService;
        _bus = bus;
    }

    public void SubscribeToLogRequests()
    {
        _bus.PubSub.SubscribeAsync<LogRequestMessage>("log-request", async (request) =>
        {
            var logs = await _logService.GetLogEventsAsync(request.Page, request.PageSize);
            var responseMessage = new LogResponseMessage { LogEvents = logs };
            await _bus.PubSub.PublishAsync(responseMessage, "log-response");
        });
    }
}