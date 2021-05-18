﻿
using Business.BusinessAspects;
using Core.Aspects.Autofac.Performance;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Core.Entities.Concrete;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Aspects.Autofac.Caching;

namespace Business.Handlers.ClientGroups.Queries
{

    public class GetClientGroupsQuery : IRequest<IDataResult<IEnumerable<ClientGroup>>>
    {
        public class GetClientGroupsQueryHandler : IRequestHandler<GetClientGroupsQuery, IDataResult<IEnumerable<ClientGroup>>>
        {
            private readonly IClientGroupRepository _clientGroupRepository;
            private readonly IMediator _mediator;

            public GetClientGroupsQueryHandler(IClientGroupRepository clientGroupRepository, IMediator mediator)
            {
                _clientGroupRepository = clientGroupRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<ClientGroup>>> Handle(GetClientGroupsQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<ClientGroup>>(await _clientGroupRepository.GetListAsync());
            }
        }
    }
}