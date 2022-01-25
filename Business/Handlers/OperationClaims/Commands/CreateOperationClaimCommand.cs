using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.OperationClaims.Commands
{
    public class CreateOperationClaimCommand : IRequest<IResult>
    {
        public string ClaimName { get; set; }

        public class CreateOperationClaimCommandHandler : IRequestHandler<CreateOperationClaimCommand, IResult>
        {
            private readonly IOperationClaimRepository _operationClaimRepository;

            public CreateOperationClaimCommandHandler(IOperationClaimRepository operationClaimRepository)
            {
                _operationClaimRepository = operationClaimRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IResult> Handle(CreateOperationClaimCommand request, CancellationToken cancellationToken)
            {
                if (!await IsClaimExists(request.ClaimName))
                    return new ErrorResult(Messages.OperationClaimExists);

                var operationClaim = new OperationClaim
                {
                    Name = request.ClaimName
                };
                await _operationClaimRepository.AddAsync(operationClaim);
                return new SuccessResult(Messages.Added);
            }

            private async Task<bool> IsClaimExists(string claimName)
            {
                return await _operationClaimRepository.GetAsync(x
                    => x.Name == claimName) is null;
            }
        }
    }
}