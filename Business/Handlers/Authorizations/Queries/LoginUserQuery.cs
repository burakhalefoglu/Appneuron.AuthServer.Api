using Business.Constants;
using Business.Internals.Handlers.GroupClaims;
using Business.Internals.Handlers.UserClaims;
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

            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IDataResult<AccessToken>> Handle(LoginUserQuery request,
                CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetAsync(u => u.Email == request.Email && u.Status);
                // Please return just default error to not give database information !!!
                if (user is null)
                    return new ErrorDataResult<AccessToken>(Messages.DefaultError);

                // Please return just default error to not give database information !!!
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
                    OperationClaims = operationClaims.Select(x => x.Name).ToArray()
                }, user.Email);

                return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
            }
        }
    }
}