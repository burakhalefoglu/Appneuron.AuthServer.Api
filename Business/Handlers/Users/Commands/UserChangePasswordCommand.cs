using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.Users.Commands
{
    public class UserChangePasswordCommand : IRequest<IResult>
    {
        public string Password { get; set; }
        public string ValidPassword { get; set; }

        public class UserChangePasswordCommandHandler : IRequestHandler<UserChangePasswordCommand, IResult>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IUserRepository _userRepository;

            public UserChangePasswordCommandHandler(IUserRepository userRepository,
                IHttpContextAccessor httpContextAccessor)
            {
                _userRepository = userRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            [SecuredOperation(Priority = 1)]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IResult> Handle(UserChangePasswordCommand request, CancellationToken cancellationToken)
            {
                var userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var user = await _userRepository.GetAsync(u => u.UserId == userId);
                if (user == null)
                    return new ErrorResult(Messages.UserNotFound);

                if (!HashingHelper.VerifyPasswordHash(request.ValidPassword, user.PasswordSalt, user.PasswordHash))
                    return new ErrorResult(Messages.PasswordError);

                HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                await _userRepository.UpdateAsync(user,x=> x.UserId == user.UserId);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}