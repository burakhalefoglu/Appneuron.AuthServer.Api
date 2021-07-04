using Business.Fakes.Handlers.GroupClaims;
using Business.Fakes.Handlers.UserClaims;
using Business.Fakes.Handlers.UserProjects;
using Business.Handlers.UserProjects.Queries;
using Business.MessageBrokers.RabbitMq.Models;
using Business.MessageBrokers.SignalR;
using Business.Services.Authentication;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.MessageBrokers.RabbitMq.Consumers
{
    public class CreateProjectMessageCommandConsumer : IConsumer<ProjectMessageCommand>
    {
        private readonly IMediator _mediator;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private IHubContext<TestHub> _hubContext;
        private readonly ITokenHelper _tokenHelper;
        private readonly IUserRepository _userRepository;

        public CreateProjectMessageCommandConsumer(IMediator mediator,
            ISendEndpointProvider sendEndpointProvider,
            IHubContext<TestHub> hubContext,
            ITokenHelper tokenHelper,
            IUserRepository userRepository)
        {
            _mediator = mediator;
            _sendEndpointProvider = sendEndpointProvider;
            _hubContext = hubContext;
            _tokenHelper = tokenHelper;
            _userRepository = userRepository;


        }

        public async Task Consume(ConsumeContext<ProjectMessageCommand> context)
        {

            var result = await _mediator.Send(new CreateUserProjectInternalCommand
            {
                UserId = context.Message.UserId,
                ProjectKey = context.Message.ProjectKey
            });
            var userHasProject = TestHub.userList.Find(user => user.UserId == context.Message.UserId);
            if (userHasProject == null)
                return;

            var user = await _userRepository.GetAsync(u => u.UserId == context.Message.UserId);

            if (user == null)
                return;

            //New Token Creation
            var GrupClaims = (await _mediator.Send(new GetGroupClaimsLookupByGroupIdInternalQuery()
            {
                GroupId = 1
            }));

            List<SelectionItem> selectionItems = GrupClaims.Data.ToList();
            List<OperationClaim> operationClaims = new List<OperationClaim>();

            foreach (var item in selectionItems)
            {
                operationClaims.Add(new OperationClaim
                {
                    Id = int.Parse(item.Id),
                    Name = item.Label
                });
            }

            await _mediator.Send(new CreateUserClaimsInternalCommand
            {
                UserId = user.UserId,
                OperationClaims = operationClaims,
            });

            var ProjectIdResult = await _mediator.Send(new GetUserProjectsInternalQuery
            {
                UserId = user.UserId,
            });
            List<string> ProjectIdList = new List<string>();
            ProjectIdResult.Data.ToList().ForEach(x =>
            {
                ProjectIdList.Add(x.ProjectKey);
            });

            var accessToken = _tokenHelper.CreateCustomerToken<DArchToken>(new UserClaimModel
            {
                UserId = user.UserId,
                UniqueKey = user.DashboardKey,
                OperationClaims = operationClaims.Select(x => x.Name).ToArray()
            }, ProjectIdList);

            _hubContext.Clients.Client(userHasProject.ConnectionId)
                .SendAsync("getProjectCreatedEvent", accessToken.Token).Wait();
        }
    }
}