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
        public string Id { get; set; }

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
                var groupClaimToDelete = await _groupClaimRepository.GetAsync(x => x.GroupId == request.Id);
                if (groupClaimToDelete == null) return new ErrorResult(Messages.GroupClaimNotFound);
                groupClaimToDelete.Status = false;
                await _groupClaimRepository.UpdateAsync(groupClaimToDelete,
                    x => x.GroupId == groupClaimToDelete.GroupId);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}