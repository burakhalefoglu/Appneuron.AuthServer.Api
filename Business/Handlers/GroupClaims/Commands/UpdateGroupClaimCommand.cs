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
    public class UpdateGroupClaimCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public long GroupId { get; set; }
        public long ClaimId { get; set; }
        public string ClaimName { get; set; }

        public class UpdateGroupClaimCommandHandler : IRequestHandler<UpdateGroupClaimCommand, IResult>
        {
            private readonly IGroupClaimRepository _groupClaimRepository;
            private readonly IOperationClaimRepository _operationClaimRepository;

            public UpdateGroupClaimCommandHandler(IGroupClaimRepository groupClaimRepository,
                IOperationClaimRepository operationClaimRepository)
            {
                _groupClaimRepository = groupClaimRepository;
                _operationClaimRepository = operationClaimRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(UpdateGroupClaimCommand request, CancellationToken cancellationToken)
            {
                if (IsClaimNotExists(request.ClaimName).Result)
                    return new ErrorResult(Messages.OperationClaimNotFound);

                if (IsGroupClaimNotExist(request.ClaimId, request.GroupId).Result)
                    return new ErrorResult(Messages.GroupClaimNotFound);

                var groupClaim = new GroupClaim {ClaimId = request.ClaimId, GroupId = request.GroupId};

                await _groupClaimRepository.AddAsync(groupClaim);
                return new SuccessResult(Messages.Updated);
            }

            private async Task<bool> IsClaimNotExists(string claimName)
            {
                return await _operationClaimRepository.GetAsync(x =>
                    x.Name == claimName && x.Status == true) is null ;
            }

            private async Task<bool> IsGroupClaimNotExist(long claimId, long groupId)
            {
                return await _groupClaimRepository.GetAsync(g => g.ClaimId == claimId
                                                                 && g.GroupId == groupId && g.Status == true) is null;
            }
        }
    }
}