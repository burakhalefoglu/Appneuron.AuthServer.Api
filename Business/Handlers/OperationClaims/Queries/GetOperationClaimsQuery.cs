using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.OperationClaims.Queries;

public class GetOperationClaimsQuery : IRequest<IDataResult<IEnumerable<OperationClaim>>>
{
    public class
        GetOperationClaimsQueryHandler : IRequestHandler<GetOperationClaimsQuery,
            IDataResult<IEnumerable<OperationClaim>>>
    {
        private readonly IOperationClaimRepository _operationClaimRepository;

        public GetOperationClaimsQueryHandler(IOperationClaimRepository operationClaimRepository)
        {
            _operationClaimRepository = operationClaimRepository;
        }

        [SecuredOperation(Priority = 1)]
        [CacheAspect(10)]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IDataResult<IEnumerable<OperationClaim>>> Handle(GetOperationClaimsQuery request,
            CancellationToken cancellationToken)
        {
            var ocs = await _operationClaimRepository.GetListAsync();
            var filterOCs = new List<OperationClaim>();
            if (ocs.Any()) filterOCs = ocs.Where(x => x.Status).ToList();
            return new SuccessDataResult<IEnumerable<OperationClaim>>(filterOCs);
        }
    }
}