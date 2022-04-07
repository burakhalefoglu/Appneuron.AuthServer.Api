using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Internals.Handlers.Users;

public class GetUserInternalQuery : IRequest<IDataResult<User>>
{
    public long Id { get; set; }

    public class GetUserInternalQueryHandler : IRequestHandler<GetUserInternalQuery, IDataResult<User>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserInternalQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IDataResult<User>> Handle(GetUserInternalQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(x => x.Id == request.Id && x.Status == true);
            return new SuccessDataResult<User>(user);
        }
    }
}