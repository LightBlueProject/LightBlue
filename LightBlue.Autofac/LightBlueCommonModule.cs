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

            builder.RegisterInstance((Func<string, IAzureStorage>)LightBlueContext.GetStorageAccount)
                .As<Func<string, IAzureStorage>>();

            builder.RegisterInstance((Func<Uri, IAzureBlobContainer>)LightBlueContext.GetBlobContainer)
                .As<Func<Uri, IAzureBlobContainer>>();

            builder.RegisterInstance((Func<Uri, string, string, IAzureBlobContainer>)LightBlueContext.GetBlobContainer)
                .As<Func<Uri, string, string, IAzureBlobContainer>>();

            builder.RegisterInstance((Func<Uri, IAzureBlockBlob>)LightBlueContext.GetBlockBlob)
                .As<Func<Uri, IAzureBlockBlob>>();

            builder.RegisterInstance((Func<Uri, string, string, IAzureBlockBlob>)LightBlueContext.GetBlockBlob)
                .As<Func<Uri, string, string, IAzureBlockBlob>>();

            builder.RegisterInstance((Func<Uri, string, IAzureBlockBlob>)LightBlueContext.GetBlockBlob)
                .As<Func<Uri, string, IAzureBlockBlob>>();

            builder.RegisterInstance((Func<Uri, IAzureQueue>)LightBlueContext.GetQueue)
                .As<Func<Uri, IAzureQueue>>();
        }
    }
}