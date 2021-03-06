using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.GroupClaims.Queries;

public class GetGroupClaimQuery : IRequest<IDataResult<GroupClaim>>
{
    public long GroupId { get; set; }

    public class GetGroupClaimQueryHandler : IRequestHandler<GetGroupClaimQuery, IDataResult<GroupClaim>>
    {
        private readonly IGroupClaimRepository _groupClaimRepository;

        public GetGroupClaimQueryHandler(IGroupClaimRepository groupClaimRepository)
        {
            _groupClaimRepository = groupClaimRepository;
        }

        [SecuredOperation(Priority = 1)]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IDataResult<GroupClaim>> Handle(GetGroupClaimQuery request,
            CancellationToken cancellationToken)
        {
            return new SuccessDataResult<GroupClaim>(
                await _groupClaimRepository.GetAsync(x => x.GroupId == request.GroupId &&
                                                          x.Status == true));
        }
    }
}