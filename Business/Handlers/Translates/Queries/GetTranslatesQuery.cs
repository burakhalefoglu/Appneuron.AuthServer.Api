using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Translates.Queries;

public class GetTranslatesQuery : IRequest<IDataResult<IEnumerable<Translate>>>
{
    public class
        GetTranslatesQueryHandler : IRequestHandler<GetTranslatesQuery, IDataResult<IEnumerable<Translate>>>
    {
        private readonly IMediator _mediator;
        private readonly ITranslateRepository _translateRepository;

        public GetTranslatesQueryHandler(ITranslateRepository translateRepository, IMediator mediator)
        {
            _translateRepository = translateRepository;
            _mediator = mediator;
        }

        [SecuredOperation(Priority = 1)]
        [PerformanceAspect(5)]
        [CacheAspect(10)]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IDataResult<IEnumerable<Translate>>> Handle(GetTranslatesQuery request,
            CancellationToken cancellationToken)
        {
            return new SuccessDataResult<IEnumerable<Translate>>(_translateRepository
                .GetListAsync().Result.Where(x => x.Status == true));
        }
    }
}