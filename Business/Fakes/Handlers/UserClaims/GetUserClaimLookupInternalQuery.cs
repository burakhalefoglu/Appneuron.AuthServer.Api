﻿using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Fakes.Handlers.UserClaims
{
    public class GetUserClaimLookupInternalQuery : IRequest<IDataResult<IEnumerable<UserClaim>>>
    {
        public int UserId { get; set; }

        public class GetUserClaimQueryHandler : IRequestHandler<GetUserClaimLookupInternalQuery, IDataResult<IEnumerable<UserClaim>>>
        {
            private readonly IUserClaimRepository _userClaimRepository;

            public GetUserClaimQueryHandler(IUserClaimRepository userClaimRepository)
            {
                _userClaimRepository = userClaimRepository;
            }

            public async Task<IDataResult<IEnumerable<UserClaim>>> Handle(GetUserClaimLookupInternalQuery request, CancellationToken cancellationToken)
            {
                var userClaims = await _userClaimRepository.GetListAsync(x => x.UsersId == request.UserId);

                return new SuccessDataResult<IEnumerable<UserClaim>>(userClaims.ToList());
            }
        }
    }
}