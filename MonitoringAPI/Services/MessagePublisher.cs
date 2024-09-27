using EasyNetQ;
using Shared.Messages.Logs;
using Shared.Messages.Topics;

namespace MonitoringAPI.Services;

public class MessagePublisher : IMessagePublisher
{
    
    public async Task PublishLogEventMessage(LogEventMessage dto)
    {
        
        try
        {
            Console.WriteLine("SENDING LOG EVENT MESSAGE");
            using var bus = RabbitHutch.CreateBus("host=rabbitmq;port=5672;username=user123;password=123456");
            await bus.PubSub.PublishAsync(dto, ServiceBusTopic.LogEvent.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

   
}