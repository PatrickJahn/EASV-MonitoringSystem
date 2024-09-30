using Shared.Entities.Logs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoggingService.Services
{
    public class LogService : ILogService
    {
        private readonly Database _database;

        public LogService(Database database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        /// <summary>
        /// Inserts a log event into the database asynchronously.
        /// </summary>
        /// <param name="logEvent">The log event to be inserted.</param>
        public async Task InsertLogEventAsync(LogEvent logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent), "Log event cannot be null");
            }

            await _database.InsertLogEventAsync(logEvent);
        }

        /// <summary>
        /// Retrieves log events from the database with pagination.
        /// </summary>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of log events per page.</param>
        /// <returns>A list of log events.</returns>
        public async Task<List<LogEvent>> GetLogEventsAsync(int page, int pageSize)
        {
            if (page <= 0)
                throw new ArgumentException("Page number must be greater than zero.", nameof(page));

            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));

            // Retrieve logs from the database using pagination
            return await _database.GetLogEventsAsync(page, pageSize);
        }
    }
}