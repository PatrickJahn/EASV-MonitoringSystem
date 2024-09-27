using Microsoft.AspNetCore.Mvc;
using MonitoringAPI.Controllers.Requests;
using MonitoringAPI.Services;
using Shared.Entities.Logs;
using Shared.Messages.Logs;

namespace MonitoringAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class LoggingController : ControllerBase
{

    private readonly IMessagePublisher _messagePublisher;
    public LoggingController(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
    }


    [HttpGet(Name = "GetLogs")]
    public async Task<IEnumerable<LogEvent>> GetLogs()
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    public async Task AddLogEvent([FromBody] CreateEventLogRequest request)
    {
        await _messagePublisher.PublishLogEventMessage(new LogEventMessage()
        {
            Message = request.Message,
            ErrorDetails = request.ErrorDetails,
            FilePath = request.FilePath,
            LineNumber = request.LineNumber,
            MemberName = request.MemberName,
            LogEventType = request.LogEventType,
            CreatedAt = DateTime.UtcNow
        });

    }
    
}