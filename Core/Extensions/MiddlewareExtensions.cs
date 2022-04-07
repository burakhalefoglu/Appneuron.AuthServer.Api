using Core.Utilities.Security.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Core.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}