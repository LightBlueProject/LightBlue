using System;
using Autofac;
using LightBlue.Standalone;

namespace LightBlue.Autofac
{
    public class LightBlueStandaloneModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            if (LightBlueContext.AzureEnvironment != AzureEnvironment.LightBlue)
            {
                throw new InvalidOperationException("Can only use the LightBlue Standalone module when running in a LightBlue host.");
            }

            builder.RegisterAssemblyTypes(typeof(AzureEnvironment).Assembly)
                .InNamespace(typeof(StandaloneAzureBlobContainer).Namespace)
                .AsImplementedInterfaces();

            builder.RegisterInstance(new AzureEnvironmentSource(AzureEnvironment.LightBlue))
                .As<IAzureEnvironmentSource>();

            builder.RegisterType<StandaloneAzureRoleInformation>()
                .WithParameter("roleName", LightBlueContext.RoleName)
                .As<IAzureRoleInformation>();
        }
    }
}