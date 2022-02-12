using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Contexts;

namespace DataAccess.Concrete.Cassandra
{
    public class CassClientRepository : CassandraRepositoryBase<Client>, IClientRepository
    {
        public CassClientRepository(CassandraContextBase cassandraContexts, string tableQuery) : base(
            cassandraContexts.CassandraConnectionSettings, tableQuery)
        {
        }
    }
}