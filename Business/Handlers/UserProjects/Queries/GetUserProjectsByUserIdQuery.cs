using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.UserProjects.Queries
{
    public class GetUserProjectsByUserIdQuery : IRequest<IDataResult<IEnumerable<UserProject>>>
    {
        public class GetUserProjectsByUserIdQueryHandler : IRequestHandler<GetUserProjectsByUserIdQuery,
            IDataResult<IEnumerable<UserProject>>>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMediator _mediator;
            private readonly IUserProjectRepository _userProjectRepository;

            public GetUserProjectsByUserIdQueryHandler(IUserProjectRepository userProjectRepository,
                IMediator mediator, IHttpContextAccessor httpContextAccessor)
            {
                _userProjectRepository = userProjectRepository;
                _mediator = mediator;
                _httpContextAccessor = httpContextAccessor;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(LogstashLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<UserProject>>> Handle(GetUserProjectsByUserIdQuery request,
                CancellationToken cancellationToken)
            {
                var userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var result = await
                    _userProjectRepository.GetListAsync(p => p.UserId == userId);
                return new SuccessDataResult<IEnumerable<UserProject>>(result);
            }
        }
    }
}