using Core.DataAccess.MongoDb.Concrete;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.MongoDb.Context;

namespace DataAccess.Concrete.MongoDb
{
    public class LanguageRepository : MongoDbRepositoryBase<Language>, ILanguageRepository
    {
        public LanguageRepository(MongoDbContextBase mongoDbContext, string collectionName) : base(
            mongoDbContext.MongoConnectionSettings, collectionName)
        {
        }
    }
}