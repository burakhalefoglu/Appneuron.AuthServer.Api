using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra
{
    public class CassOperationClaimRepository : CassandraRepositoryBase<OperationClaim>, IOperationClaimRepository
    {
        public CassOperationClaimRepository() : base(MappingConfiguration.Global.Define<OperationClaimMapper>())
        {
        }
    }
}
