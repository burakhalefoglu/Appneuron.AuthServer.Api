using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Fakes.Handlers.GroupClaims;
using Business.Fakes.Handlers.UserClaims;
using Business.Fakes.Handlers.UserProjects;
using Business.Handlers.UserProjects.Queries;
using Business.MessageBrokers.Models;
using Business.Services.Authentication;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using MassTransit;
using MediatR;

namespace Business.MessageBrokers.RabbitMq.Consumers
{
    public class CreateProjectMessageCommandConsumer : IConsumer<ProjectMessageCommand>
    {
        private readonly IMediator _mediator;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ITokenHelper _tokenHelper;
        private readonly IUserRepository _userRepository;

        public CreateProjectMessageCommandConsumer(IMediator mediator,
            ISendEndpointProvider sendEndpointProvider,
            ITokenHelper tokenHelper,
            IUserRepository userRepository)
        {
            _mediator = mediator;
            _sendEndpointProvider = sendEndpointProvider;
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

            var user = await _userRepository.GetAsync(u => u.UserId == context.Message.UserId);

            if (user == null)
                return;

            //New Token Creation
            var GrupClaims = await _mediator.Send(new GetGroupClaimsLookupByGroupIdInternalQuery
            {
                GroupId = 1
            });

            var selectionItems = GrupClaims.Data.ToList();
            var operationClaims = new List<OperationClaim>();

            foreach (var item in selectionItems)
                operationClaims.Add(new OperationClaim
                {
                    Id = int.Parse(item.Id),
                    Name = item.Label
                });

            await _mediator.Send(new CreateUserClaimsInternalCommand
            {
                UserId = user.UserId,
                OperationClaims = operationClaims
            });

            var ProjectIdResult = await _mediator.Send(new GetUserProjectsInternalQuery
            {
                UserId = user.UserId
            });
            var ProjectIdList = new List<string>();
            ProjectIdResult.Data.ToList().ForEach(x => { ProjectIdList.Add(x.ProjectKey); });

            var accessToken = _tokenHelper.CreateCustomerToken<DArchToken>(new UserClaimModel
            {
                UserId = user.UserId,
                OperationClaims = operationClaims.Select(x => x.Name).ToArray()
            }, ProjectIdList);

            //TODO: will send token with socket :)
        }
    }
}