using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbGroupRepository : MongoDbRepositoryBase<Group>, IGroupRepository
    {
        public MDbGroupRepository() : base(Collections.Collections.Group)
        {
        }
    }
}