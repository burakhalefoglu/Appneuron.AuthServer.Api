using System.Threading;
using System.Threading.Tasks;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Internals.Handlers.OperationClaims
{
    public class GetOperationClaimsByIdInternalQuery : IRequest<IDataResult<OperationClaim>>
    {
        public string Id { get; set; }

        public class GetOperationClaimsByIdInternalQueryHandler : IRequestHandler<GetOperationClaimsByIdInternalQuery,
            IDataResult<OperationClaim>>
        {
            private readonly IOperationClaimRepository _operationClaimRepository;

            public GetOperationClaimsByIdInternalQueryHandler(IOperationClaimRepository operationClaimRepository)
            {
                _operationClaimRepository = operationClaimRepository;
            }

            public async Task<IDataResult<OperationClaim>> Handle(GetOperationClaimsByIdInternalQuery request,
                CancellationToken cancellationToken)
            {
                return new SuccessDataResult<OperationClaim>(
                    await _operationClaimRepository.GetAsync(x => x.ObjectId == request.Id && x.Status == true));
            }
        }
    }
}