using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Tables;
using DataAccess.Concrete.Cassandra.Tables.TableMappers;

namespace DataAccess.Concrete.Cassandra
{
    public class CassOperationClaimRepository : CassandraRepositoryBase<OperationClaim>, IOperationClaimRepository
    {
        public CassOperationClaimRepository() : base(CassandraTableQueries.OperationClaim,
            MappingConfiguration.Global.Define<OperationClaimMapper>())
        {
        }
    }
}
