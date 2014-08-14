using System;

using LightBlue.Hosted;
using LightBlue.Infrastructure;
using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Setup
{
    public static class SetupConfiguration
    {
        private static Func<AzureEnvironment> _azureEnvironmentFunc =
            () => RoleEnvironment.IsEmulated
                ? AzureEnvironment.Emulator
                : AzureEnvironment.ActualAzure;

        private static bool _alreadyConfigured;

        static SetupConfiguration()
        {
            AzureSettings = new HostedAzureSettings();
        }

        public static IAzureSettings AzureSettings { get; private set; }

        public static AzureEnvironment AzureEnvironment
        {
            get { return _azureEnvironmentFunc(); }
        }

        private static bool IsHosted
        {
            get
            {
                bool isHosted;

                if (!Boolean.TryParse(Environment.GetEnvironmentVariable("LightBlueHost"), out isHosted))
                {
                    return false;
                }

                return isHosted;
            }
        }

        public static void SetupForWeb()
        {
            if (!IsHosted || _alreadyConfigured)
            {
                return;
            }

            _alreadyConfigured = true;

            SetAsHosted(
                configurationPath: Environment.GetEnvironmentVariable("LightBlueConfigurationPath"),
                serviceDefinitionPath: Environment.GetEnvironmentVariable("LightBlueServiceDefinitionPath"),
                roleName: Environment.GetEnvironmentVariable("LightBlueRoleName"));
        }

        public static void SetAsHosted(
            string configurationPath,
            string serviceDefinitionPath,
            string roleName)
        {
            _azureEnvironmentFunc = () => AzureEnvironment.LightBlue;

            var roleEnvironmentExceptionCreator = RoleEnvironmentExceptionCreatorFactory.BuildRoleEnvironmentExceptionCreator();

            ModuleSource.ClearModules();
            ModuleSource.AddModule(
                new LightBlueStandaloneModule(new StandaloneConfiguration
                {
                    ConfigurationPath = configurationPath,
                    ServiceDefinitionPath = serviceDefinitionPath,
                    RoleName = roleName
                },
                roleEnvironmentExceptionCreator));

            AzureSettings = new StandaloneAzureSettings(
                configurationPath,
                roleName,
                roleEnvironmentExceptionCreator);
        }
    }
}