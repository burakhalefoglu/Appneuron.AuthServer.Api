using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.ClientClaims.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ClientClaims.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class CreateClientClaimCommand : IRequest<IResult>
    {
        public int ClaimId { get; set; }

        public class CreateClientClaimCommandHandler : IRequestHandler<CreateClientClaimCommand, IResult>
        {
            private readonly IClientClaimRepository _clientClaimRepository;
            private readonly IMediator _mediator;

            public CreateClientClaimCommandHandler(IClientClaimRepository clientClaimRepository, IMediator mediator)
            {
                _clientClaimRepository = clientClaimRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateClientClaimValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateClientClaimCommand request, CancellationToken cancellationToken)
            {
                var isThereClientClaimRecord = _clientClaimRepository.Query().Any(u => u.ClaimId == request.ClaimId);

                if (isThereClientClaimRecord)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedClientClaim = new ClientClaim
                {
                    ClaimId = request.ClaimId,
                };

                _clientClaimRepository.Add(addedClientClaim);
                await _clientClaimRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}