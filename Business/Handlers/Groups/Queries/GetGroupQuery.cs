using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Groups.Queries;

public class GetGroupQuery : IRequest<IDataResult<Group>>
{
    public long GroupId { get; set; }

    public class GetGroupQueryHandler : IRequestHandler<GetGroupQuery, IDataResult<Group>>
    {
        private readonly IGroupRepository _groupRepository;

        public GetGroupQueryHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        [SecuredOperation(Priority = 1)]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IDataResult<Group>> Handle(GetGroupQuery request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository
                .GetAsync(x => x.Id == request.GroupId && x.Status == true);
            return new SuccessDataResult<Group>(group);
        }
    }
}