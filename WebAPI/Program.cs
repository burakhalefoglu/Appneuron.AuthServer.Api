using System;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Business.MessageBrokers;
using Business.MessageBrokers.Manager;
using Business.MessageBrokers.Models;
using Core.Utilities.IoC;
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
        public static Task Main(string[] args)
        {
            var result = CreateHostBuilder(args).Build().RunAsync();
            var consumer = ConsumerAdapter();
            result.Wait();
            consumer.Wait();
            return Task.CompletedTask;
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
                    webBuilder.UseKestrel(options =>
                    {
                        options.AllowSynchronousIO = true;
                        options.AddServerHeader = true;
                    });

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
            Console.WriteLine("Kafka Listening");
            var messageBroker = ServiceTool.ServiceProvider.GetService<IMessageBroker>();
            var createProjectMessageService = ServiceTool.ServiceProvider.GetService<IGetCreateProjectMessageService>();

            if (messageBroker != null)
                if (createProjectMessageService != null)
                    await messageBroker.GetMessageAsync<ProjectMessageCommand>("ProjectMessageCommand",
                        "ProjectCreationConsumerGroup",
                        createProjectMessageService.GetProjectCreationMessageQuery);
        }
    }
}