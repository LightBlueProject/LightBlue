using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using LightBlue.Infrastructure;
using LightBlue.Setup.Contexts;
using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Setup
{
    public static class LightBlueConfiguration
    {
        private static ILightBlueContext _context;
        private static readonly Func<string, RoleEnvironmentException> _roleEnvironmentExceptionCreator
            = RoleEnvironmentExceptionCreatorFactory.BuildRoleEnvironmentExceptionCreator();

        public static Func<string, RoleEnvironmentException> RoleEnvironmentExceptionCreator
        {
            get { return _roleEnvironmentExceptionCreator; }
        }

        public static bool IsInitialised
        {
            get { return _context != null; }
        }

        public static void SetAsExternal(AzureEnvironment azureEnvironment)
        {
            if (IsInitialised)
            {
                if (_context.AzureEnvironment == azureEnvironment)
                {
                    return;
                }

                var msg = string.Format(CultureInfo.InvariantCulture, "LightBlue has already been initialised for '{0}'. You cannot change the environment it is configured for.", _context.AzureEnvironment);
                throw new InvalidOperationException(msg);
            }

            _context = azureEnvironment == AzureEnvironment.LightBlue
                ? (ILightBlueContext)new ExternalLightBlueContext()
                : (ILightBlueContext)new ExternalAzureContext();
        }

        public static void SetAsMultiHost()
        {
            if (IsInitialised)
            {
                throw new InvalidOperationException("LightBlue has already been initialised and cannot be reconfigured");
            }
            _context = new LightBlueLogicalCallContext();
        }

        public static void SetAsLightBlue(
            string configurationPath,
            string serviceDefinitionPath,
            string roleName,
            LightBlueHostType lightBlueHostType,
            bool useHostedStorage)
        {
            var logicalCallContext = _context as LightBlueLogicalCallContext;
            if (logicalCallContext != null)
            {
                logicalCallContext.InitializeLogicalContext(configurationPath, serviceDefinitionPath, roleName, useHostedStorage);
                return;
            }

            if (IsInitialised)
            {
                throw new InvalidOperationException("LightBlue has already been initialised and cannot be reconfigured");
            }

            _context = new LightBlueAppDomainContext(configurationPath, serviceDefinitionPath, roleName, useHostedStorage);

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

        internal static ILightBlueContext GetConfiguredContext()
        {
            if (!IsInitialised)
            {
                LoadDefinitionFromEnvironmentVariablesOrAzureRoleDefinition();
            }

            return _context;
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
                _context = new AzureContext();
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