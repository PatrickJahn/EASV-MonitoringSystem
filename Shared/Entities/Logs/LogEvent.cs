using System.Text.Json.Serialization;

namespace Shared.Entities.Logs;

public class LogEvent
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("logEventType")]
    public LogEventType LogEventType { get; set; }
    
    
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("memberName")]
    public string MemberName { get; set; }
    
    [JsonPropertyName("filePath")]
    public string FilePath { get; set; }
    
    [JsonPropertyName("lineNumber")]
    public int? LineNumber { get; set; }

    [JsonPropertyName("errorDetails")]
    public string? ErrorDetails { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    

}