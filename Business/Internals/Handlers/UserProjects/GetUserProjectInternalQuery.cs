using System.Threading;
using System.Threading.Tasks;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Internals.Handlers.UserProjects
{
    public class GetUserProjectInternalQuery : IRequest<IDataResult<UserProject>>
    {
        public long ProjectKey { get; set; }
        public long UserId { get; set; }

        public class GetUserProjectInternalQueryHandler : IRequestHandler<GetUserProjectInternalQuery, IDataResult<UserProject>>
        {
            private readonly IUserProjectRepository _userProjectRepository;

            public GetUserProjectInternalQueryHandler(IUserProjectRepository userProjectRepository)
            {
                _userProjectRepository = userProjectRepository;
            }

            public async Task<IDataResult<UserProject>> Handle(GetUserProjectInternalQuery request,
                CancellationToken cancellationToken)
            {
                var userProjects = await _userProjectRepository.GetListAsync(p => p.UserId == request.UserId && p.Status == true);
                var userProject = userProjects.FirstOrDefault(p => p.ProjectId == request.ProjectKey);
                return new SuccessDataResult<UserProject>(userProject);
            }
        }

    }
}