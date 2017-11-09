using System.Reflection;
using LightBlue.Setup;

namespace LightBlue.Infrastructure
{
    public class HostRunner
    {
        public void Run(
            string workerRoleAssembly,
            string configurationPath,
            string roleName,
            bool useHostedStorage)
        {
            LightBlueConfiguration.SetAsLightBlue(
                configurationPath: configurationPath,
                roleName: roleName,
                lightBlueHostType: LightBlueHostType.Direct,
                useHostedStorage: useHostedStorage);

            var assembly = Assembly.LoadFrom(workerRoleAssembly);
            assembly.EntryPoint.Invoke(null, new object[] { null });
        }
    }
}