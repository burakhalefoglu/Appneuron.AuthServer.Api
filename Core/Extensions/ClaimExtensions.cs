using System.Security.Claims;
using Core.Entities.ClaimModels;

namespace Core.Extensions;

public static class ClaimExtensions
{
    public static void AddEmail(this ICollection<Claim> claims, string email)
    {
        claims.Add(new Claim(ClaimTypes.Email, email));
    }

    public static void AddName(this ICollection<Claim> claims, string name)
    {
        claims.Add(new Claim(ClaimTypes.Name, name));
    }

    public static void AddNameIdentifier(this ICollection<Claim> claims, string nameIdentifier)
    {
        claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));
    }

    public static void AddNameUniqueIdentifier(this ICollection<Claim> claims, string nameUniqueIdentifier)
    {
        claims.Add(new Claim(ClaimTypes.SerialNumber, nameUniqueIdentifier));
    }

    public static void AddRoles(this ICollection<Claim> claims, string[] roles)
    {
        roles.ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));
    }

    public static void AddUniqueKey(this ICollection<Claim> claims, string uniqueKey)
    {
        claims.Add(new Claim(JwtCustomClaimNames.UniqueKey, uniqueKey));
    }

    public static void AddCustomerId(this ICollection<Claim> claims, int customerId)
    {
        claims.Add(new Claim(JwtCustomClaimNames.CustomerId, customerId.ToString()));
    }

    public static void AddProjectId(this ICollection<Claim> claims, string projectId)
    {
        claims.Add(new Claim(JwtCustomClaimNames.ProjectId, projectId));
    }
}