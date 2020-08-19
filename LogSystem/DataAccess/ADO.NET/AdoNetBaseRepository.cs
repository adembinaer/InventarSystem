using System;
using System.Collections.Generic;
using GenericRepository;

namespace LogSystem.DataAccess.ADO.NET
{
   public abstract class AdoNetBaseRepository<M> : IRepositoryBase<M>
    {
        protected string ConnectionString { get; }
        protected readonly List<M> LogObjects;

        protected AdoNetBaseRepository()
        {
            //ConnectionString = "server=localhost;database=musterag;uid=demo;password=JusKo1986;";
            ConnectionString = @"Data Source = BPMNFOREVER\ZBWBINAER; Initial Catalog=musterag; Integrated Security =SSPI;";
            LogObjects = new List<M>(); 
        }
        public M GetSingle<P>(P pkValue)
        {
            throw new NotImplementedException();
        }

        public abstract void Add(M entity);

        public abstract void Delete(M entity);
        
        public abstract void Update(M entity);

        public abstract List<M> GetAll(string whereCondition, Dictionary<string, object> parameterValues);

        public abstract List<M> GetAll();

        public abstract long Count(string whereCondition, Dictionary<string, object> parameterValues);

        public long Count()
        {
            throw new NotImplementedException();
        }

        public virtual string TableName { get; }
    }
}
