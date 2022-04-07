using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using Business.Constants;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.IoC;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Business.DependencyResolvers;

public class BusinessModule : IDIModule
{
    public void Load(IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<ConsoleLogger>();

        Func<IServiceProvider, ClaimsPrincipal> getPrincipal = sp =>
            sp.GetService<IHttpContextAccessor>().HttpContext?.User ??
            new ClaimsPrincipal(new ClaimsIdentity(Messages.Unknown));

        services.AddScoped<IPrincipal>(getPrincipal);
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        ValidatorOptions.Global.DisplayNameResolver =
            (type, memberInfo, expression) => memberInfo.GetCustomAttribute<DisplayAttribute>()?.GetName();
    }
}