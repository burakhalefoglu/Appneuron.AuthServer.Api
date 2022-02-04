using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Internals.Handlers.Users;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.UserGroups.Commands
{
    public class UpdateUserGroupCommand : IRequest<IResult>
    {
        public string UserId { get; set; }
        public string[] GroupId { get; set; }

        public class UpdateUserGroupCommandHandler : IRequestHandler<UpdateUserGroupCommand, IResult>
        {
            private readonly IUserGroupRepository _userGroupRepository;
            private readonly IMediator _mediator;
            public UpdateUserGroupCommandHandler(IUserGroupRepository userGroupRepository, IMediator mediator)
            {
                _userGroupRepository = userGroupRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(UpdateUserGroupCommand request, CancellationToken cancellationToken)
            {
                var user = await _mediator.Send(new GetUserInternalQuery()
                {
                    Id = request.UserId
                }, cancellationToken);
                if (user.Data is null)
                    return new ErrorResult(Messages.UserNotFound);
                
                var userGroupList = request.GroupId.Select(x => new UserGroup {GroupId = x, UserId = request.UserId});
                await _userGroupRepository.AddManyAsync(userGroupList);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}