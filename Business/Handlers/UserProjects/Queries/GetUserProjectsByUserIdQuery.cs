
using Business.BusinessAspects;
using Core.Utilities.Results;
using Core.Aspects.Autofac.Performance;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Core.Aspects.Autofac.Logging;
using Microsoft.Extensions.DependencyInjection;
using Core.Aspects.Autofac.Caching;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Microsoft.AspNetCore.Http;
using Core.Utilities.IoC;

namespace Business.Handlers.UserProjects.Queries
{

    public class GetUserProjectsByUserIdQuery : IRequest<IDataResult<IEnumerable<UserProject>>>
    {
        public class GetAuthProjectModelsByUserIdQueryHandler : IRequestHandler<GetUserProjectsByUserIdQuery, IDataResult<IEnumerable<UserProject>>>
        {
            private readonly IUserProjectRepository _userProjectRepository;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetAuthProjectModelsByUserIdQueryHandler(IUserProjectRepository userProjectRepository, IMediator mediator)
            {
                _userProjectRepository = userProjectRepository;
                _mediator = mediator;
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();

            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<UserProject>>> Handle(GetUserProjectsByUserIdQuery request, CancellationToken cancellationToken)
            {
                var userId = int.Parse(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var result = await
                    _userProjectRepository.GetListAsync(p => p.UserId == userId);
                return new SuccessDataResult<IEnumerable<UserProject>>(result);
            }
        }
    }
}