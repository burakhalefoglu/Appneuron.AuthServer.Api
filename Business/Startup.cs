using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using Autofac;
using Business.Abstract;
using Business.Constants;
using Business.DependencyResolvers;
using Business.Fakes.DArch;
using Business.MessageBrokers;
using Business.MessageBrokers.Manager;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Caching.Microsoft;
using Core.DependencyResolvers;
using Core.Extensions;
using Core.Utilities.ElasticSearch;
using Core.Utilities.IoC;
using Core.Utilities.MessageBrokers.Kafka;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra;
using DataAccess.Concrete.Cassandra.Contexts;
using DataAccess.Concrete.Cassandra.Tables;
using DataAccess.Concrete.EntityFramework.Contexts;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Business
{
    public class BusinessStartup
    {
        private readonly IHostEnvironment _hostEnvironment;

        public BusinessStartup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        protected IConfiguration Configuration { get; }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <remarks>
        ///     It is common to all configurations and must be called. Aspnet core does not call this method because there are
        ///     other methods.
        /// </remarks>
        /// <param name="services"></param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            Func<IServiceProvider, ClaimsPrincipal> getPrincipal = (sp) =>
                sp.GetService<IHttpContextAccessor>().HttpContext?.User ??
                new ClaimsPrincipal(new ClaimsIdentity(Messages.Unknown));

            services.AddScoped<IPrincipal>(getPrincipal);
            services.AddMemoryCache();

            services.AddDependencyResolvers(Configuration, new ICoreModule[]
            {
                new CoreModule()
            });

            services.AddSingleton<ConfigurationManager>();
            services.AddTransient<ITokenHelper, JwtHelper>();

            services.AddTransient<IElasticSearch, ElasticSearchManager>();
            services.AddSingleton<ICacheManager, MemoryCacheManager>();
            services.AddSingleton<IMessageBroker, KafkaMessageBroker>();

            services.AddSingleton<IGetCreateProjectMessageService, GetCreateProjectMessageManager>();

            services.AddAutoMapper(typeof(ConfigurationManager));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(typeof(BusinessStartup).GetTypeInfo().Assembly);

            ValidatorOptions.Global.DisplayNameResolver =
             (type, memberInfo, expression) => memberInfo.GetCustomAttribute<DisplayAttribute>()?.GetName();
        }

        /// <summary>
        ///     This method gets called by the Development
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            ConfigureServices(services);
            services.AddScoped<CassandraContextBase, CassandraContext>();

            services.AddScoped<IClientRepository>(x => new CassClientRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Client));
            
            services.AddScoped<IUserProjectRepository>(x => new CassUserProjectRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.UserProject));
            
            services.AddScoped<ILogRepository>(x => new CassLogRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Log));
            
            services.AddScoped<ITranslateRepository>(x => new CassTranslateRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Translate));
            
            services.AddScoped<ILanguageRepository>(x => new CassLanguageRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Language));
            
            services.AddScoped<IUserRepository>(x => new CassUserRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.User));
            
            services.AddScoped<IUserClaimRepository>(x => new CassUserClaimRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.UserClaim));
            
            services.AddScoped<IOperationClaimRepository>(x => new CassOperationClaimRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.OperationClaim));
            
            services.AddScoped<IGroupRepository>(x => new CassGroupRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Group));
            
            services.AddScoped<IGroupClaimRepository>(x => new CassGroupClaimRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.GroupClaim));
            
            services.AddScoped<IUserGroupRepository>(x => new CassUserGroupRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.UserGroup));
            
            services.AddDbContext<ProjectDbContext, DArchInMemory>(ServiceLifetime.Transient);

            
        }

        /// <summary>
        ///     This method gets called by the Staging
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureStagingServices(IServiceCollection services)
        {
            ConfigureServices(services);
            services.AddSingleton<CassandraContextBase, CassandraContext>();

            services.AddScoped<IClientRepository>(x => new CassClientRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Client));
            
            services.AddScoped<IUserProjectRepository>(x => new CassUserProjectRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.UserProject));
            
            services.AddScoped<ILogRepository>(x => new CassLogRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Log));
            
            services.AddScoped<ITranslateRepository>(x => new CassTranslateRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Translate));
            
            services.AddScoped<ILanguageRepository>(x => new CassLanguageRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Language));
            
            services.AddScoped<IUserRepository>(x => new CassUserRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.User));
            
            services.AddScoped<IUserClaimRepository>(x => new CassUserClaimRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.UserClaim));
            
            services.AddScoped<IOperationClaimRepository>(x => new CassOperationClaimRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.OperationClaim));
            
            services.AddScoped<IGroupRepository>(x => new CassGroupRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Group));
            
            services.AddScoped<IGroupClaimRepository>(x => new CassGroupClaimRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.GroupClaim));
            
            services.AddScoped<IUserGroupRepository>(x => new CassUserGroupRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.UserGroup));
            
            // services.AddDbContext<ProjectDbContext>();

        }

        /// <summary>
        ///     This method gets called by the Production
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureProductionServices(IServiceCollection services)
        {
            ConfigureServices(services);
            services.AddSingleton<CassandraContextBase, CassandraContext>();            

             services.AddScoped<IClientRepository>(x => new CassClientRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Client));
            
            services.AddScoped<IUserProjectRepository>(x => new CassUserProjectRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.UserProject));
            
            services.AddScoped<ILogRepository>(x => new CassLogRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Log));
            
            services.AddScoped<ITranslateRepository>(x => new CassTranslateRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Translate));
            
            services.AddScoped<ILanguageRepository>(x => new CassLanguageRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Language));
            
            services.AddScoped<IUserRepository>(x => new CassUserRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.User));
            
            services.AddScoped<IUserClaimRepository>(x => new CassUserClaimRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.UserClaim));
            
            services.AddScoped<IOperationClaimRepository>(x => new CassOperationClaimRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.OperationClaim));
            
            services.AddScoped<IGroupRepository>(x => new CassGroupRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Group));
            
            services.AddScoped<IGroupClaimRepository>(x => new CassGroupClaimRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.GroupClaim));
            
            services.AddScoped<IUserGroupRepository>(x => new CassUserGroupRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.UserGroup));
            
            services.AddDbContext<ProjectDbContext, DArchInMemory>(ServiceLifetime.Transient);

            // services.AddDbContext<ProjectDbContext>();

        }

        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacBusinessModule(new ConfigurationManager(Configuration, _hostEnvironment)));
        }
    }
}