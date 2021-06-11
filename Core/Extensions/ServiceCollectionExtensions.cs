using Core.Utilities.IoC;
using Core.Utilities.MessageBrokers.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDependencyResolvers(this IServiceCollection services, IConfiguration configuration, ICoreModule[] modules)
        {
            foreach (var module in modules)
            {
                module.Load(services, configuration);
            }
        }

    }



}