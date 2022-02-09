using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Users.Queries;
using Business.Internals.Handlers.Users;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.UserClaims.Commands
{
    public class UpdateUserClaimCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long[] ClaimId { get; set; }

        public class UpdateUserClaimCommandHandler : IRequestHandler<UpdateUserClaimCommand, IResult>
        {
            private readonly IUserClaimRepository _userClaimRepository;
            private readonly IMediator _mediator;
            public UpdateUserClaimCommandHandler(IUserClaimRepository userClaimRepository, IMediator mediator)
            {
                _userClaimRepository = userClaimRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(UpdateUserClaimCommand request, CancellationToken cancellationToken)
            {
                var user = await _mediator.Send(new GetUserInternalQuery()
                {
                    Id = request.UserId
                }, cancellationToken);
                if (user.Data is null)
                    return new ErrorResult(Messages.UserNotFound);
                var userList = request.ClaimId.Select(x => new UserClaim {ClaimId = x, UsersId = request.UserId});
                foreach (var userClaim in userList)
                {
                    await _userClaimRepository.AddAsync(userClaim);

                }
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}