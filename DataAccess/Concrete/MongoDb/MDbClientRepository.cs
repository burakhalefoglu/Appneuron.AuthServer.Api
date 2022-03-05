using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbClientRepository : MongoDbRepositoryBase<Client>, IClientRepository
    {
        public MDbClientRepository() : base(Collections.Collections.Client)
        {
        }
    }
}