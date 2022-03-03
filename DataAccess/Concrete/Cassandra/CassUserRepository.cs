using Core.DataAccess.Cassandra;
using Core.DataAccess.MongoDb.Concrete;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Contexts;
using DataAccess.Concrete.Cassandra.Tables;
using DataAccess.Concrete.MongoDb.Context;

namespace DataAccess.Concrete.Cassandra
{
    public class CassUserRepository : CassandraRepositoryBase<User>, IUserRepository
    {
        public CassUserRepository(CassandraContextBase cassandraContexts) : base(
            cassandraContexts.CassandraConnectionSettings, CassandraTableQueries.User)
        {
        }
    }
}