using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbLanguageRepository : MongoDbRepositoryBase<Language>, ILanguageRepository
    {
        public MDbLanguageRepository() : base(Collections.Collections.Language)
        {
        }
    }
}