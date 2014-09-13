using System;

using Castle.Windsor;

namespace LightBlue.Windsor
{
    public static class WindsorContainerExtensions
    {
        public static void InstallLightBlue(this IWindsorContainer container)
        {
            if (LightBlueContext.AzureEnvironment == AzureEnvironment.LightBlue)
            {
                container.Install(
                    new LightBlueStandaloneInstaller(),
                    new LightBlueCommonInstaller());
                return;
            }

            if (LightBlueContext.AzureEnvironment == AzureEnvironment.ActualAzure || LightBlueContext.AzureEnvironment == AzureEnvironment.Emulator)
            {
                container.Install(
                    new LightBlueHostedInstaller(),
                    new LightBlueCommonInstaller());
                return;
            }

            throw new InvalidOperationException("Cannot register LightBlue when not hosted in the LightBlue, actual Azure or the Azure emulator.");
        }
    }
}