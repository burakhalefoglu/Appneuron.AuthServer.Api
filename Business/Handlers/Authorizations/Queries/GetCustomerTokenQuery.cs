using Business.BusinessAspects;
using Business.Constants;
using Business.Internals.Handlers.GroupClaims;
using Business.Internals.Handlers.RefreshTokens.Queries;
using Business.Internals.Handlers.UserClaims;
using Business.Internals.Handlers.UserGroups.Queries;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.ClaimModels;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.Authorizations.Queries;

public class GetCustomerTokenQuery: IRequest<IDataResult<AccessToken>>
{
    public class GetCustomerTokenQueryHandler : IRequestHandler<GetCustomerTokenQuery, IDataResult<AccessToken>>
    {
        private readonly IMediator _mediator;
        private readonly ITokenHelper _tokenHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCustomerTokenQueryHandler(IMediator mediator, ITokenHelper tokenHelper, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _tokenHelper = tokenHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        [SecuredOperation(Priority = 1)]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IDataResult<AccessToken>> Handle(GetCustomerTokenQuery request,
            CancellationToken cancellationToken)
        {
            
            var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["RefreshToken"];
            var userId = Convert.ToInt64(_httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);
            var name = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type.EndsWith("name"))?.Value;
            var email = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type.EndsWith("emailaddress"))?.Value;
            
            var tokenFromCassandra = await _mediator.Send(new GetRefreshTokenQuery(), cancellationToken);
            if (refreshToken != tokenFromCassandra.Data.Value)
                return new ErrorDataResult<AccessToken>(Messages.AuthorizationsDenied);
            
            var usrGroup = await _mediator.Send(new GetUserGroupInternalQuery
            {
                UserId = userId
            }, cancellationToken);

            var result = await _mediator.Send(new GetGroupClaimInternalQuery
            {
                GroupId = usrGroup.Data.GroupId
            }, cancellationToken);

            var operationClaims = new List<OperationClaim>();

            if (result.Data.ToList().Count > 0)
            {
                var selectionItems = result.Data.ToList();

                operationClaims.AddRange(selectionItems.Select(item => new OperationClaim
                    {Id = item.Id, Name = item.Label}));
            }

            await _mediator.Send(new CreateUserClaimsInternalCommand
            {
                UserId = userId,
                OperationClaims = operationClaims
            }, cancellationToken);
            var accessToken = _tokenHelper.CreateCustomerToken<AccessToken>(new UserClaimModel
            {
                UserId = userId,
                Email = email,
                Name = name,
                OperationClaims = operationClaims.Select(x => x.Name).ToArray()
            });
            return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
        }
    }
}