
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
namespace DataAccess.Concrete.EntityFramework
{
    public class UserProjectRepository : EfEntityRepositoryBase<UserProject, ProjectDbContext>, IUserProjectRepository
    {
        public UserProjectRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
