using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MySql.Data.MySqlClient;

namespace LogSystem.DataAccess.ADO.NET
{
    internal class LogEntryRepository : AdoNetBaseRepository<LoggingEntry>
    {
 public LogEntryRepository() { }

        public override void Add(LoggingEntry entity)
        {
            using (var connect = new SqlConnection(ConnectionString))
            {
                connect.Open();
                using (var command = connect.CreateCommand())
                {
                    command.CommandText = "LogMessageAdd";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pod", entity.Id);
                    command.Parameters.AddWithValue("@hostname", entity.Hostname);
                    command.Parameters.AddWithValue("@level", entity.Severity);
                    command.Parameters.AddWithValue("@message", entity.Message);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void Delete(LoggingEntry entity)
        {
            using (var connect = new SqlConnection(ConnectionString))
            {
                connect.Open();
                using (var command = connect.CreateCommand())
                {
                    command.CommandText = "LogClear";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", entity.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void Update(LoggingEntry entity)
        {
            throw new NotImplementedException();
        }

        public override List<LoggingEntry> GetAll(string whereCondition, Dictionary<string, object> parameterValues)
        {
            throw new NotImplementedException();
        }
        public override List<LoggingEntry> GetAll()
        {
            using (var connect = new SqlConnection(ConnectionString))
            {
                connect.Open();

                IDbCommand command = connect.CreateCommand();
                command.CommandText = "SELECT * FROM musterag.v_logentries;";
                IDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {            
                    LogObjects.Add(new LoggingEntry
                    {
                        Id = (int) dataReader["Id"],
                        Hostname = dataReader["hostname"].ToString(),
                        Location = dataReader["location"].ToString(),
                        Message = dataReader["message"].ToString(),
                        Pod =  dataReader["pod"].ToString(),
                        Severity = (int) dataReader["severity"],
                        Timestamp = (DateTime) dataReader["timestamp"]
                    });
                }
            }
            return LogObjects.ToList();
        }

        public override long Count(string whereCondition, Dictionary<string, object> parameterValues)
        {
            throw new NotImplementedException();
        }
    }
}
