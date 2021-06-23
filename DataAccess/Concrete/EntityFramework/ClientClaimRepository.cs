using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;

namespace DataAccess.Concrete.EntityFramework
{
    public class ClientClaimRepository : EfEntityRepositoryBase<ClientClaim, ProjectDbContext>, IClientClaimRepository
    {
        public ClientClaimRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}