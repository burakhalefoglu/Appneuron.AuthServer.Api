using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra
{
    public class CassUserProjectRepository : CassandraRepositoryBase<UserProject>, IUserProjectRepository
    {
        public CassUserProjectRepository(CassandraContexts cassandraContexts, string tableQuery) : base(
            cassandraContexts.CassandraConnectionSettings, tableQuery)
        {
        }
    }
}