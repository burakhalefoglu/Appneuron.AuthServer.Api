using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Fakes.Handlers.UserClaims;
using Business.Handlers.UserProjects.Queries;
using Business.Internals.Handlers.GroupClaims;
using Business.Internals.Handlers.UserClaims;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Authorizations.Queries
{
    public class LoginUserQuery : IRequest<IDataResult<AccessToken>>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, IDataResult<AccessToken>>
        {
            private readonly IMediator _mediator;
            private readonly ITokenHelper _tokenHelper;
            private readonly IUserRepository _userRepository;

            public LoginUserQueryHandler(IUserRepository userRepository,
                ITokenHelper tokenHelper,
                IMediator mediator)
            {
                _userRepository = userRepository;
                _tokenHelper = tokenHelper;
                _mediator = mediator;
            }

            [LogAspect(typeof(LogstashLogger))]
            public async Task<IDataResult<AccessToken>> Handle(LoginUserQuery request,
                CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetAsync(u => u.Email == request.Email);
                // Please return just default error to not give database information !!!
                if (user == null)
                    return new ErrorDataResult<AccessToken>(Messages.DefaultError);

                // Please return just default error to not give database information !!!
                if (!HashingHelper.VerifyPasswordHash(request.Password, user.PasswordSalt, user.PasswordHash))
                    return new ErrorDataResult<AccessToken>(Messages.DefaultError);

                var result = await _mediator.Send(new GetGroupClaimsLookupByGroupIdInternalQuery
                {
                    GroupId = 1
                }, cancellationToken);

                var operationClaims = new List<OperationClaim>();

                if (result.Data.ToList().Count > 0)
                {
                    var selectionItems = result.Data.ToList();

                    operationClaims.AddRange(selectionItems.Select(item => new OperationClaim { Id = Convert.ToInt32(item.Id), Name = item.Label }));
                }


                await _mediator.Send(new CreateUserClaimsInternalCommand
                {
                    UserId = user.UserId,
                    OperationClaims = operationClaims
                }, cancellationToken);

                var projectIdResult = await _mediator.Send(new GetUserProjectsInternalQuery
                {
                    UserId = user.UserId
                }, cancellationToken);
                var projectIdList = new List<string>();
                projectIdResult.Data.ToList().ForEach(x => { projectIdList.Add(x.ProjectKey); });

                var accessToken = _tokenHelper.CreateCustomerToken<AccessToken>(new UserClaimModel
                {
                    UserId = user.UserId,
                    OperationClaims = operationClaims.Select(x => x.Name).ToArray()
                }, projectIdList);

                return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
            }
        }
    }
}