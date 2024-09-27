using Shared.Messages.Logs;

namespace MonitoringAPI.Services;

public interface IMessagePublisher
{
   Task PublishLogEventMessage(LogEventMessage dto);
}