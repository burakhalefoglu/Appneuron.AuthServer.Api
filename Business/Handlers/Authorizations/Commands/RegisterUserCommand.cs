using Business.Constants;
using Business.Fakes.Handlers.GroupClaims;
using Business.Fakes.Handlers.UserClaims;
using Business.Handlers.Authorizations.ValidationRules;
using Business.Handlers.UserProjects.Queries;
using Business.Helpers;
using Business.Services.Authentication;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using Core.Utilities.Results;
using Core.Utilities.Security.Encyption;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Authorizations.Commands
{
    public class RegisterUserCommand : IRequest<IResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMediator _mediator;
            private readonly ITokenHelper _tokenHelper;

            public RegisterUserCommandHandler(IUserRepository userRepository,
                IMediator mediator,
                 ITokenHelper tokenHelper,
                  ICacheManager cacheManager)
            {
                _userRepository = userRepository;
                _mediator = mediator;
                _tokenHelper = tokenHelper;
            }

            [PerformanceAspect(5)]
            [ValidationAspect(typeof(RegisterUserValidator), Priority = 2)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [TransactionScopeAspectAsync]
            public async Task<IResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
            {
                var userExits = await _userRepository.GetAsync(u => u.Email == request.Email);

                if (userExits != null)
                    return new ErrorResult(Messages.EmailAlreadyExist);

                HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);
                var user = new User
                {
                    Email = request.Email,
                    Name = UserNameCreationHelper.EmailToUsername(request.Email),
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Status = true,
                    DashboardKey = SecurityKeyHelper.GetRandomHexNumber(32),
                };

                _userRepository.Add(user);
                await _userRepository.SaveChangesAsync();

                var newUser = _userRepository.Get(x => x.Email == user.Email);

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
                    UserId = newUser.UserId,
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
                },ProjectIdList);

                return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
            }
        }
    }
}