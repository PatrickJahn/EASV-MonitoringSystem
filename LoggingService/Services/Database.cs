using System.Data;
using System.Data.Common;
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

                Execute(connection, "CREATE TABLE LogEvents(id INTEGER PRIMARY KEY, url VARCHAR(500))");
            }
            catch (Exception ex)
            {
                throw;
            }
        
        }

        // Method to insert documents
        public void InsertLogEvent(LogEvent logEvent)
        {
            try
            {
                var connection = GetDbConnection();
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = "INSERT INTO LogEvents(id, url) VALUES(@id,@url)";

              //  var pUrl = new SqlParameter("url", url);
              //  insertCmd.Parameters.Add(pId);

                insertCmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw;
            }
           
        }
}