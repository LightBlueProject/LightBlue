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
                Component.For<Func<string, IAzureStorage>>()
                    .Instance(LightBlueContext.GetStorageAccount));

            container.Register(
                Component.For<Func<Uri, IAzureBlobContainer>>()
                    .Instance(LightBlueContext.GetBlobContainer));

            container.Register(
                Component.For<Func<Uri, StorageCredentials, IAzureBlobContainer>>()
                    .Instance((x,y) =>LightBlueContext.GetBlobContainer(x,y)));

            container.Register(
                Component.For<Func<Uri, IAzureBlockBlob>>()
                    .Instance(LightBlueContext.GetBlockBlob));

            container.Register(
                Component.For<Func<Uri, StorageCredentials, IAzureBlockBlob>>()
                    .Instance((x,y)=>LightBlueContext.GetBlockBlob(x,y)));

            container.Register(
                Component.For<Func<Uri, IAzureQueue>>()
                    .Instance(LightBlueContext.GetQueue));
        }
    }
}