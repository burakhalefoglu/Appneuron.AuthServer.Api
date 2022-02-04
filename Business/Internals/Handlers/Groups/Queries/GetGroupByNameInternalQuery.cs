using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Internals.Handlers.Groups.Queries
{
    public class GetGroupByNameInternalQuery : IRequest<IDataResult<Group>>
    {
        public string GroupName { get; set; }

        public class GetGroupByNameInternalQueryHandler : IRequestHandler<GetGroupByNameInternalQuery, IDataResult<Group>>
        {
            private readonly IGroupRepository _groupRepository;

            public GetGroupByNameInternalQueryHandler(IGroupRepository groupRepository)
            {
                _groupRepository = groupRepository; 
            }
            public async Task<IDataResult<Group>> Handle(GetGroupByNameInternalQuery request, CancellationToken cancellationToken)
            {
                var group = await _groupRepository
                    .GetAsync(x => x.GroupName == request.GroupName && x.Status == true);
                return new SuccessDataResult<Group>(group);
            }
        }
    }
}