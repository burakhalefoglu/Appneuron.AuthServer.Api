using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class UserProjectRepository : EfEntityRepositoryBase<UserProject, ProjectDbContext>, IUserProjectRepository
    {
        public UserProjectRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}