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
                var userProject = await _userProjectRepository.GetAsync(p => p.ProjectId == request.ProjectKey && p.Status == true);
                return new SuccessDataResult<UserProject>(userProject);
            }
        }
    }
}