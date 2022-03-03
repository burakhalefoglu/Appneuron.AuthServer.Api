using Core.DataAccess.Cassandra;
using Core.DataAccess.Cassandra.Configurations;
using Core.DataAccess.MongoDb.Concrete;
using Core.Entities.Concrete;
using Core.Utilities.IoC;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Contexts;
using DataAccess.Concrete.Cassandra.Tables;
using DataAccess.Concrete.MongoDb.Context;

namespace DataAccess.Concrete.Cassandra
{
    public class CassUserRepository : CassandraRepositoryBase<User>, IUserRepository
    {
        // connection string
        public CassUserRepository() : base(CassandraTableQueries.User)
        {
        }
    }
}