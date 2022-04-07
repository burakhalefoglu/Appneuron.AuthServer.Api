using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra;

public class CassUserRepository : CassandraRepositoryBase<User>, IUserRepository
{
    public CassUserRepository() : base(MappingConfiguration.Global.Define<UserMapper>())
    {
    }
}