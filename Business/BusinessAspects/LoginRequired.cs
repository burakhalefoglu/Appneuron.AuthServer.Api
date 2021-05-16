using Business.Constants;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Security;

namespace Business.BusinessAspects
{
    public class LoginRequiredAttribute : MethodInterceptionAttribute
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginRequiredAttribute()
        {
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
        }

        protected override void OnBefore(IInvocation invocation)
        {
            var userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

            if (userId == null) throw new SecurityException(Messages.AuthorizationsDenied);
        }
    }
}