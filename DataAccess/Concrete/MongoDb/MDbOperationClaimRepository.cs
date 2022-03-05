using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbOperationClaimRepository : MongoDbRepositoryBase<OperationClaim>, IOperationClaimRepository
    {
        public MDbOperationClaimRepository() : base(Collections.Collections.OperationClaim)
        {
        }
    }
}