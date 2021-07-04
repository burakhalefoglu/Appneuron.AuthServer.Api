using Business.Constants;
using Business.Fakes.Handlers.GroupClaims;
using Business.Fakes.Handlers.UserClaims;
using Business.Handlers.UserProjects.Queries;
using Business.Services.Authentication;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Authorizations.Queries
{
    public class LoginUserQuery : IRequest<IDataResult<AccessToken>>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, IDataResult<AccessToken>>
        {
            private readonly IUserRepository _userRepository;
            private readonly ITokenHelper _tokenHelper;
            private readonly IMediator _mediator;
            private readonly ICacheManager _cacheManager;

            public LoginUserQueryHandler(IUserRepository userRepository,
                ITokenHelper tokenHelper,
                IMediator mediator,
                ICacheManager cacheManager)
            {
                _userRepository = userRepository;
                _tokenHelper = tokenHelper;
                _mediator = mediator;
                _cacheManager = cacheManager;
            }

            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<AccessToken>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetAsync(u => u.Email == request.Email);

                if (user == null)
                    return new ErrorDataResult<AccessToken>(Messages.UserNotFound);

                if (!HashingHelper.VerifyPasswordHash(request.Password, user.PasswordSalt, user.PasswordHash))
                    return new ErrorDataResult<AccessToken>(Messages.PasswordError);

                var result = (await _mediator.Send(new GetGroupClaimsLookupByGroupIdInternalQuery()
                {
                    GroupId = 1
                }));

                List<SelectionItem> selectionItems = result.Data.ToList();
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


                _cacheManager.Add($"{CacheKeys.UserIdForClaim}={user.UserId}", selectionItems.Select(x => x.Label));

                return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
            }
        }
    }
}