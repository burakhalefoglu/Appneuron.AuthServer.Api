using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbLanguageRepository : MongoDbRepositoryBase<Language>, ILanguageRepository
    {
        public MDbLanguageRepository() : base(Collections.Collections.Language)
        {
        }
    }
}