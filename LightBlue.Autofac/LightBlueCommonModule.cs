using System;

using Autofac;

using Microsoft.WindowsAzure.Storage.Auth;

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

            builder.RegisterInstance((Func<Uri, IAzureBlobContainer>) LightBlueContext.AzureBlobContainerFactory)
                .As<Func<Uri, IAzureBlobContainer>>();

            builder.RegisterInstance((Func<Uri, StorageCredentials, IAzureBlobContainer>) LightBlueContext.AzureBlobContainerFactory)
                .As<Func<Uri, StorageCredentials, IAzureBlobContainer>>();

            builder.RegisterInstance((Func<Uri, IAzureBlockBlob>) LightBlueContext.AzureBlockBlobFactory)
                .As<Func<Uri, IAzureBlockBlob>>();

            builder.RegisterInstance((Func<Uri, StorageCredentials, IAzureBlockBlob>) LightBlueContext.AzureBlockBlobFactory)
                .As<Func<Uri, StorageCredentials, IAzureBlockBlob>>();
        }
    }
}