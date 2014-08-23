using System;

using Autofac;

namespace LightBlue.Autofac
{
    public class LightBlueCommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(LightBlueContext.AzureSettings)
                .As<IAzureSettings>();

            builder.RegisterInstance(LightBlueContext.AzureLocalResources)
                .As<IAzureLocalResourceSource>();

            builder.RegisterInstance(LightBlueContext.AzureStorageFactory)
                .As<Func<string, IAzureStorage>>();

            builder.RegisterInstance(LightBlueContext.AzureBlobContainerFactory)
                .As<Func<Uri, IAzureBlobContainer>>();

            builder.RegisterInstance(LightBlueContext.AzureBlockBlobFactory)
                .As<Func<Uri, IAzureBlockBlob>>();
        }
    }
}