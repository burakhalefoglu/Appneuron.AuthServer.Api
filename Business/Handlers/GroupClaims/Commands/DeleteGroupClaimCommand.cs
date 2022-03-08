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

namespace Business.Handlers.GroupClaims.Commands
{
    public class DeleteGroupClaimCommand : IRequest<IResult>
    {
        public long GroupId { get; set; }
        public long ClaimId { get; set; }

        public class DeleteGroupClaimCommandHandler : IRequestHandler<DeleteGroupClaimCommand, IResult>
        {
            private readonly IGroupClaimRepository _groupClaimRepository;

            public DeleteGroupClaimCommandHandler(IGroupClaimRepository groupClaimRepository)
            {
                _groupClaimRepository = groupClaimRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(DeleteGroupClaimCommand request, CancellationToken cancellationToken)
            {
                var groupClaimToDelete = await _groupClaimRepository
                    .GetAsync(x => x.GroupId == request.GroupId &&
                                   x.ClaimId == request.ClaimId &&
                                   x.Status == true);
                if (groupClaimToDelete == null) return new ErrorResult(Messages.GroupClaimNotFound);
                await _groupClaimRepository.DeleteAsync(groupClaimToDelete);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}