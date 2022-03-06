using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Internals.Handlers.UserClaims
{
    /// <summary>
    ///     For Internal Use Only,
    ///     Registers All Existing Operation Claims To Given User
    /// </summary>
    public class CreateUserClaimsInternalCommand : IRequest<IResult>
    {
        public long UserId { get; set; }
        public List<OperationClaim> OperationClaims { get; set; }

        public class CreateUserClaimsInternalCommandHandler : IRequestHandler<CreateUserClaimsInternalCommand, IResult>
        {
            private readonly IUserClaimRepository _userClaimsRepository;

            public CreateUserClaimsInternalCommandHandler(IUserClaimRepository userClaimsRepository)
            {
                _userClaimsRepository = userClaimsRepository;
            }

            public async Task<IResult> Handle(CreateUserClaimsInternalCommand request,
                CancellationToken cancellationToken)
            {
                var operationClaims = request.OperationClaims;
                foreach (var claim in operationClaims)
                {
                    var result =
                        await _userClaimsRepository.AnyAsync(x =>
                            x.UsersId == request.UserId && x.ClaimId == claim.Id && x.Status == true);

                    if (!result)
                        await _userClaimsRepository.AddAsync(new UserClaim
                        {
                            ClaimId = claim.Id,
                            UsersId = request.UserId
                        });
                }

                return new SuccessResult(Messages.Added);
            }
        }
    }
}