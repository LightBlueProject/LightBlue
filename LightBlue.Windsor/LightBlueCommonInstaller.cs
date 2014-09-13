using System;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using Microsoft.WindowsAzure.Storage.Auth;

namespace LightBlue.Windsor
{
    public class LightBlueCommonInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IAzureSettings>()
                    .Instance(LightBlueContext.AzureSettings));

            container.Register(
                Component.For<IAzureLocalResourceSource>()
                    .Instance(LightBlueContext.AzureLocalResources));

            container.Register(
                Component.For<Func<string, IAzureStorage>>()
                    .Instance(LightBlueContext.AzureStorageFactory));

            container.Register(
                Component.For<Func<Uri, IAzureBlobContainer>>()
                    .Instance(LightBlueContext.AzureBlobContainerFactory));

            container.Register(
                Component.For<Func<Uri, StorageCredentials, IAzureBlobContainer>>()
                    .Instance(LightBlueContext.AzureBlobContainerFactory));

            container.Register(
                Component.For<Func<Uri, IAzureBlockBlob>>()
                    .Instance(LightBlueContext.AzureBlockBlobFactory));

            container.Register(
                Component.For<Func<Uri, StorageCredentials, IAzureBlockBlob>>()
                    .Instance(LightBlueContext.AzureBlockBlobFactory));
        }
    }
}