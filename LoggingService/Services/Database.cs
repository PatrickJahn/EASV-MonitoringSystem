using System.Data;
using Microsoft.Data.SqlClient;
using Shared.Entities.Logs;

namespace LoggingService.Services
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string dbHost, string dbUser, string dbPass)
        {
            _connectionString = $"Server={dbHost};User Id={dbUser};Password={dbPass};Encrypt=false;";
            RecreateDatabaseAsync();
        }

        private async Task<SqlConnection> GetDbConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        private static async Task ExecuteAsync(SqlConnection connection, string sql)
        {
            try
            {
                await using var trans = (SqlTransaction)await connection.BeginTransactionAsync();

                await using var cmd = connection.CreateCommand();
                cmd.Transaction = trans;  
                cmd.CommandText = sql;
                await cmd.ExecuteNonQueryAsync();
        
                await trans.CommitAsync();  
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error executing SQL: {ex.Message}");
                throw;
            }
        }

        // Async method to recreate the database schema for testing or resetting
        public async Task RecreateDatabaseAsync()
        {
            await using var connection = await GetDbConnectionAsync();
            try
            {
                // Drop the table if it exists
                await ExecuteAsync(connection, "DROP TABLE IF EXISTS LogEvents");

                // Create the LogEvents table
                await ExecuteAsync(connection, @"
                    CREATE TABLE LogEvents (
                        Id UNIQUEIDENTIFIER PRIMARY KEY, 
                        LogEventType INT, 
                        Message NVARCHAR(500), 
                        MemberName NVARCHAR(500) NULL, 
                        FilePath NVARCHAR(500) NULL, 
                        LineNumber INT NULL, 
                        ErrorDetails NVARCHAR(500) NULL, 
                        CreatedAt DATETIME
                    )");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error recreating database: {ex.Message}");
                throw;
            }
        }

        // Method to insert a log event asynchronously
        public async Task InsertLogEventAsync(LogEvent logEvent)
        {
            await using var connection = await GetDbConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO LogEvents (Id, LogEventType, Message, MemberName, FilePath, LineNumber, ErrorDetails, CreatedAt) 
                VALUES (@Id, @LogEventType, @Message, @MemberName, @FilePath, @LineNumber, @ErrorDetails, @CreatedAt)";
            
            cmd.Parameters.Add(new SqlParameter("@Id", logEvent.Id));
            cmd.Parameters.Add(new SqlParameter("@LogEventType", logEvent.LogEventType));
            cmd.Parameters.Add(new SqlParameter("@Message", logEvent.Message));
            cmd.Parameters.Add(new SqlParameter("@MemberName", logEvent.MemberName ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@FilePath", logEvent.FilePath ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@LineNumber", logEvent.LineNumber.HasValue ? (object)logEvent.LineNumber : DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@ErrorDetails", logEvent.ErrorDetails ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@CreatedAt", logEvent.CreatedAt));

            await cmd.ExecuteNonQueryAsync();
    
        }

        // Async method to retrieve paginated log events
        public async Task<List<LogEvent>> GetLogEventsAsync(int page, int pageSize)
        {
            var logEvents = new List<LogEvent>();
            await using var connection = await GetDbConnectionAsync();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT * FROM LogEvents 
                ORDER BY CreatedAt DESC 
                OFFSET @Offset ROWS 
                FETCH NEXT @PageSize ROWS ONLY";

            var offset = (page - 1) * pageSize;

            cmd.Parameters.Add(new SqlParameter("@Offset", offset));
            cmd.Parameters.Add(new SqlParameter("@PageSize", pageSize));

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var logEvent = new LogEvent
                {
                    Id = reader.GetGuid(0),
                    LogEventType = (LogEventType)reader.GetInt32(1),
                    Message = reader.GetString(2),
                    MemberName = reader.IsDBNull(3) ? null : reader.GetString(3),
                    FilePath = reader.IsDBNull(4) ? null : reader.GetString(4),
                    LineNumber = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                    ErrorDetails = reader.IsDBNull(6) ? null : reader.GetString(6),
                    CreatedAt = reader.GetDateTime(7)
                };
                logEvents.Add(logEvent);
            }

            return logEvents;
        }
    }
}
