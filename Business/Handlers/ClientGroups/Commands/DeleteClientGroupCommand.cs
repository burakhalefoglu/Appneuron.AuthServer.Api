using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ClientGroups.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class DeleteClientGroupCommand : IRequest<IResult>
    {
        public int GroupId { get; set; }

        public class DeleteClientGroupCommandHandler : IRequestHandler<DeleteClientGroupCommand, IResult>
        {
            private readonly IClientGroupRepository _clientGroupRepository;
            private readonly IMediator _mediator;

            public DeleteClientGroupCommandHandler(IClientGroupRepository clientGroupRepository, IMediator mediator)
            {
                _clientGroupRepository = clientGroupRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteClientGroupCommand request, CancellationToken cancellationToken)
            {
                var clientGroupToDelete = _clientGroupRepository.Get(p => p.GroupId == request.GroupId);

                _clientGroupRepository.Delete(clientGroupToDelete);
                await _clientGroupRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}