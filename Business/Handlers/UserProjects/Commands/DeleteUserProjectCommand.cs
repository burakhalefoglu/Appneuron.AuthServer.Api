using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.UserProjects.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class DeleteUserProjectCommand : IRequest<IResult>
    {
        public long Id { get; set; }

        public class DeleteUserProjectCommandHandler : IRequestHandler<DeleteUserProjectCommand, IResult>
        {
            private readonly IUserProjectRepository _userProjectRepository;
            private readonly IMediator _mediator;

            public DeleteUserProjectCommandHandler(IUserProjectRepository userProjectRepository, IMediator mediator)
            {
                _userProjectRepository = userProjectRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteUserProjectCommand request, CancellationToken cancellationToken)
            {
                var userProjectToDelete = _userProjectRepository.Get(p => p.Id == request.Id);

                _userProjectRepository.Delete(userProjectToDelete);
                await _userProjectRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}