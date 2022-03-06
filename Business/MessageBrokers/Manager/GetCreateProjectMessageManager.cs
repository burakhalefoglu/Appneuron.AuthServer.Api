using Business.Internals.Handlers.GroupClaims;
using Business.Internals.Handlers.UserClaims;
using Business.Internals.Handlers.UserProjects;
using Business.MessageBrokers.Models;
using Core.Aspects.Autofac.Exception;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Utilities.MessageBrokers;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using MassTransit;
using MediatR;

namespace Business.MessageBrokers.Manager
{
    [LogAspect(typeof(ConsoleLogger))]
    [ExceptionLogAspect(typeof(ConsoleLogger))]
    public class GetCreateProjectMessageConsumer :
    IConsumer<ProjectMessageCommand>
    {
        private readonly IMediator _mediator;
        private readonly IMessageBroker _messageBroker;
        private readonly ITokenHelper _tokenHelper;
        private readonly IUserRepository _userRepository;

        public GetCreateProjectMessageConsumer(IMediator mediator, IUserRepository userRepository,
            ITokenHelper tokenHelper, IMessageBroker messageBroker)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _tokenHelper = tokenHelper;
            _messageBroker = messageBroker;
        }
        public async Task Consume(ConsumeContext<ProjectMessageCommand> context)
        {
            _ = await _mediator.Send(new CreateUserProjectInternalCommand
            {
                UserId = context.Message.UserId,
                ProjectId = context.Message.ProjectId
            });

            var user = await _userRepository.GetAsync(u =>
                u.Id == context.Message.UserId);

            //New Token Creation
            var groupClaims = await _mediator.Send(new GetGroupClaimInternalQuery
            {
                GroupId = 1
            });

            var selectionItems = groupClaims.Data.ToList();
            var operationClaims = selectionItems
                .Select(item => new OperationClaim {Id = item.Id, Name = item.Label}).ToList();

            await _mediator.Send(new CreateUserClaimsInternalCommand
            {
                UserId = user.Id,
                OperationClaims = operationClaims
            });

            var projectIdResult = await _mediator.Send(new GetUserProjectsInternalQuery
            {
                UserId = user.Id
            });
            var projectIdList = new List<long>();
            projectIdResult.Data.ToList().ForEach(x => { projectIdList.Add(x.ProjectId); });

            var accessToken = _tokenHelper.CreateCustomerToken<AccessToken>(new UserClaimModel
            {
                UserId = user.Id,
                OperationClaims = operationClaims.Select(x => x.Name).ToArray()
            }, projectIdList);

            var kafkaResult = await _messageBroker.SendMessageAsync(new ProjectCreationResult
            {
                Accesstoken = accessToken.Token,
                UserId = user.Id
            });

            if (kafkaResult.Success)
            { 
                await context.ConsumeCompleted;
            }
        }
    }
}