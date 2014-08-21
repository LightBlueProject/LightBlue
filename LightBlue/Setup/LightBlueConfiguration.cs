using System;
using System.Diagnostics;

using LightBlue.Infrastructure;
using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Setup
{
    public static class LightBlueConfiguration
    {
        private static bool _initialised;
        private static AzureEnvironment _environment;
        private static readonly Func<string, RoleEnvironmentException> _roleEnvironmentExceptionCreator
            = RoleEnvironmentExceptionCreatorFactory.BuildRoleEnvironmentExceptionCreator();

        public static StandaloneConfiguration StandaloneConfiguration { get; private set; }

        public static Func<string, RoleEnvironmentException> RoleEnvironmentExceptionCreator
        {
            get { return _roleEnvironmentExceptionCreator; }
        }

        public static void SetAsLightBlue(
            string configurationPath,
            string serviceDefinitionPath,
            string roleName)
        {
            _initialised = true;
            _environment = AzureEnvironment.LightBlue;

            StandaloneConfiguration = new StandaloneConfiguration
            {
                ConfigurationPath = configurationPath,
                ServiceDefinitionPath = serviceDefinitionPath,
                RoleName = roleName
            };
        }

        public static AzureEnvironment DetermineEnvironment()
        {
            if (_initialised)
            {
                return _environment;
            }

            if (HasLightBlueEnvironmentFlag())
            {
                SetAsLightBlue(
                    configurationPath: Environment.GetEnvironmentVariable("LightBlueConfigurationPath"),
                    serviceDefinitionPath: Environment.GetEnvironmentVariable("LightBlueServiceDefinitionPath"),
                    roleName: Environment.GetEnvironmentVariable("LightBlueRoleName"));

                return _environment;
            }

            _initialised = true;

            try
            {
                _environment = RoleEnvironment.IsEmulated
                    ? AzureEnvironment.Emulator
                    : AzureEnvironment.ActualAzure;
            }
            catch (InvalidOperationException)
            {
                _environment = AzureEnvironment.External;
            }

            return _environment;
        }

        private static bool HasLightBlueEnvironmentFlag()
        {
            bool isInLightBlueHost;

            if (!Boolean.TryParse(Environment.GetEnvironmentVariable("LightBlueHost"), out isInLightBlueHost))
            {
                return false;
            }

            return isInLightBlueHost;
        }
    }
}