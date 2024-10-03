using LoggingService.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Entities.Logs;

namespace LoggingService.Controllers;

[ApiController]
[Route("[controller]")]
public class LogEventsController : ControllerBase
{
    private readonly ILogService _logService;

    public LogEventsController(ILogService logService)
    {
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
    }
    
    
    [HttpGet(Name = "GetLogs")]
    public async Task<List<LogEvent>> GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
     return await _logService.GetLogEventsAsync(page, pageSize);
     
    }

    
}