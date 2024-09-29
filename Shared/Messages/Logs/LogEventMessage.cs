using Shared.Entities.Logs;

namespace Shared.Messages.Logs;

public class LogEventMessage
{
    public LogEventType LogEventType { get; set; }
    
    public string Message { get; set; }

    public string MemberName { get; set; }
    
    public string FilePath { get; set; }
    
    public int? LineNumber { get; set; }
    
    public Exception? ErrorDetails { get; set; }
    
    public DateTime CreatedAt { get; set; }
}