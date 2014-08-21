using System;

using Autofac;

using LightBlue.Hosted;
using LightBlue.Setup;

namespace LightBlue.Autofac
{
    public class LightBlueHostedModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var azureEnvironment = LightBlueConfiguration.DetermineEnvironment();
            if (azureEnvironment != AzureEnvironment.ActualAzure && azureEnvironment != AzureEnvironment.Emulator)
            {
                throw new InvalidOperationException("Can only use the LightBlue Hosted module when running in actual Azure or the Azure emulator.");
            }

            builder.RegisterAssemblyTypes(typeof(AzureEnvironment).Assembly)
                .InNamespace(typeof(HostedAzureBlobContainer).Namespace)
                .AsImplementedInterfaces();

            builder.RegisterInstance(new AzureEnvironmentSource(azureEnvironment))
                .As<IAzureEnvironmentSource>();

            builder.RegisterInstance((Func<string, IAzureStorage>) (connectionString => new HostedAzureStorage(connectionString)))
                .As<Func<string, IAzureStorage>>();

            builder.RegisterInstance((Func<Uri, IAzureBlockBlob>) (blobUri => new HostedAzureBlockBlob(blobUri)))
                .As<Func<Uri, IAzureBlockBlob>>();

            builder.RegisterInstance(LightBlueContext.AzureBlobContainerFactory)
                .As<Func<Uri, IAzureBlobContainer>>();
        }
    }
}