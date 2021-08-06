using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.ClientGroups.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ClientGroups.Commands
{
    public class UpdateClientGroupCommand : IRequest<IResult>
    {
        public int GroupId { get; set; }
        public long ClientId { get; set; }

        public class UpdateClientGroupCommandHandler : IRequestHandler<UpdateClientGroupCommand, IResult>
        {
            private readonly IClientGroupRepository _clientGroupRepository;
            private readonly IMediator _mediator;

            public UpdateClientGroupCommandHandler(IClientGroupRepository clientGroupRepository, IMediator mediator)
            {
                _clientGroupRepository = clientGroupRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateClientGroupValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateClientGroupCommand request, CancellationToken cancellationToken)
            {
                var isThereClientGroupRecord = await _clientGroupRepository.GetAsync(u => u.GroupId == request.GroupId);

                isThereClientGroupRecord.ClientId = request.ClientId;

                _clientGroupRepository.Update(isThereClientGroupRecord);
                await _clientGroupRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}