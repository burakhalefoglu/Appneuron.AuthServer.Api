using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.UserProjects.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Fakes.Handlers.UserProjects
{
    public class CreateUserProjectInternalCommand : IRequest<IResult>
    {
        public int UserId { get; set; }
        public string ProjectKey { get; set; }

        public class
            CreateUserProjectInternalCommandHandler : IRequestHandler<CreateUserProjectInternalCommand, IResult>
        {
            private readonly IMediator _mediator;
            private readonly IUserProjectRepository _userProjectRepository;

            public CreateUserProjectInternalCommandHandler(IUserProjectRepository userProjectRepository,
                IMediator mediator)
            {
                _userProjectRepository = userProjectRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateUserProjectValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(CreateUserProjectInternalCommand request,
                CancellationToken cancellationToken)
            {
                var isThereUserProjectRecord =
                    _userProjectRepository.Query().Any(u => u.ProjectKey == request.ProjectKey);

                if (isThereUserProjectRecord)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedUserProject = new UserProject
                {
                    UserId = request.UserId,
                    ProjectKey = request.ProjectKey
                };

                _userProjectRepository.Add(addedUserProject);
                await _userProjectRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}