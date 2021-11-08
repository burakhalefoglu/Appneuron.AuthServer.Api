using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities.Dtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Fakes.Handlers.GroupClaims
{
    public class GetGroupClaimsLookupByGroupIdInternalQuery : IRequest<IDataResult<IEnumerable<SelectionItem>>>
    {
        public int GroupId { get; set; }

        public class GetGroupClaimsLookupByGroupIdQueryHandler : IRequestHandler<
            GetGroupClaimsLookupByGroupIdInternalQuery, IDataResult<IEnumerable<SelectionItem>>>
        {
            private readonly IGroupClaimRepository _groupClaimRepository;

            public GetGroupClaimsLookupByGroupIdQueryHandler(IGroupClaimRepository groupClaimRepository)
            {
                _groupClaimRepository = groupClaimRepository;
            }

            public async Task<IDataResult<IEnumerable<SelectionItem>>> Handle(
                GetGroupClaimsLookupByGroupIdInternalQuery request, CancellationToken cancellationToken)
            {
                var data = await _groupClaimRepository.GetGroupClaimsSelectedList(request.GroupId);
                return new SuccessDataResult<IEnumerable<SelectionItem>>(data);
            }
        }
    }
}