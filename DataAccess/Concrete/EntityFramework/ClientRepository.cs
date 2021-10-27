
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
namespace DataAccess.Concrete.EntityFramework
{
    public class ClientRepository : EfEntityRepositoryBase<Client, ProjectDbContext>, IClientRepository
    {
        public ClientRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}

