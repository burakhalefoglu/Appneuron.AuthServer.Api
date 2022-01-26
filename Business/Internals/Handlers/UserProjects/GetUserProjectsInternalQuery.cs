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
        public string UserId { get; set; }

        public class GetUserProjectsInternalQueryHandler : IRequestHandler<GetUserProjectsInternalQuery,
            IDataResult<IEnumerable<UserProject>>>
        {
            private readonly IMediator _mediator;
            private readonly IUserProjectRepository _userProjectRepository;

            public GetUserProjectsInternalQueryHandler(IUserProjectRepository userProjectRepository, IMediator mediator)
            {
                _userProjectRepository = userProjectRepository;
                _mediator = mediator;
            }

            public async Task<IDataResult<IEnumerable<UserProject>>> Handle(GetUserProjectsInternalQuery request,
                CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<UserProject>>(
                    await _userProjectRepository.GetListAsync(p => p.UserId == request.UserId));
            }
        }
    }
}