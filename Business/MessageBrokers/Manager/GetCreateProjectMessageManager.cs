using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Internals.Handlers.GroupClaims;
using Business.Internals.Handlers.UserClaims;
using Business.Internals.Handlers.UserProjects;
using Business.MessageBrokers.Models;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using MediatR;

namespace Business.MessageBrokers.Manager
{
    public class GetCreateProjectMessageManager : IGetCreateProjectMessageService
    {
        private readonly IMediator _mediator;
        private readonly IMessageBroker _messageBroker;
        private readonly ITokenHelper _tokenHelper;
        private readonly IUserRepository _userRepository;

        public GetCreateProjectMessageManager(IMediator mediator, IUserRepository userRepository,
            ITokenHelper tokenHelper, IMessageBroker messageBroker)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _tokenHelper = tokenHelper;
            _messageBroker = messageBroker;
        }

        public async Task<IResult> GetProjectCreationMessageQuery(ProjectMessageCommand message)
        {
            _ = await _mediator.Send(new CreateUserProjectInternalCommand
            {
                UserId = message.UserId,
                ProjectKey = message.ProjectKey
            });

            var user = await _userRepository.GetAsync(u =>
                u.ObjectId == message.UserId);

            if (user == null)
                return new ErrorResult();

            //New Token Creation
            var groupClaims = await _mediator.Send(new GetGroupClaimsLookupByGroupIdInternalQuery
            {
                GroupId = "Test"
            });

            var selectionItems = groupClaims.Data.ToList();
            var operationClaims = selectionItems
                .Select(item => new OperationClaim {Id = int.Parse(item.Id), Name = item.Label}).ToList();

            await _mediator.Send(new CreateUserClaimsInternalCommand
            {
                UserId = user.ObjectId,
                OperationClaims = operationClaims
            });

            var projectIdResult = await _mediator.Send(new GetUserProjectsInternalQuery
            {
                UserId = user.ObjectId
            });
            var projectIdList = new List<string>();
            projectIdResult.Data.ToList().ForEach(x => { projectIdList.Add(x.ProjectKey); });

            var accessToken = _tokenHelper.CreateCustomerToken<AccessToken>(new UserClaimModel
            {
                UserId = user.ObjectId,
                OperationClaims = operationClaims.Select(x => x.Name).ToArray()
            }, projectIdList);

            var kafkaResult = await _messageBroker.SendMessageAsync(new ProjectCreationResult
            {
                Accesstoken = accessToken.Token,
                UserId = user.ObjectId
            });

            if (kafkaResult.Success)
                return new SuccessResult();

            return new ErrorResult();
        }
    }
}