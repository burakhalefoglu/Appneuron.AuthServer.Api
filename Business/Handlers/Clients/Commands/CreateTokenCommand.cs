using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Clients.ValidationRules;
using Business.Internals.Handlers.GroupClaims;
using Business.Internals.Handlers.UserProjects;
using Business.MessageBrokers;
using Business.MessageBrokers.Models;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Clients.Commands
{
    public class CreateTokenCommand : IRequest<IResult>
    {
        public long ClientId { get; set; }
        public long ProjectId { get; set; }

        public class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, IResult>
        {
            private readonly IClientRepository _clientRepository;
            private readonly IMediator _mediator;
            private readonly IMessageBroker _messageBroker;

            private readonly ITokenHelper _tokenHelper;

            public CreateTokenCommandHandler(IClientRepository clientRepository,
                ITokenHelper tokenHelper,
                IMediator mediator, IMessageBroker messageBroker)

            {
                _clientRepository = clientRepository;
                _mediator = mediator;
                _messageBroker = messageBroker;
                _tokenHelper = tokenHelper;
            }

            [LogAspect(typeof(ConsoleLogger))]
            [ValidationAspect(typeof(CreateTokenValidator), Priority = 1)]
            public async Task<IResult> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
            {
                var projectInfo = await _mediator.Send(new GetUserProjectInternalQuery
                {
                    ProjectKey = request.ProjectId
                }, cancellationToken);
                if (projectInfo.Data == null)
                    return new ErrorDataResult<AccessToken>(Messages.ProjectNotFound);

                var result = await _clientRepository.GetAsync(c =>
                    c.Id == request.ClientId
                    && c.ProjectId == request.ProjectId);

                if (result == null)
                {
                    await _clientRepository.AddAsync(new Client
                    {
                        ProjectId = request.ProjectId
                    });

                    await _messageBroker.SendMessageAsync(new CreateClientMessageComamnd
                    {
                        ClientId = request.ClientId,
                        ProjectId = request.ProjectId,
                        CreatedAt = DateTime.Now,
                        IsPaidClient = false
                    });
                }

                var resultGroupClaim = await _mediator.Send(new GetGroupClaimsLookupByGroupIdInternalQuery
                {
                    GroupId = 2
                }, cancellationToken);

                var selectionItems = resultGroupClaim.Data.ToList();

                var operationClaims =
                    selectionItems.Select(item =>
                            new OperationClaim {Id = item.Id, Name = item.Label})
                        .ToList();

                var accessToken = _tokenHelper.CreateClientToken<AccessToken>(new ClientClaimModel
                {
                    ClientId = request.ClientId,
                    ProjectId = request.ProjectId,
                    OperationClaims = operationClaims.Select(x => x.Name).ToArray()
                });
                return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
            }
        }
    }
}