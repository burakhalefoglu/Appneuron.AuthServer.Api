using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Entities.Concrete;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Authorizations.Commands
{
    public class ResetPasswordCommand : IRequest<IResult>
    {
        public string Password { get; set; }

        public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public ResetPasswordCommandHandler(IUserRepository userRepository,
                IMediator mediator)
            {
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
                _userRepository = userRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ApacheKafkaForgotResetLogger))]
            public async Task<IResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
            {
                var queryString = _httpContextAccessor.HttpContext.Request.Query;
                var token = queryString["token"];
                var resultUser = await _userRepository.GetAsync(p => p.ResetPasswordToken == token.ToString());
                if (resultUser == null)
                {
                    return new ErrorDataResult<User>(Messages.InvalidCode);
                }

                int resultDate = DateTime.Compare(DateTime.Now, resultUser.ResetPasswordExpires);
                if (resultDate > 0)
                {
                    return new ErrorDataResult<User>(Messages.InvalidCode);
                }

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