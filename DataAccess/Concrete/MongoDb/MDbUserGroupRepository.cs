using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbUserGroupRepository : MongoDbRepositoryBase<UserGroup>, IUserGroupRepository
    {
        public MDbUserGroupRepository() : base(Collections.Collections.UserGroup)
        {
        }
    }
}