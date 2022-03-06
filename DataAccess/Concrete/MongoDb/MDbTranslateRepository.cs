using Core.DataAccess.MongoDb;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace DataAccess.Concrete.MongoDb
{
    public class MDbTranslateRepository : MongoDbRepositoryBase<Translate>, ITranslateRepository
    {
        public MDbTranslateRepository() : base(Collections.Collections.Translate)
        {
        }
    }
}