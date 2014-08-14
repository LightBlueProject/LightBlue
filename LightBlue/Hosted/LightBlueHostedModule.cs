using System;
using System.Reflection;

using Autofac;

namespace LightBlue.Hosted
{
    public class LightBlueHostedModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .InNamespace(typeof(LightBlueHostedModule).Namespace)
                .AsImplementedInterfaces();

            builder.RegisterType<HostedAzureEnvironmentSource>()
                .SingleInstance()
                .As<IAzureEnvironmentSource>();

            builder.RegisterInstance((Func<string, IAzureStorage>) (connectionString => new HostedAzureStorage(connectionString)))
                .As<Func<string, IAzureStorage>>();

            builder.RegisterInstance((Func<Uri, IAzureBlockBlob>) (blobUri => new HostedAzureBlockBlob(blobUri)))
                .As<Func<Uri, IAzureBlockBlob>>();

            builder.RegisterInstance((Func<Uri, IAzureBlobContainer>) (containerUri => new HostedAzureBlobContainer(containerUri)))
                .As<Func<Uri, IAzureBlobContainer>>();
        }
    }
}