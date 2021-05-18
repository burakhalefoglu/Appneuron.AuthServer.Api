
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;

namespace Business.Fakes.Handlers.Clients.Queries
{
    public class GetClientInternalQuery : IRequest<IDataResult<Client>>
    {
        public string ClientId { get; set; }
        public string ProjectId { get; set; }


        public class GetClientQueryHandler : IRequestHandler<GetClientInternalQuery, IDataResult<Client>>
        {
            private readonly IClientRepository _clientRepository;
            private readonly IMediator _mediator;

            public GetClientQueryHandler(IClientRepository clientRepository, IMediator mediator)
            {
                _clientRepository = clientRepository;
                _mediator = mediator;
            }

            public async Task<IDataResult<Client>> Handle(GetClientInternalQuery request, CancellationToken cancellationToken)
            {
                var client = await _clientRepository.GetAsync(p => p.ClientId == request.ClientId && p.ProjectId == request.ProjectId);
                return new SuccessDataResult<Client>(client);
            }
        }
    }
}
