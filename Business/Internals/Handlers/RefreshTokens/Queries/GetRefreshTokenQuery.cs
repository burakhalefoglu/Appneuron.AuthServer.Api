using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Internals.Handlers.RefreshTokens.Queries;

public class GetRefreshTokenQuery : IRequest<IDataResult<RefreshToken>>
{
    public long UserId { get; set; }

    public class GetRefreshTokenQueryHandler : IRequestHandler<GetRefreshTokenQuery, IDataResult<RefreshToken>>
    {
        private readonly IRefreshTokenrepository _refreshTokenRepository;

        public GetRefreshTokenQueryHandler(IRefreshTokenrepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IDataResult<RefreshToken>> Handle(GetRefreshTokenQuery request,
            CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenRepository
                .GetAsync(p => p.UserId == request.UserId);
            Console.WriteLine("refrestoken: :" + refreshToken);
            return new SuccessDataResult<RefreshToken>(refreshToken);
        }
    }
}