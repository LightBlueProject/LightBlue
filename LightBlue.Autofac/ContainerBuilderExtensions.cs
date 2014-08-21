using System;

using Autofac;

using LightBlue.Setup;

namespace LightBlue.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterLightBlueModules(this ContainerBuilder builder)
        {
            var azureEnvironment = LightBlueConfiguration.DetermineEnvironment();

            if (azureEnvironment == AzureEnvironment.LightBlue)
            {
                builder.RegisterModule<LightBlueStandaloneModule>();
                return;
            }

            if (azureEnvironment == AzureEnvironment.ActualAzure || azureEnvironment == AzureEnvironment.Emulator)
            {
                builder.RegisterModule<LightBlueHostedModule>();
            }

            throw new InvalidOperationException("Cannot register LightBlue when not hosted in the LightBlue, actual Azure or the Azure emulator.");
        }
    }
}