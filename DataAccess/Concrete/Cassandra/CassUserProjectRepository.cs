using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Contexts;
using DataAccess.Concrete.Cassandra.Tables;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra
{
    public class CassUserProjectRepository : CassandraRepositoryBase<UserProject>, IUserProjectRepository
    {
        public CassUserProjectRepository(CassandraContextBase cassandraContexts) : base(
            cassandraContexts.CassandraConnectionSettings, CassandraTableQueries.UserProject)
        {
        }
    }
}