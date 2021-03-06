using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Internals.Handlers.OperationClaims;

public class GetOperationClaimsByIdInternalQuery : IRequest<IDataResult<OperationClaim>>
{
    public long Id { get; set; }

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
                await _operationClaimRepository.GetAsync(x => x.Id == request.Id && x.Status == true));
        }
    }
}