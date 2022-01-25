using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Internals.Handlers.OperationClaims;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Internals.Handlers.GroupClaims
{
    public class GetGroupClaimsLookupByGroupIdInternalQuery : IRequest<IDataResult<IEnumerable<SelectionItem>>>
    {
        public int GroupId { get; set; }

        public class GetGroupClaimsLookupByGroupIdQueryHandler : IRequestHandler<
            GetGroupClaimsLookupByGroupIdInternalQuery, IDataResult<IEnumerable<SelectionItem>>>
        {
            private readonly IMediator _mediator;
            private readonly IGroupClaimRepository _groupClaimRepository;

            public GetGroupClaimsLookupByGroupIdQueryHandler(IGroupClaimRepository groupClaimRepository, IMediator mediator)
            {
                _mediator = mediator;
                _groupClaimRepository = groupClaimRepository;
            }

            public Task<IDataResult<IEnumerable<SelectionItem>>> Handle(
                GetGroupClaimsLookupByGroupIdInternalQuery request, CancellationToken cancellationToken)
            {
                var oClaims = new List<SelectionItem>();
                _groupClaimRepository.GetListAsync(x=> x.GroupId == request.GroupId)
                    .Result.ToList().ForEach(Action);
                
                async void Action(GroupClaim x)
                {
                    var result = await _mediator.Send(new GetOperationClaimsByIdInternalQuery() {Id = x.ClaimId}, cancellationToken);
                    if (result.Data is null)
                        return;
                    oClaims.Add(new SelectionItem()
                    {
                        Id = result.Data.ClaimId,
                        Label = result.Data.Name
                    });
                } 
                
                return Task.FromResult<IDataResult<IEnumerable<SelectionItem>>>(new SuccessDataResult<IEnumerable<SelectionItem>>(oClaims));
            }
        }
    }
}