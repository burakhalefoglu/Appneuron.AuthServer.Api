
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Core.Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.Clients.ValidationRules;

namespace Business.Handlers.Clients.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateClientCommand : IRequest<IResult>
    {

        public string ClientId { get; set; }
        public string ProjectId { get; set; }
        public int CustomerId { get; set; }


        public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, IResult>
        {
            private readonly IClientRepository _clientRepository;
            private readonly IMediator _mediator;
            public CreateClientCommandHandler(IClientRepository clientRepository, IMediator mediator)
            {
                _clientRepository = clientRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateClientValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateClientCommand request, CancellationToken cancellationToken)
            {
                var isThereClientRecord = _clientRepository.Query().Any(u => u.ClientId == request.ClientId);

                if (isThereClientRecord)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedClient = new Client
                {
                    ClientId = request.ClientId,
                    ProjectId = request.ProjectId,
                    CustomerId = request.CustomerId,

                };

                _clientRepository.Add(addedClient);
                await _clientRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}