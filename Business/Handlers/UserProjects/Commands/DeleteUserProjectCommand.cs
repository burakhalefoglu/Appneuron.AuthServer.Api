using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.UserProjects.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteUserProjectCommand : IRequest<IResult>
    {
        public long Id { get; set; }

        public class DeleteUserProjectCommandHandler : IRequestHandler<DeleteUserProjectCommand, IResult>
        {
            private readonly IMediator _mediator;
            private readonly IUserProjectRepository _userProjectRepository;

            public DeleteUserProjectCommandHandler(IUserProjectRepository userProjectRepository, IMediator mediator)
            {
                _userProjectRepository = userProjectRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteUserProjectCommand request, CancellationToken cancellationToken)
            {
                var userProjectToDelete = await _userProjectRepository.GetAsync(p => p.Id == request.Id);

                if (userProjectToDelete == null)
                {
                    return new ErrorResult(Messages.UserProjectNotFound);
                }

                _userProjectRepository.Delete(userProjectToDelete);
                await _userProjectRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}