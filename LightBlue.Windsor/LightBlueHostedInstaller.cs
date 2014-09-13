using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using LightBlue.Hosted;

namespace LightBlue.Windsor
{
    public class LightBlueHostedInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IAzureEnvironmentSource>()
                    .Instance(new AzureEnvironmentSource(LightBlueContext.AzureEnvironment)));

            container.Register(
                Component.For<IAzureRoleInformation>()
                    .Instance(new HostedAzureRoleInformation()));
        }
    }
}