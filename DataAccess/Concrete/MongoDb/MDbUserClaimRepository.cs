using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;


namespace DataAccess.Concrete.MongoDb
{
    public class MDbUserClaimRepository : MongoDbRepositoryBase<UserClaim>, IUserClaimRepository
    {
        public MDbUserClaimRepository() : base(Collections.Collections.UserClaim)
        {
        }
    }
}