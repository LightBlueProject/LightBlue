using System;

using Autofac;

using LightBlue.Setup;
using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;

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

            builder.RegisterInstance(LightBlueConfiguration.RoleEnvironmentExceptionCreator)
                .SingleInstance()
                .As<Func<string, RoleEnvironmentException>>();

            builder.RegisterType<StandaloneAzureRoleInformation>()
                .WithParameter("roleName", LightBlueContext.RoleName)
                .As<IAzureRoleInformation>();
        }
    }
}