using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Translates.Queries;

public class GetTranslateQuery : IRequest<IDataResult<Translate>>
{
    public long Id { get; set; }

    public class GetTranslateQueryHandler : IRequestHandler<GetTranslateQuery, IDataResult<Translate>>
    {
        private readonly IMediator _mediator;
        private readonly ITranslateRepository _translateRepository;

        public GetTranslateQueryHandler(ITranslateRepository translateRepository, IMediator mediator)
        {
            _translateRepository = translateRepository;
            _mediator = mediator;
        }

        [SecuredOperation(Priority = 1)]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IDataResult<Translate>> Handle(GetTranslateQuery request,
            CancellationToken cancellationToken)
        {
            var translate = await _translateRepository.GetAsync(p => p.Id == request.Id && p.Status == true);
            return new SuccessDataResult<Translate>(translate);
        }
    }
}