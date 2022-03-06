using System;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Authorizations.Commands
{
    public class ResetPasswordCommand : IRequest<IResult>
    {
        public string Password { get; set; }

        public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, IResult>
        {
            private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;
            private readonly IUserRepository _userRepository;

            public ResetPasswordCommandHandler(IUserRepository userRepository,
                Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
                _userRepository = userRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
            {
                var queryString = _httpContextAccessor.HttpContext.Request.Query;
                var token = queryString["token"];
                var resultUser = await _userRepository.GetAsync(p => p.ResetPasswordToken == token.ToString());
                if (resultUser == null) return new ErrorDataResult<User>(Messages.InvalidCode);

                var resultDate = DateTimeOffset.Compare(DateTimeOffset.Now, resultUser.ResetPasswordExpires);
                if (resultDate > 0) return new ErrorDataResult<User>(Messages.InvalidCode);

                HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);

                resultUser.PasswordHash = passwordHash;
                resultUser.PasswordSalt = passwordSalt;
                resultUser.ResetPasswordExpires = DateTimeOffset.MinValue;
                resultUser.ResetPasswordToken = null;
                await _userRepository.UpdateAsync(resultUser);
                return new SuccessDataResult<User>(Messages.ResetPasswordSuccess);
            }
        }
    }
}