using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
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

namespace Business.Handlers.UserProjects.Commands
{
    /// <summary>
    /// </summary>
    public class CreateUserProjectCommand : IRequest<IResult>
    {
        public string UserId { get; set; }
        public string ProjectKey { get; set; }

        public class CreateUserProjectCommandHandler : IRequestHandler<CreateUserProjectCommand, IResult>
        {
            private readonly IUserProjectRepository _userProjectRepository;

            public CreateUserProjectCommandHandler(IUserProjectRepository userProjectRepository)
            {
                _userProjectRepository = userProjectRepository;
            }

            [ValidationAspect(typeof(CreateUserProjectValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateUserProjectCommand request, CancellationToken cancellationToken)
            {
                var isThereUserProjectRecord = await _userProjectRepository
                    .GetAsync(u => u.ObjectId == request.UserId);

                if (isThereUserProjectRecord != null)
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