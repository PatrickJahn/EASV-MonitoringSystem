using Shared.Entities.Logs;

namespace LoggingService.Services
{
    public interface ILogService
    {
        Task<List<LogEvent>> GetLogEventsAsync(int page, int pageSize);
        Task InsertLogEventAsync(LogEvent logEvent);
    }
}