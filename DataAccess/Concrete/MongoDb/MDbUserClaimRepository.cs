using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;


namespace DataAccess.Concrete.MongoDb
{
    public class MDbUserClaimRepository : MongoDbRepositoryBase<UserClaim>, IUserClaimRepository
    {
        public MDbUserClaimRepository() : base(Collections.Collections.UserClaim)
        {
        }
    }
}