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
using Entities.Concrete;
using MediatR;

namespace Business.Internals.Handlers.UserGroups.Commands
{
    public class CreateUserGroupInternalCommand : IRequest<IResult>
    {
        public long GroupId { get; set; }
        public long UserId { get; set; }

        public class CreateUserGroupInternalCommandHandler : IRequestHandler<CreateUserGroupInternalCommand, IResult>
        {
            private readonly IUserGroupRepository _userGroupRepository;

            public CreateUserGroupInternalCommandHandler(IUserGroupRepository userGroupRepository)
            {
                _userGroupRepository = userGroupRepository;
            }
            
            public async Task<IResult> Handle(CreateUserGroupInternalCommand request, CancellationToken cancellationToken)
            {
                var userGroupIsExist = await _userGroupRepository.AnyAsync(x => x.UsersId == request.UserId && x.Status == true);
                if (userGroupIsExist)
                    return new ErrorResult(Messages.UserGroupNotFound);
                
                var userGroup = new UserGroup
                {
                    GroupId = request.GroupId,
                    UsersId = request.UserId
                };

                await _userGroupRepository.AddAsync(userGroup);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}