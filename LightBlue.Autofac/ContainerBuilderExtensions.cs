using System;

using Autofac;

namespace LightBlue.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterLightBlueModules(this ContainerBuilder builder)
        {
            if (LightBlueContext.AzureEnvironment == AzureEnvironment.LightBlue)
            {
                builder.RegisterModule<LightBlueStandaloneModule>();
                return;
            }

            if (LightBlueContext.AzureEnvironment == AzureEnvironment.ActualAzure || LightBlueContext.AzureEnvironment == AzureEnvironment.Emulator)
            {
                builder.RegisterModule<LightBlueHostedModule>();
            }

            throw new InvalidOperationException("Cannot register LightBlue when not hosted in the LightBlue, actual Azure or the Azure emulator.");
        }
    }
}