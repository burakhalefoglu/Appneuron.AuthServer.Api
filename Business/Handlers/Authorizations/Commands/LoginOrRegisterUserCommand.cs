using Business.Constants;
using Business.Helpers;
using Business.Internals.Handlers.GroupClaims;
using Business.Internals.Handlers.RefreshTokens.Commands;
using Business.Internals.Handlers.RefreshTokens.Queries;
using Business.Internals.Handlers.UserClaims;
using Business.Internals.Handlers.UserGroups.Commands;
using Business.Internals.Handlers.UserGroups.Queries;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.ClaimModels;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.Authorizations.Commands;

public class LoginOrRegisterUserCommand : IRequest<IDataResult<AccessToken>>
{
    public string Email { get; set; }
    public string Password { get; set; }

    public class
        LoginOrRegisterUserCommandHandler : IRequestHandler<LoginOrRegisterUserCommand, IDataResult<AccessToken>>
    {
        private readonly IMediator _mediator;
        private readonly ITokenHelper _tokenHelper;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public LoginOrRegisterUserCommandHandler(IUserRepository userRepository,
            ITokenHelper tokenHelper,
            IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _tokenHelper = tokenHelper;
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IDataResult<AccessToken>> Handle(LoginOrRegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(u => u.Email == request.Email && u.Status);
            var refreshTokenResult = "";
            if (user is null)
            {
                HashingHelper.CreatePasswordHash(request.Password,
                    out var passwordSalt, out var passwordHash);
                await _userRepository.AddAsync(new User
                {
                    Email = request.Email,
                    Name = UserNameCreationHelper.EmailToUsername(request.Email),
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Status = true
                });
                user = await _userRepository.GetAsync(x => x.Email == request.Email &&
                                                           x.Status);
                await _mediator.Send(new CreateUserGroupInternalCommand()
                {
                    GroupId = 1,
                    UserId = user.Id
                }, cancellationToken);
                refreshTokenResult = _mediator.Send(new CreateRefreshTokenCommand
                {
                    UserId = user.Id
                }, cancellationToken).Result.Data;
            }

            if (!(user is null))
                refreshTokenResult = _mediator.Send(new GetRefreshTokenQuery(), cancellationToken).Result.Data.Value;

            if (!HashingHelper.VerifyPasswordHash(request.Password, user.PasswordSalt, user.PasswordHash))
                return new ErrorDataResult<AccessToken>(Messages.DefaultError);

            var usrGroup = await _mediator.Send(new GetUserGroupInternalQuery
            {
                UserId = user.Id
            }, cancellationToken);

            var result = await _mediator.Send(new GetGroupClaimInternalQuery
            {
                GroupId = usrGroup.Data.GroupId
            }, cancellationToken);

            var operationClaims = new List<OperationClaim>();

            if (result.Data.ToList().Count > 0)
            {
                var selectionItems = result.Data.ToList();

                operationClaims.AddRange(selectionItems.Select(item => new OperationClaim
                    {Id = item.Id, Name = item.Label}));
            }

            await _mediator.Send(new CreateUserClaimsInternalCommand
            {
                UserId = user.Id,
                OperationClaims = operationClaims
            }, cancellationToken);

            var accessToken = _tokenHelper.CreateCustomerToken<AccessToken>(new UserClaimModel
            {
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                OperationClaims = operationClaims.Select(x => x.Name).ToArray()
            });
            accessToken.RefreshToken = refreshTokenResult;

            return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
        }
    }
}