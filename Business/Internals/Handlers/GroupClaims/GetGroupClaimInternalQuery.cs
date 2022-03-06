using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Internals.Handlers.OperationClaims;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using MediatR;

namespace Business.Internals.Handlers.GroupClaims
{
    public class GetGroupClaimInternalQuery : IRequest<IDataResult<IEnumerable<SelectionItem>>>
    {
        public long GroupId { get; set; }

        public class GetGroupClaimInternalQueryHandler : IRequestHandler<
            GetGroupClaimInternalQuery, IDataResult<IEnumerable<SelectionItem>>>
        {
            private readonly IGroupClaimRepository _groupClaimRepository;
            private readonly IMediator _mediator;

            public GetGroupClaimInternalQueryHandler(IGroupClaimRepository groupClaimRepository,
                IMediator mediator)
            {
                _mediator = mediator;
                _groupClaimRepository = groupClaimRepository;
            }

            public Task<IDataResult<IEnumerable<SelectionItem>>> Handle(
                GetGroupClaimInternalQuery request, CancellationToken cancellationToken)
            {
                var oClaims = new List<SelectionItem>();
                _groupClaimRepository.GetListAsync().WaitAsync(cancellationToken)
                    .Result.ToList().Where(x => x.GroupId == request.GroupId && x.Status)
                    .ToList().ForEach(Action);

                async void Action(GroupClaim x)
                {
                    var result = await _mediator.Send(new GetOperationClaimsByIdInternalQuery {Id = x.ClaimId},
                        cancellationToken);
                    if (result.Data is null)
                        return;
                    oClaims.Add(new SelectionItem
                    {
                        Id = result.Data.Id,
                        Label = result.Data.Name
                    });
                }

                return Task.FromResult<IDataResult<IEnumerable<SelectionItem>>>(
                    new SuccessDataResult<IEnumerable<SelectionItem>>(oClaims));
            }
        }
    }
}