namespace Shared.Entities.Logs;

public class LogEvent
{
    public Guid Id { get; set; }
    
    public LogEventType LogEventType { get; set; }
    
    public string Message { get; set; }
    
    public string MemberName { get; set; }
    
    public string FilePath { get; set; }
    
    public int? LineNumber { get; set; }

    public string? ErrorDetails { get; set; }
    
    public DateTime CreatedAt { get; set; }
    

}