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
        }
    }
}