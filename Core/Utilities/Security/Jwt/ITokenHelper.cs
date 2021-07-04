using Core.Entities.ClaimModels;
using System.Collections.Generic;

namespace Core.Utilities.Security.Jwt
{
    public interface ITokenHelper
    {
        TAccessToken CreateCustomerToken<TAccessToken>(UserClaimModel user, List<string> ProjectIdList)
          where TAccessToken : IAccessToken, new();

        TAccessToken CreateClientToken<TAccessToken>(ClientClaimModel clientClaimModel)
               where TAccessToken : IAccessToken, new();

    }
}