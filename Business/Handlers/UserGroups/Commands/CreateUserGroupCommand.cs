using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.UserGroups.Commands
{
    public class CreateUserGroupCommand : IRequest<IResult>
    {
        public string GroupId { get; set; }
        public string UserId { get; set; }

        public class CreateUserGroupCommandHandler : IRequestHandler<CreateUserGroupCommand, IResult>
        {
            private readonly IUserGroupRepository _userGroupRepository;

            public CreateUserGroupCommandHandler(IUserGroupRepository userGroupRepository)
            {
                _userGroupRepository = userGroupRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(CreateUserGroupCommand request, CancellationToken cancellationToken)
            {
                var userGroupIsExist = await _userGroupRepository.AnyAsync(x => x.UserId == request.UserId && x.Status == true);
                if (userGroupIsExist)
                    return new ErrorResult(Messages.UserGroupNotFound);
                var userGroup = new UserGroup
                {
                    GroupId = request.GroupId,
                    UserId = request.UserId
                };

                await _userGroupRepository.AddAsync(userGroup);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}