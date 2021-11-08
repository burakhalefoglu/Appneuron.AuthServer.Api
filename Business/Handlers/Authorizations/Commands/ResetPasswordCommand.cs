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
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.Authorizations.Commands
{
    public class ResetPasswordCommand : IRequest<IResult>
    {
        public string Password { get; set; }

        public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, IResult>
        {
            public readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMediator _mediator;
            private readonly IUserRepository _userRepository;

            public ResetPasswordCommandHandler(IUserRepository userRepository,
                IMediator mediator,
                IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
                _userRepository = userRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
            {
                var queryString = _httpContextAccessor.HttpContext.Request.Query;
                var token = queryString["token"];
                var resultUser = await _userRepository.GetAsync(p => p.ResetPasswordToken == token.ToString());
                if (resultUser == null) return new ErrorDataResult<User>(Messages.InvalidCode);

                var resultDate = DateTime.Compare(DateTime.Now, resultUser.ResetPasswordExpires);
                if (resultDate > 0) return new ErrorDataResult<User>(Messages.InvalidCode);

                HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);

                resultUser.PasswordHash = passwordHash;
                resultUser.PasswordSalt = passwordSalt;
                resultUser.ResetPasswordExpires = DateTime.MinValue;
                resultUser.ResetPasswordToken = null;

                _userRepository.Update(resultUser);
                await _userRepository.SaveChangesAsync();

                return new SuccessDataResult<User>(Messages.ResetPasswordSuccess);
            }
        }
    }
}