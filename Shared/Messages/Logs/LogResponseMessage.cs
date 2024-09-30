using Shared.Entities.Logs;

namespace Shared.Messages.Logs;

public class LogResponseMessage
{
    public List<LogEvent> LogEvents { get; set; }
}