using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.OperationClaims.Commands
{
    public class UpdateOperationClaimCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }

        public class UpdateOperationClaimCommandHandler : IRequestHandler<UpdateOperationClaimCommand, IResult>
        {
            private readonly IOperationClaimRepository _operationClaimRepository;

            public UpdateOperationClaimCommandHandler(IOperationClaimRepository operationClaimRepository)
            {
                _operationClaimRepository = operationClaimRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(UpdateOperationClaimCommand request, CancellationToken cancellationToken)
            {
                var isOperationClaimsExits = await _operationClaimRepository.GetAsync(u => u.Id == request.Id && u.Status == true);
                if (isOperationClaimsExits == null) return new ErrorResult(Messages.OperationClaimNotFound);

                isOperationClaimsExits.Alias = request.Alias;
                isOperationClaimsExits.Description = request.Description;
                await _operationClaimRepository.UpdateAsync(isOperationClaimsExits);

                return new SuccessResult(Messages.Updated);
            }
        }
    }
}