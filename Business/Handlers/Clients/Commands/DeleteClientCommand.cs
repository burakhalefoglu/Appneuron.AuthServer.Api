using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Clients.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class DeleteClientCommand : IRequest<IResult>
    {
        public long Id { get; set; }

        public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, IResult>
        {
            private readonly IClientRepository _clientRepository;
            private readonly IMediator _mediator;

            public DeleteClientCommandHandler(IClientRepository clientRepository, IMediator mediator)
            {
                _clientRepository = clientRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
            {
                var clientToDelete = _clientRepository.Get(p => p.Id == request.Id);

                _clientRepository.Delete(clientToDelete);
                await _clientRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}