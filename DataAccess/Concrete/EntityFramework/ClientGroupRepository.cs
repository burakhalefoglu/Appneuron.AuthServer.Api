
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
namespace DataAccess.Concrete.EntityFramework
{
    public class ClientGroupRepository : EfEntityRepositoryBase<ClientGroup, ProjectDbContext>, IClientGroupRepository
    {
        public ClientGroupRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
