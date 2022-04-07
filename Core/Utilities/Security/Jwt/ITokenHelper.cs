using Core.Entities.ClaimModels;

namespace Core.Utilities.Security.Jwt;

public interface ITokenHelper
{
    TAccessToken CreateCustomerToken<TAccessToken>(UserClaimModel user)
        where TAccessToken : IAccessToken, new();

    TAccessToken CreateClientToken<TAccessToken>(ClientClaimModel clientClaimModel)
        where TAccessToken : IAccessToken, new();
}