using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using LogSystem.Models;
using MySql.Data.MySqlClient;

namespace LogSystem.DataAccess.ADO.NET
{
    internal class LocationRepository : AdoNetBaseRepository<Location>
    {
        public override string TableName => "Location";

        public override void Add(Location entity)
        {

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {

                        command.CommandText = $"INSERT INTO location (FK_PointOfDelivery, FK_Address) VALUES({ entity.FK_PointOfDelivery},{ entity.FK_Address});";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public override void Delete(Location entity)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM location WHERE (Id={entity.Id});";
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void Update(Location entity)
        {
            throw new NotImplementedException();
        }

        public override List<Location> GetAll(string whereCondition, Dictionary<string, object> parameterValues)
        {
            throw new NotImplementedException();
        }

        public override List<Location> GetAll()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM musterag.location";
                    LogObjects.Clear();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LogObjects.Add(new Location
                            {
                                Id = reader.GetInt32(0),
                                FK_PointOfDelivery = reader.GetInt32(1),
                                FK_Address = reader.GetInt32(2),
                            });
                        }
                    }
                }
            }
            return LogObjects;
        }

        public override long Count(string whereCondition, Dictionary<string, object> parameterValues)
        {
            throw new NotImplementedException();
        }

    }
}
