using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ClientClaims.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class DeleteClientClaimCommand : IRequest<IResult>
    {
        public long ClientId { get; set; }

        public class DeleteClientClaimCommandHandler : IRequestHandler<DeleteClientClaimCommand, IResult>
        {
            private readonly IClientClaimRepository _clientClaimRepository;
            private readonly IMediator _mediator;

            public DeleteClientClaimCommandHandler(IClientClaimRepository clientClaimRepository, IMediator mediator)
            {
                _clientClaimRepository = clientClaimRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteClientClaimCommand request, CancellationToken cancellationToken)
            {
                var clientClaimToDelete = _clientClaimRepository.Get(p => p.ClientId == request.ClientId);

                _clientClaimRepository.Delete(clientClaimToDelete);
                await _clientClaimRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}