using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbTranslateRepository : MongoDbRepositoryBase<Translate>, ITranslateRepository
    {
        public MDbTranslateRepository() : base(Collections.Collections.Translate)
        {
        }
    }
}