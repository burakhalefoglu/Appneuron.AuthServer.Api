using AutoMapper;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.Users.Queries;

public class GetUserQuery : IRequest<IDataResult<UserDto>>
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, IDataResult<UserDto>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public GetUserQueryHandler(IUserRepository userRepository,
            IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        [SecuredOperation(Priority = 1)]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IDataResult<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var email = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type.EndsWith("emailaddress"))?.Value;

            var user = await _userRepository.GetAsync(p => p.Email == email && p.Status == true);
            var userDto = _mapper.Map<UserDto>(user);
            return new SuccessDataResult<UserDto>(userDto);
        }
    }
}