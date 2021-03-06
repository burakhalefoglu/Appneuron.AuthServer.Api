using Core.DataAccess.MongoDb;
using DataAccess.Abstract;
using Entities.Concrete;

namespace DataAccess.Concrete.MongoDb;

public class MDbUserRepository : MongoDbRepositoryBase<User>, IUserRepository
{
    public MDbUserRepository() : base(Collections.Collections.User)
    {
    }
}