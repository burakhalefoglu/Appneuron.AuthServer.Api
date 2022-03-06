using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbUserGroupRepository : MongoDbRepositoryBase<UserGroup>, IUserGroupRepository
    {
        public MDbUserGroupRepository() : base(Collections.Collections.UserGroup)
        {
        }
    }
}