using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Clients.Queries
{
    public class GetClientsQuery : IRequest<IDataResult<IEnumerable<Client>>>
    {
        public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, IDataResult<IEnumerable<Client>>>
        {
            private readonly IClientRepository _clientRepository;
            private readonly IMediator _mediator;

            public GetClientsQueryHandler(IClientRepository clientRepository, IMediator mediator)
            {
                _clientRepository = clientRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<Client>>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<Client>>(await _clientRepository.GetListAsync());
            }
        }
    }
}