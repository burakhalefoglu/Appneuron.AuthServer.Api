using Business.Constants;
using Business.Handlers.Authorizations.ValidationRules;
using Business.Helpers;
using Business.Internals.Handlers.GroupClaims;
using Business.Internals.Handlers.UserClaims;
using Business.Internals.Handlers.UserGroups.Commands;
using Business.MessageBrokers.Models;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Utilities.MessageBrokers;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Authorizations.Commands
{
    public class RegisterUserCommand : IRequest<IDataResult<AccessToken>>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IDataResult<AccessToken>>
        {
            private readonly IMediator _mediator;
            private readonly ITokenHelper _tokenHelper;
            private readonly IUserRepository _userRepository;
            private readonly IMessageBroker _messageBroker;

            public RegisterUserCommandHandler(
                IUserRepository userRepository,
                IMediator mediator,
                ITokenHelper tokenHelper,
                IMessageBroker messageBroker
                )
            {
                _userRepository = userRepository;
                _mediator = mediator;
                _tokenHelper = tokenHelper;
                _messageBroker = messageBroker;
            }

            [PerformanceAspect(5)]
            [ValidationAspect(typeof(RegisterUserValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [TransactionScopeAspect]
            public async Task<IDataResult<AccessToken>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
            {
                var userExits = await _userRepository.AnyAsync(u => u.Email == request.Email && u.Status == true);
                
                if (userExits)
                    return new ErrorDataResult<AccessToken>(Messages.DefaultError);
                
                HashingHelper.CreatePasswordHash(request.Password, 
                    out var passwordSalt, out var passwordHash);
                var user = new User
                {
                    Email = request.Email,
                    Name = UserNameCreationHelper.EmailToUsername(request.Email),
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Status = true
                };
                
                await _userRepository.AddAsync(user);
                
                var usr = await _userRepository.GetAsync(x => x.Email == user.Email);
                // if user type is more than one, we will use group ıd with query
                _ = await _mediator.Send(new CreateUserGroupInternalCommand
                {
                    UserId = usr.Id,
                    GroupId = 1 // is default for game customer
                }, cancellationToken);
                
                var result = await _mediator.Send(new GetGroupClaimInternalQuery
                {
                    GroupId = 1
                }, cancellationToken);
                
                var selectionItems = result.Data.ToList();
                var oClaims = new List<OperationClaim>();
                
                if (selectionItems.ToList().Count > 0)
                    oClaims = selectionItems.Select(item =>
                        new OperationClaim {Id = item.Id, Name = item.Label}).ToList();
                
                await _mediator.Send(new CreateUserClaimsInternalCommand
                {
                    UserId = user.Id,
                    OperationClaims = oClaims
                }, cancellationToken);
                
                var accessToken = _tokenHelper.CreateCustomerToken<AccessToken>(new UserClaimModel
                {
                    UserId = user.Id,
                    OperationClaims = oClaims.Select(x => x.Name).ToArray()
                }, new List<long>());
                
                // create customer with kafka 
                await _messageBroker.SendMessageAsync(new CreateCustomerMessageCommand
                {
                 Id =  user.Id,
                 DemographicId = 0,
                 IndustryId = 1
                });
                
                return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
            }
        }
    }
}