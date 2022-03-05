using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Tables;
using DataAccess.Concrete.Cassandra.Tables.TableMappers;

namespace DataAccess.Concrete.Cassandra
{
    public class CassGroupClaimRepository : CassandraRepositoryBase<GroupClaim>, IGroupClaimRepository
    {
        public CassGroupClaimRepository() : base(CassandraTableQueries.GroupClaim,
            MappingConfiguration.Global.Define<GroupClaimMapper>())
        {
        }
    }
}