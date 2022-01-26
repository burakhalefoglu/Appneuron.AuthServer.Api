using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Authorizations.ValidationRules;
using Business.Helpers;
using Business.Internals.Handlers.GroupClaims;
using Business.Internals.Handlers.Groups.Queries;
using Business.Internals.Handlers.UserClaims;
using Business.Internals.Handlers.UserGroups.Commands;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using MediatR;
using MongoDB.Bson;

namespace Business.Handlers.Authorizations.Commands
{
    public class RegisterUserCommand : IRequest<IResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResult>
        {
            private readonly IMediator _mediator;
            private readonly ITokenHelper _tokenHelper;
            private readonly IUserRepository _userRepository;

            public RegisterUserCommandHandler(IUserRepository userRepository,
                IMediator mediator,
                ITokenHelper tokenHelper)
            {
                _userRepository = userRepository;
                _mediator = mediator;
                _tokenHelper = tokenHelper;
            }

            [PerformanceAspect(5)]
            [ValidationAspect(typeof(RegisterUserValidator), Priority = 2)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            [TransactionScopeAspect]
            public async Task<IResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
            {
                var userExits = await _userRepository.AnyAsync(u => u.Email == request.Email);

                if (userExits)
                    return new ErrorResult(Messages.DefaultError);

                HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);
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
                var group = await _mediator.Send(new GetGroupByNameInternalQuery
                {
                    GroupName = "customer_group"
                }, cancellationToken);
                
                _ = await _mediator.Send(new CreateUserGroupInternalCommand
                {
                    UserId = usr.ObjectId,
                    GroupId = group.Data.ObjectId
                }, cancellationToken);
                
                var result = await _mediator.Send(new GetGroupClaimsLookupByGroupIdInternalQuery
                {
                    GroupId = group.Data.ObjectId
                }, cancellationToken);

                var selectionItems = result.Data.ToList();
                var oClaims = new List<OperationClaim>();

                if (selectionItems.ToList().Count > 0)
                    oClaims = selectionItems.Select(item =>
                        new OperationClaim {Id = new ObjectId(item.Id), Name = item.Label}).ToList();

                await _mediator.Send(new CreateUserClaimsInternalCommand
                {
                    UserId = user.ObjectId,
                    OperationClaims = oClaims
                }, cancellationToken);

                var accessToken = _tokenHelper.CreateCustomerToken<AccessToken>(new UserClaimModel
                {
                    UserId = user.ObjectId,
                    OperationClaims = oClaims.Select(x => x.Name).ToArray()
                }, new List<string>());

                return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
            }
        }
    }
}