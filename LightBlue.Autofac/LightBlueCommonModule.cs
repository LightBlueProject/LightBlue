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

            builder.RegisterInstance((Func<string, IAzureStorage>)LightBlueContext.GetStorageAccount)
                .As<Func<string, IAzureStorage>>();

            builder.RegisterInstance((Func<Uri, IAzureBlobContainer>) LightBlueContext.GetBlobContainer)
                .As<Func<Uri, IAzureBlobContainer>>();

            builder.RegisterInstance((Func<Uri, StorageCredentials, IAzureBlobContainer>) LightBlueContext.GetBlobContainer)
                .As<Func<Uri, StorageCredentials, IAzureBlobContainer>>();

            builder.RegisterInstance((Func<Uri, IAzureBlockBlob>) LightBlueContext.GetBlockBlob)
                .As<Func<Uri, IAzureBlockBlob>>();

            builder.RegisterInstance((Func<Uri, StorageCredentials, IAzureBlockBlob>) LightBlueContext.GetBlockBlob)
                .As<Func<Uri, StorageCredentials, IAzureBlockBlob>>();

            builder.RegisterInstance((Func<Uri, IAzureQueue>) LightBlueContext.GetQueue)
                .As<Func<Uri, IAzureQueue>>();
        }
    }
}