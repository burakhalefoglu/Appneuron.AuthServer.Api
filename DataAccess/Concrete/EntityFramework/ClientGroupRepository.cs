using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;

namespace DataAccess.Concrete.EntityFramework
{
    public class ClientGroupRepository : EfEntityRepositoryBase<ClientGroup, ProjectDbContext>, IClientGroupRepository
    {
        public ClientGroupRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}