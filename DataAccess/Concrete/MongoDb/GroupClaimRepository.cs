using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DataAccess.MongoDb.Concrete;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using DataAccess.Abstract;
using DataAccess.Concrete.MongoDb.Context;

namespace DataAccess.Concrete.MongoDb
{
    public class GroupClaimRepository : MongoDbRepositoryBase<GroupClaim>, IGroupClaimRepository
    {
        public GroupClaimRepository(MongoDbContextBase mongoDbContext, string collectionName) : base(mongoDbContext.MongoConnectionSettings, collectionName)
        {
        }
    }
}