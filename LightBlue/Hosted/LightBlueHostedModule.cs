using System.Reflection;

using Autofac;

namespace LightBlue.Hosted
{
    public class LightBlueHostedModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .InNamespace(typeof(LightBlueHostedModule).Namespace)
                .AsImplementedInterfaces();

            builder.RegisterType<HostedAzureEnvironmentSource>()
                .SingleInstance()
                .As<IAzureEnvironmentSource>();
        }
    }
}