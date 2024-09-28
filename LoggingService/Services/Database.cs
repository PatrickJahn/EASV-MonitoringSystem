using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Shared.Entities.Logs;

namespace LoggingService.Services;

public class Database
{
    
    
    private SqlConnection? _connection;

    private DbConnection GetDbConnection()
    {
        if (_connection != null) 
            return _connection;
            
        var  connection = new SqlConnection($"Server=log-db;User Id=sa;Password=SuperSecret7!;Encrypt=false;");
        connection.Open();

        _connection = connection;
        return connection;
    }


        // Execute method with proper exception handling and transaction management
        private void Execute(IDbConnection connection, string sql)
        {
            try
            {
                using var trans = connection.BeginTransaction();
                var cmd = connection.CreateCommand();
                cmd.Transaction = trans;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                trans.Commit();
            }
            catch (SqlException ex)
            {
                throw;
            }
           
        }

     

        // Method to recreate the database (recreate tables)
        public void RecreateDatabase()
        {
            var connection = GetDbConnection();

            try
            {
                Execute(connection, "DROP TABLE IF EXISTS LogEvents");

                Execute(connection, "CREATE TABLE LogEvents(id uniqueidentifier PRIMARY KEY, logEventType integer, message varchar(500), memberName varchar(500) null, filePath varchar(500) null, lineNumber integer null, errorDetalis varchar(500) null, createdAt DATE)");
            }
            catch (Exception ex)
            {
                throw;
            }
        
        }
        
        // Method to insert documents
        public async Task InsertLogEvent(LogEvent logEvent)
        {

                var connection = GetDbConnection();
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = "INSERT INTO LogEvents(id, logEventType, message, memberName, filePath, lineNumber, errorDetalis, createdAt) VALUES(@id,@logEventType,@message,@memberName,@filePath,@lineNumber,@errorDetails,@createdAt)";

                var pId = new SqlParameter("id", logEvent.Id);
                var pType = new SqlParameter("logEventType", logEvent.LogEventType);
                var pMessage = new SqlParameter("message", logEvent.Message);
                var pMemberName = new SqlParameter("memberName", logEvent.MemberName ?? "");
                var pFilePath = new SqlParameter("filePath", logEvent.FilePath ?? "");
                var pLineNumber = new SqlParameter("lineNumber", logEvent.LineNumber);
                var pError = new SqlParameter("errorDetails", logEvent.ErrorDetails ?? "");
                var pCreatedAt = new SqlParameter("createdAt", logEvent.CreatedAt);

                insertCmd.Parameters.Add(pId);
                insertCmd.Parameters.Add(pType);
                insertCmd.Parameters.Add(pMessage);
                insertCmd.Parameters.Add(pMemberName);
                insertCmd.Parameters.Add(pFilePath);
                insertCmd.Parameters.Add(pLineNumber);
                insertCmd.Parameters.Add(pError);
                insertCmd.Parameters.Add(pCreatedAt);

                await insertCmd.ExecuteNonQueryAsync();
            
        }

        public List<LogEvent> GetLogEvents()
        {
            var connection = GetDbConnection();
            var sql = @"SELECT * FROM LogEvents";
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = sql;
            List<LogEvent> logEvents = new List<LogEvent>();
            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetGuid(0);
                    var logEventType = (LogEventType) reader.GetInt16(1);
                    var message = reader.GetString(2);
                    var memberName = reader.GetString(3);
                    var filePath = reader.GetString(4);
                    var lineNumber = reader.GetInt32(5);
                    var errorDetails = reader.GetString(6);
                    var createdAt = reader.GetDateTime(7);

                    logEvents.Add(new LogEvent()
                    {
                        Id = id,
                        LogEventType = logEventType,
                        Message = message,
                        MemberName = memberName,
                        FilePath = filePath,
                        LineNumber = lineNumber,
                        ErrorDetails = errorDetails,
                        CreatedAt = createdAt
                    });
                }
            }
            return logEvents;
        }

}