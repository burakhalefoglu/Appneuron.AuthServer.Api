using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Languages.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Internals.Handlers.Languages
{
    /// <summary>
    /// </summary>
    public class CreateLanguageInternalCommand : IRequest<IResult>
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public class CreateLanguageInternalCommandHandler : IRequestHandler<CreateLanguageInternalCommand, IResult>
        {
            private readonly ILanguageRepository _languageRepository;

            public CreateLanguageInternalCommandHandler(ILanguageRepository languageRepository)
            {
                _languageRepository = languageRepository;
            }

            [ValidationAspect(typeof(CreateLanguageValidator), Priority = 2)]
            [CacheRemoveAspect("Get")]
            public async Task<IResult> Handle(CreateLanguageInternalCommand request,
                CancellationToken cancellationToken)
            {
                var isThereLanguageRecord = await _languageRepository.AnyAsync(u => u.Name == request.Name);

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