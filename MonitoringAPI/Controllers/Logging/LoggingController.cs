using Microsoft.AspNetCore.Mvc;
using MonitoringAPI.Controllers.Logging.Requests;
using MonitoringAPI.Services;
using Shared.Messages.Logs;

namespace MonitoringAPI.Controllers.Logging;


[ApiController]
[Route("[controller]")]
public class LoggingController : ControllerBase
{
    private readonly LogServiceRequester _logServiceRequester;

    private readonly IMessagePublisher _messagePublisher;
    public LoggingController(IMessagePublisher messagePublisher, LogServiceRequester logServiceRequester)
    {
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        _logServiceRequester = logServiceRequester;

    }


    [HttpGet(Name = "GetLogs")]
    public async Task<IActionResult> GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var logs = await _logServiceRequester.RequestLogsAsync(page, pageSize);
     
        if (logs == null || !logs.Any())
        {
            return NotFound("No logs found.");
        }

        return Ok(logs);
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