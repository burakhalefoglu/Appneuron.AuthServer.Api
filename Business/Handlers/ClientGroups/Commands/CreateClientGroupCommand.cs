
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
using Business.Handlers.ClientGroups.ValidationRules;

namespace Business.Handlers.ClientGroups.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateClientGroupCommand : IRequest<IResult>
    {

        public long ClientId { get; set; }


        public class CreateClientGroupCommandHandler : IRequestHandler<CreateClientGroupCommand, IResult>
        {
            private readonly IClientGroupRepository _clientGroupRepository;
            private readonly IMediator _mediator;
            public CreateClientGroupCommandHandler(IClientGroupRepository clientGroupRepository, IMediator mediator)
            {
                _clientGroupRepository = clientGroupRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateClientGroupValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateClientGroupCommand request, CancellationToken cancellationToken)
            {
                var isThereClientGroupRecord = _clientGroupRepository.Query().Any(u => u.ClientId == request.ClientId);

                if (isThereClientGroupRecord == true)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedClientGroup = new ClientGroup
                {
                    ClientId = request.ClientId,

                };

                _clientGroupRepository.Add(addedClientGroup);
                await _clientGroupRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}