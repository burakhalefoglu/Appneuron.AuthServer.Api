using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Internals.Handlers.OperationClaims
{
    public class CreateOperationClaimInternalCommand : IRequest<IResult>
    {
        public string ClaimName { get; set; }

        public class
            CreateOperationClaimInternalCommandHandler : IRequestHandler<CreateOperationClaimInternalCommand, IResult>
        {
            private readonly IOperationClaimRepository _operationClaimRepository;

            public CreateOperationClaimInternalCommandHandler(IOperationClaimRepository operationClaimRepository)
            {
                _operationClaimRepository = operationClaimRepository;
            }

            public async Task<IResult> Handle(CreateOperationClaimInternalCommand request,
                CancellationToken cancellationToken)
            {
                if (await IsClaimExists(request.ClaimName))
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
                return await _operationClaimRepository.AnyAsync(x => x.Name == claimName);
            }
        }
    }
}