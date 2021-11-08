using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Fakes.Handlers.UserClaims
{
    /// <summary>
    ///     For Internal Use Only,
    ///     Registers All Existing Operation Claims To Given User
    /// </summary>
    public class CreateUserClaimsInternalCommand : IRequest<IResult>
    {
        public int UserId { get; set; }
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
                var OperationClaimList = request.OperationClaims;
                foreach (var claim in OperationClaimList)
                {
                    var result =
                        await _userClaimsRepository.GetCountAsync(x =>
                            x.UsersId == request.UserId && x.ClaimId == claim.Id);

                    if (result == 0)
                        _userClaimsRepository.Add(new UserClaim
                        {
                            ClaimId = claim.Id,
                            UsersId = request.UserId
                        });
                }

                await _userClaimsRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}