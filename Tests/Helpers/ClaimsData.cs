using System.Collections.Generic;
using System.Security.Claims;

namespace Tests.Helpers
{
    public static class ClaimsData
    {
        public static List<Claim> GetClaims()
        {
            return new()
            {
                new Claim("username", "deneme"),
                new Claim("email", "test@test.com"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "10")
            };
        }
    }
}