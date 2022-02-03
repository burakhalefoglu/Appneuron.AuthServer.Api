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

namespace Business.Handlers.Translates.Commands
{
    public class DeleteTranslateCommand : IRequest<IResult>
    {
        public string Id { get; set; }

        public class DeleteTranslateCommandHandler : IRequestHandler<DeleteTranslateCommand, IResult>
        {
            private readonly IMediator _mediator;
            private readonly ITranslateRepository _translateRepository;

            public DeleteTranslateCommandHandler(ITranslateRepository translateRepository, IMediator mediator)
            {
                _translateRepository = translateRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(DeleteTranslateCommand request, CancellationToken cancellationToken)
            {
                var translateToDelete = await _translateRepository.GetAsync(p => p.ObjectId == request.Id);
                translateToDelete.Status = false;
                await _translateRepository.UpdateAsync(translateToDelete,
                    x => x.ObjectId == translateToDelete.ObjectId);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}