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
                builder.RegisterModule<LightBlueCommonModule>();
                return;
            }

            if (LightBlueContext.AzureEnvironment == AzureEnvironment.Azure)
            {
                builder.RegisterModule<LightBlueHostedModule>();
                builder.RegisterModule<LightBlueCommonModule>();
                return;
            }

            throw new InvalidOperationException("Cannot register LightBlue when not hosted in the LightBlue, actual Azure or the Azure emulator.");
        }
    }
}