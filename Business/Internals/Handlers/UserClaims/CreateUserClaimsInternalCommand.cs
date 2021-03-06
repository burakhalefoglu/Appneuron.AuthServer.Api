using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Internals.Handlers.UserClaims;

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
                    _userClaimsRepository.GetListAsync(x =>
                        x.UsersId == request.UserId && x.Status == true).Result.Any(x => x.ClaimId == claim.Id);

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