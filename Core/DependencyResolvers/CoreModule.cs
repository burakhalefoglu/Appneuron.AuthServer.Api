using System.Diagnostics;
using System.Reflection;
using Core.ApiDoc;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Caching.Microsoft;
using Core.CrossCuttingConcerns.Caching.Redis;
using Core.Utilities.ElasticSearch;
using Core.Utilities.IoC;
using Core.Utilities.Mail;
using Core.Utilities.MessageBrokers;
using Core.Utilities.MessageBrokers.Kafka;
using Core.Utilities.Messages;
using Core.Utilities.Security.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Core.DependencyResolvers
{
    public class CoreModule : IDIModule
    {
        public void Load(IServiceCollection services)
        {
            services.AddMemoryCache();
            // set redis
            services.AddSingleton<ICacheManager, MemoryCacheManager>();
            services.AddSingleton<IMailService, MailManager>();
            services.AddSingleton<IEmailConfiguration, EmailConfiguration>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ITokenHelper, JwtHelper>();
            services.AddSingleton<IMessageBroker, KafkaMessageBroker>();
            
            services.AddSingleton<Stopwatch>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SwaggerMessages.Version, new OpenApiInfo
                {
                    Version = SwaggerMessages.Version,
                    Title = SwaggerMessages.Title,
                    Description = SwaggerMessages.Description,
                    TermsOfService = new Uri(SwaggerMessages.TermsOfService),
                    Contact = new OpenApiContact
                    {
                        Name = SwaggerMessages.ContactName,
                    },
                    License = new OpenApiLicense
                    {
                        Name = SwaggerMessages.LicenceName,
                    },
                });

                c.OperationFilter<AddAuthHeaderOperationFilter>();
                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Description = "`Token only!!!` - without `Bearer_` prefix",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer"
                });
            });
        }
    }
}