using EasyNetQ;
using Shared.Entities.Logs;
using Shared.Messages.Logs;
using Shared.Messages.Topics;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MonitoringAPI.Services
{
    public class LogServiceRequester
    {
        private readonly HttpClient client = new HttpClient();
        

        public async Task<List<LogEvent>?> RequestLogsAsync(int page, int pageSize)
        {
            var requestMessage = new LogRequestMessage
            {
                Page = page,
                PageSize = pageSize
            };
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://logging-service/LogEvents?page={page}&pageSize={pageSize}");
            var response = await client.SendAsync(request);
            
            var content = response.Content.ReadAsStringAsync().Result;
            
            return JsonSerializer.Deserialize<List<LogEvent>>(content);
            
        }

      
    }
}