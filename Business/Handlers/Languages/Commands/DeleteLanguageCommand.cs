using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Languages.Commands
{
    public class DeleteLanguageCommand : IRequest<IResult>
    {
        public string Id { get; set; }

        public class DeleteLanguageCommandHandler : IRequestHandler<DeleteLanguageCommand, IResult>
        {
            private readonly ILanguageRepository _languageRepository;

            public DeleteLanguageCommandHandler(ILanguageRepository languageRepository)
            {
                _languageRepository = languageRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
            {
                var languageToDelete = await _languageRepository.GetAsync(p => p.ObjectId == request.Id && p.Status == true);
                languageToDelete.Status = false;
                await _languageRepository.UpdateAsync(languageToDelete,
                    x => x.ObjectId == languageToDelete.ObjectId);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}