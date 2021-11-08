using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.UserProjects.Queries
{
    public class GetUserProjectsQuery : IRequest<IDataResult<IEnumerable<UserProject>>>
    {
        public class
            GetUserProjectsQueryHandler : IRequestHandler<GetUserProjectsQuery, IDataResult<IEnumerable<UserProject>>>
        {
            private readonly IMediator _mediator;
            private readonly IUserProjectRepository _userProjectRepository;

            public GetUserProjectsQueryHandler(IUserProjectRepository userProjectRepository, IMediator mediator)
            {
                _userProjectRepository = userProjectRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<UserProject>>> Handle(GetUserProjectsQuery request,
                CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<UserProject>>(await _userProjectRepository.GetListAsync());
            }
        }
    }
}