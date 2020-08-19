using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericRepository;
using MySql.Data.MySqlClient;

namespace LogSystem.DataAccess.ADO.NET
{
    public abstract class RepositoryBase<M> : IRepositoryBase<M>
    { 
        protected string ConnectionString { get; }
        protected readonly List<M> list;
        protected RepositoryBase()
        {
            // TODO: Load ConnectionString
            ConnectionString = "server=localhost;database=musterag;uid=demo;password=JusKo1986;";
            list = new List<M>();
        }
       
        public M GetSingle<P>(P pkValue)
        {
            throw new NotImplementedException();
        }

        public void Add(M entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(M entity)
        {
            throw new NotImplementedException();
        }

        public void Update(M entity)
        {
            throw new NotImplementedException();
        }

        public List<M> GetAll(string whereCondition, Dictionary<string, object> parameterValues)
        {
            throw new NotImplementedException();
        }

        public abstract List<M> GetAll();

        public long Count(string whereCondition, Dictionary<string, object> parameterValues)
        {
            throw new NotImplementedException();
        }

        public long Count()
        {
            using (var conn = new MySqlConnection(this.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = $"select count(*) from {this.TableName}";
                    return (long)cmd.ExecuteScalar();
                }
            }
        }
        public abstract string TableName { get; }
        /* TODO: Implement Interface Members */
    }
}
