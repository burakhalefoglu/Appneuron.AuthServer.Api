using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using Core.Utilities.Toolkit;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.RefreshTokens.Commands;

public class CreateRefreshTokenCommand: IRequest<IDataResult<string>>
{
    public long UserId { get; set; }

    public class CreateRefreshTokenCommandHandler : IRequestHandler<CreateRefreshTokenCommand, IDataResult<string>>
    {
        private readonly IRefreshTokenrepository _refreshTokenRepository;

        public CreateRefreshTokenCommandHandler(IRefreshTokenrepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        [SecuredOperation(Priority = 1)]
        [CacheRemoveAspect("Get")]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IDataResult<string>> Handle(CreateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var token = RandomHelper.CreateRandomString(32);
            var refreshToken = new RefreshToken
            {
                Value = token,
                UserId = request.UserId
            };
            await _refreshTokenRepository.AddAsync(refreshToken);
            return new SuccessDataResult<string>(token,Messages.Added);
        }
    }
}
