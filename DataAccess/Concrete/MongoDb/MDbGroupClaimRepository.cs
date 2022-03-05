using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbGroupClaimRepository : MongoDbRepositoryBase<GroupClaim>, IGroupClaimRepository
    {
        public MDbGroupClaimRepository() : base(Collections.Collections.GroupClaim)
        {
        }
    }
}