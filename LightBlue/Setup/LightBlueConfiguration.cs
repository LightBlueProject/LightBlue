using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using LightBlue.Infrastructure;
using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Setup
{
    public class LightBlueConfiguration
    {
        public static LightBlueConfiguration Instance
        {
            get {
                return _getConfiguration();
            }
        }

        private static Func<LightBlueConfiguration> _getConfiguration = LightBlueContextStorageFactory.FromAppDomainContext("LightBlueConfiguration", () => new LightBlueConfiguration());

        private EnvironmentDefinition _environmentDefinition;
        private readonly Func<string, RoleEnvironmentException> _roleEnvironmentExceptionCreator
            = RoleEnvironmentExceptionCreatorFactory.BuildRoleEnvironmentExceptionCreator();

        public static Func<string, RoleEnvironmentException> RoleEnvironmentExceptionCreator
        {
            get { return Instance._roleEnvironmentExceptionCreator; }
        }

        public static bool IsInitialised
        {
            get { return Instance._environmentDefinition != null; }
        }

        public static void SetAsExternal(AzureEnvironment azureEnvironment)
        {
            if (IsInitialised)
            {
                if (Instance._environmentDefinition.AzureEnvironment == azureEnvironment)
                {
                    return;
                }

                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "LightBlue has already been initialised for '{0}'. You cannot change the environment it is configured for.",
                        Instance._environmentDefinition.AzureEnvironment));
            }

            Instance._environmentDefinition = new EnvironmentDefinition(
                azureEnvironment,
                HostingType.External,
                new StandaloneConfiguration
                {
                    UseHostedStorage = azureEnvironment != AzureEnvironment.LightBlue
                });
        }

        public static void RunAsMultiHost()
        {
            _getConfiguration = LightBlueContextStorageFactory.FromCallContext("LightBlueConfiguration", () => new LightBlueConfiguration());
            LightBlueContext.RunAsMultiHost();
        }

        public static void SetAsLightBlue(
            string configurationPath,
            string serviceDefinitionPath,
            string roleName,
            LightBlueHostType lightBlueHostType,
            bool useHostedStorage)
        {
            if (IsInitialised)
            {
                throw new InvalidOperationException(
                    "LightBlue has already been initialised and cannot be reconfigured");
            }

            Instance._environmentDefinition = new EnvironmentDefinition(
                AzureEnvironment.LightBlue,
                HostingType.Role,
                new StandaloneConfiguration
                {
                    ConfigurationPath = configurationPath,
                    ServiceDefinitionPath = serviceDefinitionPath,
                    RoleName = roleName,
                    UseHostedStorage = useHostedStorage
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

            return Instance._environmentDefinition;
        }

        private static void LoadDefinitionFromEnvironmentVariablesOrAzureRoleDefinition()
        {
            if (HasLightBlueEnvironmentFlag())
            {
                SetAsLightBlue(
                    configurationPath: Environment.GetEnvironmentVariable("LightBlueConfigurationPath"),
                    serviceDefinitionPath: Environment.GetEnvironmentVariable("LightBlueServiceDefinitionPath"),
                    roleName: Environment.GetEnvironmentVariable("LightBlueRoleName"),
                    lightBlueHostType: LightBlueHostType.Indirect,
                    useHostedStorage: Boolean.Parse(Environment.GetEnvironmentVariable("LightBlueUseHostedStorage")));

                return;
            }

            try
            {
                Instance._environmentDefinition = new EnvironmentDefinition(
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