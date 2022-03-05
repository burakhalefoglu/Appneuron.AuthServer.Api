using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Tables;
using DataAccess.Concrete.Cassandra.Tables.TableMappers;

namespace DataAccess.Concrete.Cassandra
{
    public class CassUserRepository : CassandraRepositoryBase<User>, IUserRepository
    {
        public CassUserRepository() : base(CassandraTableQueries.User,
            MappingConfiguration.Global.Define<UserMapper>())
        {
        }
    }
}