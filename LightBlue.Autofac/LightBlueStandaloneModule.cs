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
            if (LightBlueConfiguration.DetermineEnvironment() != AzureEnvironment.LightBlue)
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

            builder.RegisterInstance(LightBlueContext.AzureSettings)
                .As<IAzureSettings>();

            builder.RegisterInstance(LightBlueContext.AzureLocalResources)
                .As<IAzureLocalResourceSource>();

            builder.RegisterType<StandaloneAzureRoleInformation>()
                .WithParameter("roleName", LightBlueConfiguration.StandaloneConfiguration.RoleName)
                .As<IAzureRoleInformation>();

            builder.RegisterInstance((Func<string, IAzureStorage>)(connectionString => new StandaloneAzureStorage(connectionString)))
                .As<Func<string, IAzureStorage>>();

            builder.RegisterInstance((Func<Uri, IAzureBlockBlob>)(blobUri => new StandaloneAzureBlockBlob(blobUri)))
                .As<Func<Uri, IAzureBlockBlob>>();

            builder.RegisterInstance(LightBlueContext.AzureBlobContainerFactory)
                .As<Func<Uri, IAzureBlobContainer>>();
        }
    }
}