using System;
using AutoMapper;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Users.Queries
{
    public class GetUserQuery : IRequest<IDataResult<UserDto>>
    {
        public class GetUserQueryHandler : IRequestHandler<GetUserQuery, IDataResult<UserDto>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetUserQueryHandler(IUserRepository userRepository,
                IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _userRepository = userRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            [SecuredOperation(Priority = 1)]
            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
            {
                var userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var user = await _userRepository.GetAsync(p => p.UserId == userId);
                var userDto = _mapper.Map<UserDto>(user);
                return new SuccessDataResult<UserDto>(userDto);
            }
        }
    }
}