using EasyNetQ;
using Polly;
using Shared.Entities.Logs;
using Shared.Messages.Logs;
using Shared.Messages.Topics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoggingService.Services
{
    public static class MessageSubscriber
    {
        private static readonly IBus bus = RabbitHutch.CreateBus("host=rabbitmq;port=5672;username=user123;password=123456");
        private static ILogService _logService;

        public static void StartSubscribing(ILogService logService)
        {
            // Injecting LogService
            _logService = logService;

            // Wait for RabbitMQ to start
            Thread.Sleep(6000);

            // Subscribe to log events and log requests
            SubscribeToLogEvents();
        }

        private static async void SubscribeToLogEvents()
        {
            await bus.PubSub.SubscribeAsync<LogEventMessage>("LogEvents",
                async message =>
                {
                    // Retry the message handling logic 2 times
                    var retryPolicy = Policy
                        .Handle<Exception>()
                        .WaitAndRetryAsync(2, _ => TimeSpan.FromMinutes(1));

                    try
                    {
                        await retryPolicy.ExecuteAsync(async () =>
                        {
                            Console.WriteLine("Handling log event message...");
                            await _logService.InsertLogEventAsync(new LogEvent
                            {
                                Id = Guid.NewGuid(),
                                Message = message.Message,
                                LineNumber = message.LineNumber,
                                FilePath = message.FilePath,
                                MemberName = message.MemberName,
                                LogEventType = message.LogEventType,
                                ErrorDetails = message.ErrorDetails?.Message,
                                CreatedAt = message.CreatedAt
                            });
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Message processing failed after retries: {ex.Message}");
                    }
                }, x => x.WithTopic(ServiceBusTopic.LogEvent.ToString()));
        }




    }
}
