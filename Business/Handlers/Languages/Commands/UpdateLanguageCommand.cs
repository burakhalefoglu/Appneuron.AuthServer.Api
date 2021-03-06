using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Languages.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Languages.Commands;

public class UpdateLanguageCommand : IRequest<IResult>
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }

    public class UpdateLanguageCommandHandler : IRequestHandler<UpdateLanguageCommand, IResult>
    {
        private readonly ILanguageRepository _languageRepository;

        public UpdateLanguageCommandHandler(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;
        }

        [SecuredOperation(Priority = 1)]
        [ValidationAspect(typeof(CreateLanguageValidator), Priority = 2)]
        [CacheRemoveAspect("Get")]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IResult> Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
        {
            var isThereLanguageRecord = await _languageRepository.GetAsync(u => u.Id == request.Id && u.Status == true);

            isThereLanguageRecord.Name = request.Name;
            isThereLanguageRecord.Code = request.Code;
            await _languageRepository.UpdateAsync(isThereLanguageRecord);
            return new SuccessResult(Messages.Updated);
        }
    }
}