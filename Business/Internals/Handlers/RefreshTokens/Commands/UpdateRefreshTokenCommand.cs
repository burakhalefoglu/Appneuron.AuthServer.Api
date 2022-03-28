using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using Core.Utilities.Toolkit;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Internals.Handlers.RefreshTokens.Commands;

public class UpdateRefreshTokenCommand: IRequest<IDataResult<string>>
{
    public long UserId { get; set; }

    public class UpdateRefreshTokenCommandHandler : IRequestHandler<UpdateRefreshTokenCommand, IDataResult<string>>
    {
        private readonly IRefreshTokenrepository _refreshTokenRepository;

        public UpdateRefreshTokenCommandHandler(IRefreshTokenrepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        [CacheRemoveAspect("Get")]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IDataResult<string>> Handle(UpdateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var token = RandomHelper.CreateRandomString(32);
            var refreshToken = new RefreshToken
            {
                Value = token,
                UserId = request.UserId
            };
            await _refreshTokenRepository.UpdateAsync(refreshToken);
            return new SuccessDataResult<string>(token,Messages.Added);
        }
    }
}
