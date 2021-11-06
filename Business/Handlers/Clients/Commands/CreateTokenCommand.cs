using System;
using Business.Constants;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Business.Fakes.Handlers.UserProjects;
using Core.Utilities.Security.Jwt;
using Core.Entities.Concrete;
using Business.Fakes.Handlers.GroupClaims;
using Business.Services.Authentication;
using Core.Entities.ClaimModels;
using System.Linq;
using Business.Handlers.Clients.ValidationRules;
using Core.Aspects.Autofac.Validation;

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
            //private readonly IKafkaMessageBroker _kafkaMessageBroker;

            public CreateTokenCommandHandler(IClientRepository clientRepository,
                ITokenHelper tokenHelper,
                IMediator mediator
               )

            {
                _clientRepository = clientRepository;
                _mediator = mediator;
                _tokenHelper = tokenHelper;
            }

            [LogAspect(typeof(FileLogger))]
            [ValidationAspect(typeof(CreateTokenValidator), Priority = 1)]
            public async Task<IResult> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
            {
                var projectInfo = await _mediator.Send(new GetUserProjectInternalQuery()
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
                    var client = _clientRepository.Add(new Client
                    {
                        ClientId = request.ClientId,
                        ProjectId = request.ProjectId
                    });

                    //await _kafkaMessageBroker.SendMessageAsync(new CreateClientMessageComamnd
                    //{
                    //    ClientId = request.ClientId,
                    //    ProjectKey = request.ProjectId,
                    //    CreatedAt = DateTime.Now,
                    //    IsPaidClient = false
                    //});
                }

                var resultGroupClaim = await _mediator.Send(new GetGroupClaimsLookupByGroupIdInternalQuery()
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