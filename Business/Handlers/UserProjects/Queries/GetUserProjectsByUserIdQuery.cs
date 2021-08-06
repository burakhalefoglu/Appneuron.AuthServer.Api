
using Business.BusinessAspects;
using Core.Utilities.Results;
using Core.Aspects.Autofac.Performance;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Caching;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;

namespace Business.Handlers.UserProjects.Queries
{

    public class GetUserProjectsByUserIdQuery : IRequest<IDataResult<IEnumerable<UserProject>>>
    {
        public long UserId { get; set; }

        public class GetAuthProjectModelsByUserIdQueryHandler : IRequestHandler<GetUserProjectsByUserIdQuery, IDataResult<IEnumerable<UserProject>>>
        {
            private readonly IUserProjectRepository _userProjectRepository;
            private readonly IMediator _mediator;

            public GetAuthProjectModelsByUserIdQueryHandler(IUserProjectRepository userProjectRepository, IMediator mediator)
            {
                _userProjectRepository = userProjectRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<UserProject>>> Handle(GetUserProjectsByUserIdQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<UserProject>>(await _userProjectRepository.GetListAsync(p => p.UserId == request.UserId));
            }
        }
    }
}