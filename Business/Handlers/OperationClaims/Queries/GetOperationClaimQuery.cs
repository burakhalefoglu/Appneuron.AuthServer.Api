using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.OperationClaims.Queries
{
    public class GetOperationClaimQuery : IRequest<IDataResult<OperationClaim>>
    {
        public string Id { get; set; }

        public class
            GetOperationClaimQueryHandler : IRequestHandler<GetOperationClaimQuery, IDataResult<OperationClaim>>
        {
            private readonly IOperationClaimRepository _operationClaimRepository;

            public GetOperationClaimQueryHandler(IOperationClaimRepository operationClaimRepository)
            {
                _operationClaimRepository = operationClaimRepository;
            }

            [SecuredOperation(Priority = 1)]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IDataResult<OperationClaim>> Handle(GetOperationClaimQuery request,
                CancellationToken cancellationToken)
            {
                return new SuccessDataResult<OperationClaim>(await _operationClaimRepository
                    .GetAsync(x => x.ObjectId == request.Id));
            }
        }
    }
}