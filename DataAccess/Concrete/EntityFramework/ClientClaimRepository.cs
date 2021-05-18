
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
namespace DataAccess.Concrete.EntityFramework
{
    public class ClientClaimRepository : EfEntityRepositoryBase<ClientClaim, ProjectDbContext>, IClientClaimRepository
    {
        public ClientClaimRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
