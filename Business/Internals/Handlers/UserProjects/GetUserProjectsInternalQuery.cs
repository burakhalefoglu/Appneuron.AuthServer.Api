using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.UserProjects.Queries
{
    public class GetUserProjectsInternalQuery : IRequest<IDataResult<IEnumerable<UserProject>>>
    {
        public long UserId { get; set; }

        public class GetUserProjectsInternalQueryHandler : IRequestHandler<GetUserProjectsInternalQuery, IDataResult<IEnumerable<UserProject>>>
        {
            private readonly IUserProjectRepository _userProjectRepository;
            private readonly IMediator _mediator;

            public GetUserProjectsInternalQueryHandler(IUserProjectRepository userProjectRepository, IMediator mediator)
            {
                _userProjectRepository = userProjectRepository;
                _mediator = mediator;
            }

            public async Task<IDataResult<IEnumerable<UserProject>>> Handle(GetUserProjectsInternalQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<UserProject>>(await _userProjectRepository.GetListAsync(p => p.UserId == request.UserId));
            }
        }
    }
}