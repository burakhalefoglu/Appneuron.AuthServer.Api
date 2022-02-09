using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Internals.Handlers.UserProjects
{
    public class GetUserProjectsInternalQuery : IRequest<IDataResult<IEnumerable<UserProject>>>
    {
        public long UserId { get; set; }

        public class GetUserProjectsInternalQueryHandler : IRequestHandler<GetUserProjectsInternalQuery,
            IDataResult<IEnumerable<UserProject>>>
        {
            private readonly IUserProjectRepository _userProjectRepository;

            public GetUserProjectsInternalQueryHandler(IUserProjectRepository userProjectRepository)
            {
                _userProjectRepository = userProjectRepository;
            }

            public async Task<IDataResult<IEnumerable<UserProject>>> Handle(GetUserProjectsInternalQuery request,
                CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<UserProject>>(
                    await _userProjectRepository.GetListAsync(p => p.UserId == request.UserId && p.Status == true));
            }
        }
    }
}