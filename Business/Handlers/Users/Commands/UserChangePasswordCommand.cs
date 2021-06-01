using AutoMapper;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Business.Handlers.Users.Commands
{
    public class UserChangePasswordCommand : IRequest<IResult>
    {
        public string Password { get; set; }
        public string validPassword { get; set; }

        public class UserChangePasswordCommandHandler : IRequestHandler<UserChangePasswordCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;


            public UserChangePasswordCommandHandler(IUserRepository userRepository, IMediator mediator, IMapper mapper)
            {
                _userRepository = userRepository;
                _mapper = mapper;
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            }

            [SecuredOperation(Priority = 1)]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(UserChangePasswordCommand request, CancellationToken cancellationToken)
            {
                var UserId = int.Parse(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var user = await _userRepository.GetAsync(u => u.UserId == UserId);
                if (user == null)
                    return new ErrorResult(Messages.UserNotFound);


                if (!HashingHelper.VerifyPasswordHash(request.validPassword, user.PasswordSalt, user.PasswordHash))
                    return new ErrorResult(Messages.PasswordError);


                HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}