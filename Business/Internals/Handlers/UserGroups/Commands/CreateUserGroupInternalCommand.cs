using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Internals.Handlers.UserGroups.Commands;

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
            var userGroupIsExist =
                await _userGroupRepository.AnyAsync(x => x.UserId == request.UserId && x.Status == true);
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