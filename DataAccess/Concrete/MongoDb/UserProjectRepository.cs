using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DataAccess.MongoDb.Concrete;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using DataAccess.Abstract;
using DataAccess.Concrete.MongoDb.Context;
using Entities.Concrete;
using Newtonsoft.Json;

namespace DataAccess.Concrete.MongoDb
{
    public class UserProjectRepository : MongoDbRepositoryBase<UserProject>, IUserProjectRepository
    {
        public UserProjectRepository(MongoDbContextBase mongoDbContext, string collectionName) : base(mongoDbContext.MongoConnectionSettings, collectionName)
        {
        }
    }
}