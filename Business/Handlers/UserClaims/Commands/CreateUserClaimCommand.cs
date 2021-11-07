using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.UserClaims.Commands
{
    public class CreateUserClaimCommand : IRequest<IResult>
    {
        public int UserId { get; set; }
        public int ClaimId { get; set; }

        public class CreateUserClaimCommandHandler : IRequestHandler<CreateUserClaimCommand, IResult>
        {
            private readonly IUserClaimRepository _userClaimRepository;

            public CreateUserClaimCommandHandler(IUserClaimRepository userClaimRepository)
            {
                _userClaimRepository = userClaimRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(CreateUserClaimCommand request, CancellationToken cancellationToken)
            {
                var userClaim = new UserClaim
                {
                    ClaimId = request.ClaimId,
                    UsersId = request.UserId
                };
                _userClaimRepository.Add(userClaim);
                await _userClaimRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}