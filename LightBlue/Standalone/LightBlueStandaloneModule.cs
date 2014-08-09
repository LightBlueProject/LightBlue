using System.Reflection;

using Autofac;

namespace LightBlue.Standalone
{
    public class LightBlueStandaloneModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .InNamespace(typeof(LightBlueStandaloneModule).Namespace)
                .AsImplementedInterfaces();

            builder.RegisterType<StandaloneAzureEnvironmentSource>()
                .SingleInstance()
                .As<IAzureEnvironmentSource>();
        }
    }
}