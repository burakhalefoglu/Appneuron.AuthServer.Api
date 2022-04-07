using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra;

public class CassUserClaimRepository : CassandraRepositoryBase<UserClaim>, IUserClaimRepository
{
    public CassUserClaimRepository() : base(MappingConfiguration.Global.Define<UserClaimMapper>())
    {
    }
}