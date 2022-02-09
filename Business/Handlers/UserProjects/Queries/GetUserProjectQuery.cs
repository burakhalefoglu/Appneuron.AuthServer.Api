using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.UserProjects.Queries
{
    public class GetUserProjectQuery : IRequest<IDataResult<UserProject>>
    {
        public long Id { get; set; }

        public class GetUserProjectQueryHandler : IRequestHandler<GetUserProjectQuery, IDataResult<UserProject>>
        {
            private readonly IUserProjectRepository _userProjectRepository;

            public GetUserProjectQueryHandler(IUserProjectRepository userProjectRepository)
            {
                _userProjectRepository = userProjectRepository;
            }

            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<UserProject>> Handle(GetUserProjectQuery request,
                CancellationToken cancellationToken)
            {
                var userProject = await _userProjectRepository.GetAsync(p => p.Id == request.Id && p.Status == true);
                return new SuccessDataResult<UserProject>(userProject);
            }
        }
    }
}