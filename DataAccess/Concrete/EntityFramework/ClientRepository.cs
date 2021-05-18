
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Core.Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class ClientRepository : EfEntityRepositoryBase<Client, ProjectDbContext>, IClientRepository
    {
        public ClientRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
