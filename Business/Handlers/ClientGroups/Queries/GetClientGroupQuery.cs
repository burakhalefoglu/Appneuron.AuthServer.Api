
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Core.Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;


namespace Business.Handlers.ClientGroups.Queries
{
    public class GetClientGroupQuery : IRequest<IDataResult<ClientGroup>>
    {
        public int GroupId { get; set; }

        public class GetClientGroupQueryHandler : IRequestHandler<GetClientGroupQuery, IDataResult<ClientGroup>>
        {
            private readonly IClientGroupRepository _clientGroupRepository;
            private readonly IMediator _mediator;

            public GetClientGroupQueryHandler(IClientGroupRepository clientGroupRepository, IMediator mediator)
            {
                _clientGroupRepository = clientGroupRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<ClientGroup>> Handle(GetClientGroupQuery request, CancellationToken cancellationToken)
            {
                var clientGroup = await _clientGroupRepository.GetAsync(p => p.GroupId == request.GroupId);
                return new SuccessDataResult<ClientGroup>(clientGroup);
            }
        }
    }
}
