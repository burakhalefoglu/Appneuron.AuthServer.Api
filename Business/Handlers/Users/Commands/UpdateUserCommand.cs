using System;
using AutoMapper;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Users.Commands
{
    public class UpdateUserCommand : IRequest<IResult>
    {
        public string Email { get; set; }
        public string FullName { get; set; }

        public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public UpdateUserCommandHandler(IUserRepository userRepository,
                IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _userRepository = userRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                var userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var isUserExits = await _userRepository.GetAsync(u => u.UserId == userId);
                if (isUserExits == null)
                {
                    return new ErrorResult(Messages.UserNotFound);
                }
                isUserExits.Name = request.FullName;
                isUserExits.Email = request.Email;

                _userRepository.Update(isUserExits);
                await _userRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}