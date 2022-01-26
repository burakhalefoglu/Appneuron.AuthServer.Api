using System.Threading;
using System.Threading.Tasks;
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

namespace Business.Handlers.Translates.Commands
{
    public class UpdateTranslateCommand : IRequest<IResult>
    {
        public string Id { get; set; }
        public string LangId { get; set; }
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
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IResult> Handle(UpdateTranslateCommand request, CancellationToken cancellationToken)
            {
                var isThereTranslateRecord = await _translateRepository.GetAsync(u => u.ObjectId == request.Id);

                isThereTranslateRecord.Value = request.Value;
                isThereTranslateRecord.Code = request.Code;

                await _translateRepository.UpdateAsync(isThereTranslateRecord,
                    x => x.ObjectId == isThereTranslateRecord.ObjectId);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}