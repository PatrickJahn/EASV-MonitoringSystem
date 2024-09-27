using EasyNetQ;
using Shared.Entities.Logs;
using Shared.Messages.Logs;
using Shared.Messages.Topics;

namespace LoggingService.Services;

public static class MessageSubscriber
{

    private static readonly IBus bus = RabbitHutch.CreateBus("host=rabbitmq;port=5672;username=user123;password=123456");
    private static readonly Database database = new Database();

    

    public static void StartSubscribing()
    {
        // Wait for rabitmq to start
        Thread.Sleep(6000);
        SubscribeToLogEvents();
    }

    private  static async void SubscribeToLogEvents()
    {
        
        await bus.PubSub.SubscribeAsync<LogEventMessage>("LogEvents", message =>
        {
            Console.WriteLine($"Received message: {message}");
            database.InsertLogEvent(new LogEvent()
            {
                Id = Guid.NewGuid(),
                Message = message.Message,
                LineNumber = message.LineNumber,
                FilePath = message.FilePath,
                MemberName = message.MemberName,
                LogEventType = message.LogEventType,
                ErrorDetails = message.ErrorDetails,
                CreatedAt = message.CreatedAt
            });
        }, x => x.WithTopic(ServiceBusTopic.LogEvent.ToString()));

       
    }
}