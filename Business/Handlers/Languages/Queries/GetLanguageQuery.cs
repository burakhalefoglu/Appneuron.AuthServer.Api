using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Languages.Queries;

public class GetLanguageQuery : IRequest<IDataResult<Language>>
{
    public long Id { get; set; }

    public class GetLanguageQueryHandler : IRequestHandler<GetLanguageQuery, IDataResult<Language>>
    {
        private readonly ILanguageRepository _languageRepository;

        public GetLanguageQueryHandler(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;
        }

        [SecuredOperation(Priority = 1)]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IDataResult<Language>> Handle(GetLanguageQuery request,
            CancellationToken cancellationToken)
        {
            var language = await _languageRepository.GetAsync(p => p.Id == request.Id && p.Status == true);
            return new SuccessDataResult<Language>(language);
        }
    }
}