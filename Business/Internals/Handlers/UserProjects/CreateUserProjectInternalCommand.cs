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

namespace Business.Internals.Handlers.UserProjects
{
    public class CreateUserProjectInternalCommand : IRequest<IResult>
    {
        public string UserId { get; set; }
        public string ProjectKey { get; set; }

        public class
            CreateUserProjectInternalCommandHandler : IRequestHandler<CreateUserProjectInternalCommand, IResult>
        {
            private readonly IUserProjectRepository _userProjectRepository;

            public CreateUserProjectInternalCommandHandler(IUserProjectRepository userProjectRepository)
            {
                _userProjectRepository = userProjectRepository;
            }

            [ValidationAspect(typeof(CreateUserProjectValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(CreateUserProjectInternalCommand request,
                CancellationToken cancellationToken)
            {
                var isThereUserProjectRecord =
                    await _userProjectRepository.AnyAsync(u => u.ProjectKey == request.ProjectKey);

                if (isThereUserProjectRecord)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedUserProject = new UserProject
                {
                    UserId = request.UserId,
                    ProjectKey = request.ProjectKey
                };

                await _userProjectRepository.AddAsync(addedUserProject);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}