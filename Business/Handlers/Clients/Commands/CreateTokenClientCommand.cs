using Business.Constants;
using Business.Fakes.Handlers.Clients.Commands;
using Business.Fakes.Handlers.Clients.Queries;
using Business.Fakes.Handlers.GroupClaims;
using Business.Fakes.Handlers.UserProjects;
using Business.Handlers.Authorizations.ValidationRules;
using Business.MessageBrokers.RabbitMq.Models;
using Business.Services.Authentication;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Business.Fakes.Handlers.Clients.Queries.GetClientInternalQuery;

namespace Business.Handlers.Clients.Commands
{
    public class CreateTokenClientCommand : IRequest<IResult>
    {
        public string ClientId { get; set; }
        public string DashboardKey { get; set; }
        public string ProjectId { get; set; }


        public class CreateTokenClientCommandHandler : IRequestHandler<CreateTokenClientCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMediator _mediator;
            private readonly ITokenHelper _tokenHelper;
            private readonly ICacheManager _cacheManager;
            private readonly ISendEndpointProvider _sendEndpointProvider;


            public CreateTokenClientCommandHandler(ISendEndpointProvider sendEndpointProvider,
                IUserRepository userRepository,
                IMediator mediator,
                 ITokenHelper tokenHelper,
                  ICacheManager cacheManager)
            {
                _userRepository = userRepository;
                _mediator = mediator;
                _tokenHelper = tokenHelper;
                _cacheManager = cacheManager;
                _sendEndpointProvider = sendEndpointProvider;


            }

            [PerformanceAspect(5)]
            [ValidationAspect(typeof(RegisterUserValidator), Priority = 2)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(CreateTokenClientCommand request, CancellationToken cancellationToken)
            {

                var user = await _userRepository.GetAsync(u => u.DashboardKey == request.DashboardKey);

                if (user == null)
                    return new ErrorResult(Messages.UserNotFound);

                var projectInfo = (await _mediator.Send(new GetUserProjectInternalQuery()
                {
                    UserId = user.UserId,
                    ProjectKey = request.ProjectId
                }));

                if (projectInfo.Data == null)
                {
                    return new ErrorResult(Messages.ProjectNotFound);
                }


                var result = (await _mediator.Send(new GetClientInternalQuery()
                {
                    ClientId = request.ClientId,
                    ProjectId = request.ProjectId
                }));

                Client client = result.Data;

                if (client == null)
                {
                    await _mediator.Send(new CreateClientInternalCommand()
                    {
                        ClientId = request.ClientId,
                        ProjectId = request.ProjectId,
                        CustomerId = user.UserId
                    });

                    var clientResult = (await _mediator.Send(new GetClientInternalQuery()
                    {
                        ClientId = request.ClientId,
                        ProjectId = request.ProjectId
                    }));
                    client = clientResult.Data;

                    var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:CreateClientQueue"));

                    await sendEndpoint.Send(new CreateClientMessageComamnd
                    {

                        ClientId = client.ClientId,
                        ProjectKey = client.ProjectId,
                        CreatedAt = DateTime.Now,
                        IsPaidClient = false
                    });
                }

                var resultGroupClaim = (await _mediator.Send(new GetGroupClaimsLookupByGroupIdInternalQuery()
                {
                    GroupId = 2
                }));

                List<SelectionItem> selectionItems = resultGroupClaim.Data.ToList();


                List<OperationClaim> operationClaims = new List<OperationClaim>();

                foreach (var item in selectionItems)
                {
                    operationClaims.Add(new OperationClaim
                    {
                        Id = int.Parse(item.Id),
                        Name = item.Label
                    });
                }


                var accessToken = _tokenHelper.CreateClientToken<DArchToken>(new ClientClaimModel
                {
                    ClientId = client.ClientId,
                    CustomerId = user.UserId,
                    ProjectId = client.ProjectId,
                    OperationClaims = operationClaims.Select(x => x.Name).ToArray()
                });

                _cacheManager.Add($"{CacheKeys.UserIdForClaim}={user.UserId}", operationClaims.Select(x => x.Name));

                return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
            }
        }
    }
}