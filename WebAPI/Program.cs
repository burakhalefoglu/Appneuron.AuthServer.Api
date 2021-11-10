using System;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Business.MessageBrokers;
using Business.MessageBrokers.Kafka;
using Business.MessageBrokers.Manager;
using Business.MessageBrokers.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebAPI
{
    /// <summary>
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();

            await ConsumerAdapter();
        }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                });
        }

        private static async Task ConsumerAdapter()
        {
            IServiceCollection services = new ServiceCollection();
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var kafka = serviceProvider.GetService<IMessageBroker>();
            var getCreateProjectMessageService = serviceProvider.GetService<IGetCreateProjectMessageService>();
            
            await kafka.GetMessageAsync<ProjectMessageCommand>("ProjectMessageCommand", 
                getCreateProjectMessageService.GetProjectCreationMessageQuery);
        }
    }
}