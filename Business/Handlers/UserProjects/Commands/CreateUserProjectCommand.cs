
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.UserProjects.ValidationRules;

namespace Business.Handlers.UserProjects.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateUserProjectCommand : IRequest<IResult>
    {

        public int UserId { get; set; }
        public string ProjectKey { get; set; }


        public class CreateUserProjectCommandHandler : IRequestHandler<CreateUserProjectCommand, IResult>
        {
            private readonly IUserProjectRepository _userProjectRepository;
            private readonly IMediator _mediator;
            public CreateUserProjectCommandHandler(IUserProjectRepository userProjectRepository, IMediator mediator)
            {
                _userProjectRepository = userProjectRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateUserProjectValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateUserProjectCommand request, CancellationToken cancellationToken)
            {
                var isThereUserProjectRecord = _userProjectRepository.Query().Any(u => u.UserId == request.UserId);

                if (isThereUserProjectRecord)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedUserProject = new UserProject
                {
                    UserId = request.UserId,
                    ProjectKey = request.ProjectKey,

                };

                _userProjectRepository.Add(addedUserProject);
                await _userProjectRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}