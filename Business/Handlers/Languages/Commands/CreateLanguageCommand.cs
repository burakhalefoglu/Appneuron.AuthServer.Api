using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Languages.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Languages.Commands
{
    public class CreateLanguageCommand : IRequest<IResult>
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, IResult>
        {
            private readonly ILanguageRepository _languageRepository;

            public CreateLanguageCommandHandler(ILanguageRepository languageRepository)
            {
                _languageRepository = languageRepository;
            }

            [SecuredOperation(Priority = 1)]
            [ValidationAspect(typeof(CreateLanguageValidator), Priority = 2)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
            {
                var isThereLanguageRecord = await _languageRepository.AnyAsync(u => u.Name == request.Name && u.Status == true);

                if (isThereLanguageRecord)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedLanguage = new Language
                {
                    Name = request.Name,
                    Code = request.Code
                };
                await _languageRepository.AddAsync(addedLanguage);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}