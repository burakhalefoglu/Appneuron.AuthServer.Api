using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbClientRepository : MongoDbRepositoryBase<Client>, IClientRepository
    {
        public MDbClientRepository() : base(Collections.Collections.Client)
        {
        }
    }
}