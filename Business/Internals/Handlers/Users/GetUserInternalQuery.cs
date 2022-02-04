﻿using System.Threading;
using System.Threading.Tasks;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Internals.Handlers.Users
{
    public class GetUserInternalQuery : IRequest<IDataResult<User>>
    {
        public string Id { get; set; }

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
                var user = await _userRepository.GetAsync(x=> x.ObjectId == request.Id && x.Status == true);
                return new SuccessDataResult<User>(user);
            }
        }
    }
}