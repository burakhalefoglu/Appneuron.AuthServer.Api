using Business.Constants;
using Business.Handlers.Authorizations.ValidationRules;
using Business.Services.Authentication;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Encyption;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Authorizations.Commands
{
    public class RegisterUserCommand : IRequest<IResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public short CustomerScaleId { get; set; }

        public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMediator _mediator;
            private readonly ITokenHelper _tokenHelper;
            private readonly ICacheManager _cacheManager;


            public RegisterUserCommandHandler(IUserRepository userRepository,
                IMediator mediator,
                 ITokenHelper tokenHelper,
                  ICacheManager cacheManager)
            {
                _userRepository = userRepository;
                _mediator = mediator;
                _tokenHelper = tokenHelper;
                _cacheManager = cacheManager;


            }

            [PerformanceAspect(5)]
            [ValidationAspect(typeof(RegisterUserValidator), Priority = 2)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [TransactionScopeAspectAsync]
            public async Task<IResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
            {
                if (request.Password != request.ConfirmPassword)
                    return new ErrorResult(Messages.PassworddidntMatch);

                var userExits = await _userRepository.GetAsync(u => u.Email == request.Email);

                if (userExits != null)
                    return new ErrorResult(Messages.NameAlreadyExist);

                HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);
                var user = new User
                {
                    Email = request.Email,
                    Name = request.Name,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Status = true,
                    Notes = request.Notes
                };

                _userRepository.Add(user);
                await _userRepository.SaveChangesAsync();

                //await _mediator.Send(new CreateCustomerCommand
                //{
                //    DashboardKey = SecurityKeyHelper.GetRandomHexNumber(64).ToLower(),
                //    UserId = user.UserId,
                //    CustomerScaleId = request.CustomerScaleId

                //});

                var claims = _userRepository.GetClaims(user.UserId);

                var accessToken = _tokenHelper.CreateToken<DArchToken>(user);
                accessToken.Claims = claims.Select(x => x.Name).ToList();

                _cacheManager.Add($"{CacheKeys.UserIdForClaim}={user.UserId}", claims.Select(x => x.Name));

                return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
            }
        }
    }
}