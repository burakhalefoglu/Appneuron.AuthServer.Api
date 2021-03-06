using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.UserClaims.Commands;

public class DeleteUserClaimCommand : IRequest<IResult>
{
    public long Id { get; set; }

    public class DeleteUserClaimCommandHandler : IRequestHandler<DeleteUserClaimCommand, IResult>
    {
        private readonly IUserClaimRepository _userClaimRepository;

        public DeleteUserClaimCommandHandler(IUserClaimRepository userClaimRepository)
        {
            _userClaimRepository = userClaimRepository;
        }

        [SecuredOperation(Priority = 1)]
        [CacheRemoveAspect("Get")]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IResult> Handle(DeleteUserClaimCommand request, CancellationToken cancellationToken)
        {
            var entityToDelete = await _userClaimRepository.GetAsync(x => x.UsersId == request.Id && x.Status == true);
            if (entityToDelete == null) return new ErrorResult(Messages.UserClaimNotFound);
            await _userClaimRepository.DeleteAsync(entityToDelete);
            return new SuccessResult(Messages.Deleted);
        }
    }
}