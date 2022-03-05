using Core.DataAccess.MongoDb;
using DataAccess.Abstract;
using Entities.Concrete;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbUserProjectRepository : MongoDbRepositoryBase<UserProject>, IUserProjectRepository
    {
        public MDbUserProjectRepository() : base(Collections.Collections.UserProject)
        {
        }
    }
}