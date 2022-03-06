using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbOperationClaimRepository : MongoDbRepositoryBase<OperationClaim>, IOperationClaimRepository
    {
        public MDbOperationClaimRepository() : base(Collections.Collections.OperationClaim)
        {
        }
    }
}