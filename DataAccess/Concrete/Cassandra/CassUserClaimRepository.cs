using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;

namespace DataAccess.Concrete.Cassandra
{
    public class CassUserClaimRepository : CassandraRepositoryBase<UserClaim>, IUserClaimRepository
    {
        public CassUserClaimRepository() : base(MappingConfiguration.Global.Define<UserClaimMapper>())
        {
        }
    }
}