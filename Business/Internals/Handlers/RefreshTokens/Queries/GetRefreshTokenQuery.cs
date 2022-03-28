using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Internals.Handlers.RefreshTokens.Queries;

public class GetRefreshTokenQuery: IRequest<IDataResult<RefreshToken>>
{
    public class GetRefreshTokenQueryHandler : IRequestHandler<GetRefreshTokenQuery, IDataResult<RefreshToken>>
    {
        private readonly IRefreshTokenrepository _refreshTokenRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetRefreshTokenQueryHandler(IRefreshTokenrepository refreshTokenRepository, IHttpContextAccessor httpContextAccessor)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IDataResult<RefreshToken>> Handle(GetRefreshTokenQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;
            
            var refreshToken = await _refreshTokenRepository
                .GetAsync(p => p.UserId == Convert.ToInt64(userId));
            return new SuccessDataResult<RefreshToken>(refreshToken);
        }
    }
}
