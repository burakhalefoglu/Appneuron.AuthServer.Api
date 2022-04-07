using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Translates.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Translates.Commands;

public class UpdateTranslateCommand : IRequest<IResult>
{
    public long Id { get; set; }
    public long LangId { get; set; }
    public string Value { get; set; }
    public string Code { get; set; }

    public class UpdateTranslateCommandHandler : IRequestHandler<UpdateTranslateCommand, IResult>
    {
        private readonly ITranslateRepository _translateRepository;

        public UpdateTranslateCommandHandler(ITranslateRepository translateRepository)
        {
            _translateRepository = translateRepository;
        }

        [SecuredOperation(Priority = 1)]
        [ValidationAspect(typeof(CreateTranslateValidator), Priority = 2)]
        [CacheRemoveAspect("Get")]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IResult> Handle(UpdateTranslateCommand request, CancellationToken cancellationToken)
        {
            var isThereTranslateRecord =
                await _translateRepository.GetAsync(u => u.Id == request.Id && u.Status == true);

            isThereTranslateRecord.Value = request.Value;
            isThereTranslateRecord.Code = request.Code;

            await _translateRepository.UpdateAsync(isThereTranslateRecord);
            return new SuccessResult(Messages.Updated);
        }
    }
}