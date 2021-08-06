﻿using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.ClientClaims.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ClientClaims.Commands
{
    public class UpdateClientClaimCommand : IRequest<IResult>
    {
        public long ClientId { get; set; }
        public int ClaimId { get; set; }

        public class UpdateClientClaimCommandHandler : IRequestHandler<UpdateClientClaimCommand, IResult>
        {
            private readonly IClientClaimRepository _clientClaimRepository;
            private readonly IMediator _mediator;

            public UpdateClientClaimCommandHandler(IClientClaimRepository clientClaimRepository, IMediator mediator)
            {
                _clientClaimRepository = clientClaimRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateClientClaimValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateClientClaimCommand request, CancellationToken cancellationToken)
            {
                var isThereClientClaimRecord = await _clientClaimRepository.GetAsync(u => u.ClientId == request.ClientId);

                isThereClientClaimRecord.ClaimId = request.ClaimId;

                _clientClaimRepository.Update(isThereClientClaimRecord);
                await _clientClaimRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}