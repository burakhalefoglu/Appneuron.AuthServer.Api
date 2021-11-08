using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;

namespace DataAccess.Concrete.EntityFramework
{
    public class ClientRepository : EfEntityRepositoryBase<Client, ProjectDbContext>, IClientRepository
    {
        public ClientRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}