using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra;
using DataAccess.Concrete.Cassandra.Contexts;
using Module = Autofac.Module;

namespace Business.DependencyResolvers
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CassandraContext>().As<CassandraContextBase>().SingleInstance();
            builder.RegisterType<CassClientRepository>().As<IClientRepository>().SingleInstance();
            builder.RegisterType<CassUserProjectRepository>().As<IUserProjectRepository>().SingleInstance();
            builder.RegisterType<CassLogRepository>().As<ILogRepository>().SingleInstance();
            builder.RegisterType<CassTranslateRepository>().As<ITranslateRepository>().SingleInstance();
            builder.RegisterType<CassLanguageRepository>().As<ILanguageRepository>().SingleInstance();
            builder.RegisterType<CassUserRepository>().As<IUserRepository>().SingleInstance();
            builder.RegisterType<CassUserClaimRepository>().As<IUserClaimRepository>().SingleInstance();
            builder.RegisterType<CassOperationClaimRepository>().As<IOperationClaimRepository>().SingleInstance();
            builder.RegisterType<CassGroupRepository>().As<IGroupRepository>().SingleInstance();
            builder.RegisterType<CassGroupClaimRepository>().As<IGroupClaimRepository>().SingleInstance();
            builder.RegisterType<CassUserGroupRepository>().As<IUserGroupRepository>().SingleInstance();


            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions
                {
                    Selector = new AspectInterceptorSelector()
                }).SingleInstance().InstancePerDependency();
        }
    }
}
