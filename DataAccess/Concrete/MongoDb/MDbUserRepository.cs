using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbUserRepository : MongoDbRepositoryBase<User>, IUserRepository
    {
        public MDbUserRepository() : base(Collections.Collections.User)
        {
        }
    }
}