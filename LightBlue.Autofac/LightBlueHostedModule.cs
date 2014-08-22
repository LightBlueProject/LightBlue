using System;

using Autofac;

using LightBlue.Hosted;

namespace LightBlue.Autofac
{
    public class LightBlueHostedModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            if (LightBlueContext.AzureEnvironment != AzureEnvironment.ActualAzure && LightBlueContext.AzureEnvironment != AzureEnvironment.Emulator)
            {
                throw new InvalidOperationException("Can only use the LightBlue Hosted module when running in actual Azure or the Azure emulator.");
            }

            builder.RegisterAssemblyTypes(typeof(AzureEnvironment).Assembly)
                .InNamespace(typeof(HostedAzureBlobContainer).Namespace)
                .AsImplementedInterfaces();

            builder.RegisterInstance(new AzureEnvironmentSource(LightBlueContext.AzureEnvironment))
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