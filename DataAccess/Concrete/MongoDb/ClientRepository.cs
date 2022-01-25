using Core.DataAccess.MongoDb.Concrete;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.MongoDb.Context;

namespace DataAccess.Concrete.MongoDb
{
    public class ClientRepository : MongoDbRepositoryBase<Client>, IClientRepository
    {
        public ClientRepository(MongoDbContextBase mongoDbContext, string collectionName) : base(mongoDbContext.MongoConnectionSettings, collectionName)
        {
        }
    }
}