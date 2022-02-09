using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Translates.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Translates.Commands
{
    /// <summary>
    /// </summary>
    public class CreateTranslateCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }

        public class CreateTranslateCommandHandler : IRequestHandler<CreateTranslateCommand, IResult>
        {
            private readonly ITranslateRepository _translateRepository;

            public CreateTranslateCommandHandler(ITranslateRepository translateRepository)
            {
                _translateRepository = translateRepository;
            }

            [SecuredOperation(Priority = 1)]
            [ValidationAspect(typeof(CreateTranslateValidator), Priority = 2)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(CreateTranslateCommand request, CancellationToken cancellationToken)
            {
                var isThereTranslateRecord = await _translateRepository
                    .AnyAsync(u => u.Id == request.Id && u.Code == request.Code && u.Status == true);

                if (isThereTranslateRecord)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedTranslate = new Translate
                {
                    Value = request.Value,
                    Code = request.Code
                };

                await _translateRepository.AddAsync(addedTranslate);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}