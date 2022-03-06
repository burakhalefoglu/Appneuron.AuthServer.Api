using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbGroupClaimRepository : MongoDbRepositoryBase<GroupClaim>, IGroupClaimRepository
    {
        public MDbGroupClaimRepository() : base(Collections.Collections.GroupClaim)
        {
        }
    }
}