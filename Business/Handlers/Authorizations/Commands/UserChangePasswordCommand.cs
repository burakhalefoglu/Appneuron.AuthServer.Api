using System.IdentityModel.Tokens.Jwt;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Authorizations.Commands
{
    public class UserChangePasswordCommand : IRequest<IResult>
    {
        public string Password { get; set; }
        public string ValidPassword { get; set; }

        public class UserChangePasswordCommandHandler : IRequestHandler<UserChangePasswordCommand, IResult>
        {
            private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;
            private readonly IUserRepository _userRepository;

            public UserChangePasswordCommandHandler(IUserRepository userRepository,
                Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
            {
                _userRepository = userRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            [SecuredOperation(Priority = 1)]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(UserChangePasswordCommand request, CancellationToken cancellationToken)
            {
                // var email = _httpContextAccessor.HttpContext?.User.Claims
                //     .FirstOrDefault(x => x.Type.Equals("email"))?.Value;
                
                var handler = new JwtSecurityTokenHandler();
                string authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
                authHeader = authHeader.Replace("Bearer ", "");
                var jsonToken = handler.ReadToken(authHeader);
                var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
                var email = tokenS.Claims.First(claim => claim.Type == "type").Value;
                
                var user = await _userRepository.GetAsync(u => u.Email == email && u.Status == true);
                if (user == null)
                    return new ErrorResult(Messages.DefaultError);

                if (!HashingHelper.VerifyPasswordHash(request.ValidPassword, user.PasswordSalt, user.PasswordHash))
                    return new ErrorResult(Messages.PasswordError);

                HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                await _userRepository.UpdateAsync(user);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}