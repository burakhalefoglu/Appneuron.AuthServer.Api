using System.Reflection;
using Core.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Security.Principal;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Microsoft.AspNetCore.Http;
using Business.Constants;
using Core.Utilities.ElasticSearch;
using Core.Utilities.MessageBrokers.Kafka;
using Core.Utilities.Security.Jwt;
using Core.CrossCuttingConcerns.Caching.Microsoft;
using Core.CrossCuttingConcerns.Caching;
using Business.MessageBrokers.Manager;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Business.MessageBrokers.Models;
using Confluent.Kafka;
using Core.Utilities.MessageBrokers;
using MassTransit;
using Microsoft.Extensions.Configuration;

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
            
            services.AddMassTransit(x =>
            {
                x.AddRider(rider =>
                {
                    Console.WriteLine("Kafka Listening");
                    //Consumers
                    rider.AddConsumer<GetCreateProjectMessageConsumer>();

                    rider.UsingKafka((context, k) =>
                    {
                        var kafkaOptions = ServiceTool.ServiceProvider.GetService<IConfiguration>()
                            .GetSection("MessageBrokerOptions").Get<MessageBrokerOption>();
                        k.Host($"{kafkaOptions.HostName}:{kafkaOptions.Port}");

                        //TopicEndpoints
                        k.TopicEndpoint<ProjectMessageCommand>("ProjectMessageCommand",
                            "ProjectCreationConsumerGroup", e =>
                        {
                            e.AutoOffsetReset = AutoOffsetReset.Earliest;
                            e.EnablePartitionEof = true;
                            e.PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky;
                            e.AutoStart = true;
                            e.ConfigureConsumer<GetCreateProjectMessageConsumer>(context);
                        });
                        
                    });
                });
            });
        }
    }
}