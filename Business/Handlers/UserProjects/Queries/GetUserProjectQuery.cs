
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;


namespace Business.Handlers.UserProjects.Queries
{
    public class GetUserProjectQuery : IRequest<IDataResult<UserProject>>
    {
        public long Id { get; set; }

        public class GetUserProjectQueryHandler : IRequestHandler<GetUserProjectQuery, IDataResult<UserProject>>
        {
            private readonly IUserProjectRepository _userProjectRepository;
            private readonly IMediator _mediator;

            public GetUserProjectQueryHandler(IUserProjectRepository userProjectRepository, IMediator mediator)
            {
                _userProjectRepository = userProjectRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<UserProject>> Handle(GetUserProjectQuery request, CancellationToken cancellationToken)
            {
                var userProject = await _userProjectRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<UserProject>(userProject);
            }
        }
    }
}
