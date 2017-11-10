using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LightBlue.Standalone;


namespace LightBlue.Windsor
{
    public class LightBlueStandaloneInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (LightBlueContext.AzureEnvironment != AzureEnvironment.LightBlue)
            {
                throw new InvalidOperationException("Can only use the LightBlue Standalone installer when running in a LightBlue host.");
            }

            container.Register(
                Component.For<IAzureEnvironmentSource>()
                    .Instance(new AzureEnvironmentSource(AzureEnvironment.LightBlue)));

            container.Register(
                Component.For<IAzureRoleInformation>()
                    .ImplementedBy<StandaloneAzureRoleInformation>()
                    .DependsOn(Dependency.OnValue("roleName", LightBlueContext.RoleName)));
        }
    }
}