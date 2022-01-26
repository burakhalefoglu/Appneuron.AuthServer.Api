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

namespace Business.Handlers.GroupClaims.Commands
{
    public class CreateGroupClaimCommand : IRequest<IResult>
    {
        public string GroupId { get; set; }
        public string ClaimId { get; set; }
        public string ClaimName { get; set; }

        public class CreateGroupClaimCommandHandler : IRequestHandler<CreateGroupClaimCommand, IResult>
        {
            private readonly IGroupClaimRepository _groupClaimRepository;
            private readonly IOperationClaimRepository _operationClaimRepository;

            public CreateGroupClaimCommandHandler(IGroupClaimRepository groupClaimRepository,
                IOperationClaimRepository operationClaimRepository)
            {
                _groupClaimRepository = groupClaimRepository;
                _operationClaimRepository = operationClaimRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IResult> Handle(CreateGroupClaimCommand request, CancellationToken cancellationToken)
            {
                if (IsClaimNotExists(request.ClaimName).Result)
                    return new ErrorResult(Messages.OperationClaimNotFound);


                if (IsGroupClaimExist(request.ClaimId, request.GroupId).Result)
                    return new ErrorResult(Messages.GroupClaimExit);

                var groupClaim = new GroupClaim
                {
                    GroupId = request.GroupId,
                    ClaimId = request.ClaimId
                };
                await _groupClaimRepository.AddAsync(groupClaim);
                return new SuccessResult(Messages.Added);
            }

            private async Task<bool> IsClaimNotExists(string claimName)
            {
                return await _operationClaimRepository.GetAsync(x =>
                    x.Name == claimName) is null;
            }

            private async Task<bool> IsGroupClaimExist(string claimId, string groupId)
            {
                return !(await _groupClaimRepository.GetAsync(g => g.ClaimId == claimId
                                                                   && g.GroupId == groupId) is null);
            }
        }
    }
}