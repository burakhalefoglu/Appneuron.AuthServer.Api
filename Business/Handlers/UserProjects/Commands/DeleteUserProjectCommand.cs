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
        public string Id { get; set; }

        public class DeleteUserProjectCommandHandler : IRequestHandler<DeleteUserProjectCommand, IResult>
        {
            private readonly IUserProjectRepository _userProjectRepository;

            public DeleteUserProjectCommandHandler(IUserProjectRepository userProjectRepository)
            {
                _userProjectRepository = userProjectRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteUserProjectCommand request, CancellationToken cancellationToken)
            {
                var userProjectToDelete = await _userProjectRepository.GetAsync(p => p.ObjectId == request.Id);

                if (userProjectToDelete == null) return new ErrorResult(Messages.UserProjectNotFound);
                userProjectToDelete.Status = false;
                await _userProjectRepository.UpdateAsync(userProjectToDelete,
                    x => x.UserId == userProjectToDelete.UserId);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}