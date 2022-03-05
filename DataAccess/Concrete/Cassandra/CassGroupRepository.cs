using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Tables;
using DataAccess.Concrete.Cassandra.Tables.TableMappers;

namespace DataAccess.Concrete.Cassandra
{
    public class CassGroupRepository : CassandraRepositoryBase<Group>, IGroupRepository
    {
        public CassGroupRepository() : base(CassandraTableQueries.Group,
            MappingConfiguration.Global.Define<GroupMapper>())
        {
        }
    }
}