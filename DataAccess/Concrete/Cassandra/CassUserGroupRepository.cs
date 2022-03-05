using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Tables;
using DataAccess.Concrete.Cassandra.Tables.TableMappers;

namespace DataAccess.Concrete.Cassandra
{
    public class CassUserGroupRepository : CassandraRepositoryBase<UserGroup>, IUserGroupRepository
    {
        public CassUserGroupRepository() : base(CassandraTableQueries.UserGroup,
            MappingConfiguration.Global.Define<UserGroupMapper>())
        {
        }
    }
}