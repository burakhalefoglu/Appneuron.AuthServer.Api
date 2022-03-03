using Business.Constants;
using Business.Handlers.Authorizations.ValidationRules;
using Business.Helpers;
using Business.Internals.Handlers.GroupClaims;
using Business.Internals.Handlers.Groups.Queries;
using Business.Internals.Handlers.UserClaims;
using Business.Internals.Handlers.UserGroups.Commands;
using Business.MessageBrokers.Models;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.ApiModel;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Utilities.MessageBrokers;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;

namespace Business.Abstract;

public class AuthManager: IAuthService
{
    private readonly ITokenHelper _tokenHelper;
    private readonly IUserRepository _userRepository;
    private readonly IMessageBroker _messageBroker;

    public AuthManager(
        IUserRepository userRepository,
        ITokenHelper tokenHelper,
        IMessageBroker messageBroker
    )
    {
        _userRepository = userRepository;
        _tokenHelper = tokenHelper;
        _messageBroker = messageBroker;
    }

    [PerformanceAspect(5)]
    [ValidationAspect(typeof(RegisterUserValidator), Priority = 2)]
    [CacheRemoveAspect("Get")]
    [LogAspect(typeof(ConsoleLogger))]
    [TransactionScopeAspect]
    public async Task<IDataResult<AccessToken>> Register(Register register)
    {
        var userExits = await _userRepository.AnyAsync(u => u.Email == register.Email && u.Status == true);
                
                if (userExits)
                    return new ErrorDataResult<AccessToken>(Messages.DefaultError);
                
                HashingHelper.CreatePasswordHash(register.Password, 
                    out var passwordSalt, out var passwordHash);
                var user = new User
                {
                    Email = register.Email,
                    Name = UserNameCreationHelper.EmailToUsername(register.Email),
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Status = true
                };
                
                await _userRepository.AddAsync(user);
                
                // var usr = await _userRepository.GetAsync(x => x.Email == user.Email);
                // var group = await _mediator.Send(new GetGroupByNameInternalQuery
                // {
                //     GroupName = "customer_group"
                // });
                //
                // _ = await _mediator.Send(new CreateUserGroupInternalCommand
                // {
                //     UserId = usr.Id,
                //     GroupId = group.Data.Id
                // });
                //
                // var result = await _mediator.Send(new GetGroupClaimsLookupByGroupIdInternalQuery
                // {
                //     GroupId = group.Data.Id
                // }, cancellationToken);
                //
                // var selectionItems = result.Data.ToList();
                // var oClaims = new List<OperationClaim>();
                //
                // if (selectionItems.ToList().Count > 0)
                //     oClaims = selectionItems.Select(item =>
                //         new OperationClaim {Id = item.Id, Name = item.Label}).ToList();
                //
                // await _mediator.Send(new CreateUserClaimsInternalCommand
                // {
                //     UserId = user.Id,
                //     OperationClaims = oClaims
                // });
                
                var accessToken = _tokenHelper.CreateCustomerToken<AccessToken>(new UserClaimModel
                {
                    UserId = user.Id,
                    OperationClaims =  new []{"test"} //oClaims.Select(x => x.Name).ToArray()
                }, new List<long>());
                
                // create customer with kafka 
                await _messageBroker.SendMessageAsync(new CreateCustomerMessageCommand
                {
                 Id =  user.Id,
                 DemographicId = 0,
                 IndustryId = 1
                });
                
                return new SuccessDataResult<AccessToken>(new AccessToken(), Messages.SuccessfulLogin);

    }
}