using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Fakes.Handlers.GroupClaims;
using Business.Fakes.Handlers.UserProjects;
using Business.Handlers.Clients.ValidationRules;
using Business.MessageBrokers;
using Business.MessageBrokers.Models;
using Business.Services.Authentication;
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
        public string ClientId { get; set; }
        public string ProjectId { get; set; }

        public class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, IResult>
        {
            private readonly IClientRepository _clientRepository;
            private readonly IMediator _mediator;

            private readonly ITokenHelper _tokenHelper;
            private readonly IMessageBroker _messageBroker;

            public CreateTokenCommandHandler(IClientRepository clientRepository,
                ITokenHelper tokenHelper,
                IMediator mediator, IMessageBroker messageBroker)

            {
                _clientRepository = clientRepository;
                _mediator = mediator;
                _messageBroker = messageBroker;
                _tokenHelper = tokenHelper;
            }

            [LogAspect(typeof(FileLogger))]
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
                    c.ClientId == request.ClientId
                    && c.ProjectId == request.ProjectId);

                if (result == null)
                {
                    _clientRepository.Add(new Client
                    {
                        ClientId = request.ClientId,
                        ProjectId = request.ProjectId
                    });

                    await _messageBroker.SendMessageAsync(new CreateClientMessageComamnd
                    {
                        ClientId = request.ClientId,
                        ProjectKey = request.ProjectId,
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
                            new OperationClaim { Id = Convert.ToInt32(item.Id), Name = item.Label })
                        .ToList();

                var accessToken = _tokenHelper.CreateClientToken<DArchToken>(new ClientClaimModel
                {
                    ClientId = request.ClientId,
                    ProjectId = request.ProjectId,
                    OperationClaims = operationClaims.Select(x => x.Name).ToArray()
                });
                await _clientRepository.SaveChangesAsync();
                return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
            }
        }
    }
}