
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Core.Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;


namespace Business.Handlers.ClientClaims.Queries
{
    public class GetClientClaimQuery : IRequest<IDataResult<ClientClaim>>
    {
        public long ClientId { get; set; }

        public class GetClientClaimQueryHandler : IRequestHandler<GetClientClaimQuery, IDataResult<ClientClaim>>
        {
            private readonly IClientClaimRepository _clientClaimRepository;
            private readonly IMediator _mediator;

            public GetClientClaimQueryHandler(IClientClaimRepository clientClaimRepository, IMediator mediator)
            {
                _clientClaimRepository = clientClaimRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<ClientClaim>> Handle(GetClientClaimQuery request, CancellationToken cancellationToken)
            {
                var clientClaim = await _clientClaimRepository.GetAsync(p => p.ClientId == request.ClientId);
                return new SuccessDataResult<ClientClaim>(clientClaim);
            }
        }
    }
}
