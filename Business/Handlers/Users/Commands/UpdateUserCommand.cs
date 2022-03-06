using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Users.Commands
{
    public class UpdateUserCommand : IRequest<IResult>
    {
        public string Email { get; set; }
        public string FullName { get; set; }

        public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResult>
        {
            private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;
            private readonly IUserRepository _userRepository;

            public UpdateUserCommandHandler(IUserRepository userRepository,
                Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
            {
                _userRepository = userRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                var email = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("email"))?.Value;

                var isUserExits = await _userRepository.GetAsync(u => u.Email == email && u.Status == true);
                if (isUserExits == null) return new ErrorResult(Messages.UserNotFound);
                isUserExits.Name = request.FullName;
                isUserExits.Email = request.Email;

                await _userRepository.UpdateAsync(isUserExits);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}