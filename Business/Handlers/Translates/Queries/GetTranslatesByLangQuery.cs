using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Translates.Queries
{
    public class GetTranslatesByLangQuery : IRequest<IDataResult<string>>
    {
        public string Lang { get; set; }

        public class GetTranslatesByLangQueryHandler : IRequestHandler<GetTranslatesByLangQuery, IDataResult<string>>
        {
            private readonly IMediator _mediator;
            private readonly ITranslateRepository _translateRepository;

            public GetTranslatesByLangQueryHandler(ITranslateRepository translateRepository, IMediator mediator)
            {
                _translateRepository = translateRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(LogstashLogger))]
            public async Task<IDataResult<string>> Handle(GetTranslatesByLangQuery request,
                CancellationToken cancellationToken)
            {
                return new SuccessDataResult<string>(
                    data: await _translateRepository.GetTranslatesByLang(request.Lang));
            }
        }
    }
}