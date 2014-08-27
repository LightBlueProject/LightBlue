using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using LightBlue.Infrastructure;
using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Setup
{
    public static class LightBlueConfiguration
    {
        private static EnvironmentDefinition _environmentDefinition;
        private static readonly Func<string, RoleEnvironmentException> _roleEnvironmentExceptionCreator
            = RoleEnvironmentExceptionCreatorFactory.BuildRoleEnvironmentExceptionCreator();

        public static Func<string, RoleEnvironmentException> RoleEnvironmentExceptionCreator
        {
            get { return _roleEnvironmentExceptionCreator; }
        }

        public static bool IsInitialised
        {
            get { return _environmentDefinition != null; }
        }

        public static void SetAsExternal(AzureEnvironment azureEnvironment)
        {
            if (IsInitialised)
            {
                if (_environmentDefinition.AzureEnvironment == azureEnvironment)
                {
                    return;
                }

                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "LightBlue has already been initialised for '{0}'. You cannot change the environment it is configured for.",
                        _environmentDefinition.AzureEnvironment));
            }

            _environmentDefinition = new EnvironmentDefinition(
                azureEnvironment,
                HostingType.External,
                null);
        }

        public static void SetAsLightBlue(
            string configurationPath,
            string serviceDefinitionPath,
            string roleName,
            LightBlueHostType lightBlueHostType)
        {
            if (IsInitialised)
            {
                throw new InvalidOperationException(
                    "LightBlue has already been initialised and cannot be reconfigured");
            }

            _environmentDefinition = new EnvironmentDefinition(
                AzureEnvironment.LightBlue,
                HostingType.Role,
                new StandaloneConfiguration
                {
                    ConfigurationPath = configurationPath,
                    ServiceDefinitionPath = serviceDefinitionPath,
                    RoleName = roleName
                });

            if (lightBlueHostType == LightBlueHostType.Direct)
            {
                var processId = roleName
                    + "-"
                    + Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture);

                var temporaryDirectory = Path.Combine(StandaloneEnvironment.LightBlueDataDirectory, "temp", processId);

                Directory.CreateDirectory(temporaryDirectory);

                Environment.SetEnvironmentVariable("TMP", temporaryDirectory);
                Environment.SetEnvironmentVariable("TEMP", temporaryDirectory);
            }
        }

        internal static EnvironmentDefinition DetermineEnvironmentDefinition()
        {
            if (!IsInitialised)
            {
                LoadDefinitionFromEnvironmentVariablesOrAzureRoleDefinition();
            }

            return _environmentDefinition;
        }

        private static void LoadDefinitionFromEnvironmentVariablesOrAzureRoleDefinition()
        {
            if (HasLightBlueEnvironmentFlag())
            {
                SetAsLightBlue(
                    configurationPath: Environment.GetEnvironmentVariable("LightBlueConfigurationPath"),
                    serviceDefinitionPath: Environment.GetEnvironmentVariable("LightBlueServiceDefinitionPath"),
                    roleName: Environment.GetEnvironmentVariable("LightBlueRoleName"),
                    lightBlueHostType: LightBlueHostType.Indirect);

                return;
            }

            try
            {
                _environmentDefinition = new EnvironmentDefinition(
                    RoleEnvironment.IsEmulated
                        ? AzureEnvironment.Emulator
                        : AzureEnvironment.ActualAzure,
                    HostingType.Role,
                    null);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(
                    "Cannot determine what environment the code is running in. If running externally to Azure, the Azure emulator or a LightBlue host you must manually configure LightBlue by calling LightBlueConfiguration.SetAsExternal.",
                    ex);
            }
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