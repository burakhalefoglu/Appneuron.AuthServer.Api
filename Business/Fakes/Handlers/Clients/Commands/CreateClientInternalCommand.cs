using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Fakes.Handlers.Clients.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class CreateClientInternalCommand : IRequest<IResult>
    {
        public string ClientId { get; set; }
        public string ProjectId { get; set; }
        public int CustomerId { get; set; }

        public class CreateClientCommandHandler : IRequestHandler<CreateClientInternalCommand, IResult>
        {
            private readonly IClientRepository _clientRepository;
            private readonly IMediator _mediator;

            public CreateClientCommandHandler(IClientRepository clientRepository, IMediator mediator)
            {
                _clientRepository = clientRepository;
                _mediator = mediator;
            }

            public async Task<IResult> Handle(CreateClientInternalCommand request, CancellationToken cancellationToken)
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