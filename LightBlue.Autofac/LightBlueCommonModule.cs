using System;
using Autofac;
using Azure.Storage;

namespace LightBlue.Autofac
{
    public class LightBlueCommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(LightBlueContext.AzureSettings)
                .As<IAzureSettings>();

            builder.RegisterInstance((Func<string, IAzureStorage>)LightBlueContext.GetStorageAccount)
                .As<Func<string, IAzureStorage>>();

            builder.RegisterInstance((Func<Uri, IAzureBlobContainer>) LightBlueContext.GetBlobContainer)
                .As<Func<Uri, IAzureBlobContainer>>();

            builder.RegisterInstance((Func<Uri, StorageSharedKeyCredential, IAzureBlobContainer>) LightBlueContext.GetBlobContainer)
                .As<Func<Uri, StorageSharedKeyCredential, IAzureBlobContainer>>();

            builder.RegisterInstance((Func<Uri, IAzureBlockBlob>) LightBlueContext.GetBlockBlob)
                .As<Func<Uri, IAzureBlockBlob>>();

            builder.RegisterInstance((Func<Uri, StorageSharedKeyCredential, IAzureBlockBlob>) LightBlueContext.GetBlockBlob)
                .As<Func<Uri, StorageSharedKeyCredential, IAzureBlockBlob>>();

            builder.RegisterInstance((Func<Uri, IAzureQueue>) LightBlueContext.GetQueue)
                .As<Func<Uri, IAzureQueue>>();
        }
    }
}