using System.Reflection;
using Core.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Security.Principal;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Microsoft.AspNetCore.Http;
using Business.Constants;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Business.DependencyResolvers
{
    public class BusinessModule : IDIModule
    {
        public void Load(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ConsoleLogger>();
            
            Func<IServiceProvider, ClaimsPrincipal> getPrincipal = (sp) =>
                sp.GetService<IHttpContextAccessor>().HttpContext?.User ??
                new ClaimsPrincipal(new ClaimsIdentity(Messages.Unknown));

            services.AddScoped<IPrincipal>(getPrincipal);
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            ValidatorOptions.Global.DisplayNameResolver =
                (type, memberInfo, expression) => memberInfo.GetCustomAttribute<DisplayAttribute>()?.GetName();
        }
    }
}