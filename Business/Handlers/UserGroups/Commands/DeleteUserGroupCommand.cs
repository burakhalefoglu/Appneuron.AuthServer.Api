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

namespace Business.Handlers.UserGroups.Commands
{
    public class DeleteUserGroupCommand : IRequest<IResult>
    {
        public string Id { get; set; }

        public class DeleteUserGroupCommandHandler : IRequestHandler<DeleteUserGroupCommand, IResult>
        {
            private readonly IUserGroupRepository _userGroupRepository;

            public DeleteUserGroupCommandHandler(IUserGroupRepository userGroupRepository)
            {
                _userGroupRepository = userGroupRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(DeleteUserGroupCommand request, CancellationToken cancellationToken)
            {
                var entityToDelete = await _userGroupRepository.GetAsync(x => x.UserId == request.Id);
                entityToDelete.Status = false;
                await _userGroupRepository.UpdateAsync(entityToDelete, x => x.UserId == entityToDelete.UserId);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}