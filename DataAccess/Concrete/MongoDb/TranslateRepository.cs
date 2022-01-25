using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DataAccess.MongoDb.Concrete;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using DataAccess.Abstract;
using DataAccess.Concrete.MongoDb.Context;
using Newtonsoft.Json;

namespace DataAccess.Concrete.MongoDb
{
    public class TranslateRepository : MongoDbRepositoryBase<Translate>, ITranslateRepository
    {
        public TranslateRepository(MongoDbContextBase mongoDbContext, string collectionName) : base(mongoDbContext.MongoConnectionSettings, collectionName)
        {
        }
    }
}