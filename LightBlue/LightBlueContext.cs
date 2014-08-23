using System;

using LightBlue.External;
using LightBlue.Hosted;
using LightBlue.Setup;
using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue
{
    public static class LightBlueContext
    {
        private static bool _initialised;
        private static string _roleName;
        private static IAzureSettings _azureSettings;
        private static IAzureLocalResourceSource _azureLocalResources;
        private static Func<string, IAzureStorage> _azureStorageFactory;
        private static Func<Uri, IAzureBlobContainer> _azureBlobContainerFactory;
        private static Func<Uri, IAzureBlockBlob> _azureBlockBlobFactory;
        private static AzureEnvironment _azureEnvironment;

        public static string RoleName
        {
            get
            {
                Initialise();
                return _roleName;
            }
        }

        public static IAzureSettings AzureSettings
        {
            get
            {
                Initialise();
                return _azureSettings;
            }
        }

        public static IAzureLocalResourceSource AzureLocalResources
        {
            get
            {
                Initialise();
                return _azureLocalResources;
            }
        }

        public static Func<string, IAzureStorage> AzureStorageFactory
        {
            get
            {
                Initialise();
                return _azureStorageFactory;
            }
        }

        public static Func<Uri, IAzureBlobContainer> AzureBlobContainerFactory
        {
            get
            {
                Initialise();
                return _azureBlobContainerFactory;
            }
        }

        public static Func<Uri, IAzureBlockBlob> AzureBlockBlobFactory
        {
            get
            {
                Initialise();
                return _azureBlockBlobFactory;
            }
        }

        public static AzureEnvironment AzureEnvironment
        {
            get
            {
                Initialise();
                return _azureEnvironment;
            }
        }

        private static void Initialise()
        {
            if (_initialised)
            {
                return;
            }

            var environmentDefinition = LightBlueConfiguration.DetermineEnvironmentDefinition();

            _azureEnvironment = environmentDefinition.AzureEnvironment;

            if (_azureEnvironment == AzureEnvironment.LightBlue)
            {
                InitialiseAsLightBlue(environmentDefinition);
            }
            else
            {
                InitialiseAsHosted(environmentDefinition);
            }
            _initialised = true;
        }

        private static void InitialiseAsLightBlue(EnvironmentDefinition environmentDefinition)
        {
            if (environmentDefinition.HostingType == HostingType.Role)
            {
                _roleName = environmentDefinition.StandaloneConfiguration.RoleName;
                _azureSettings = new StandaloneAzureSettings(environmentDefinition.StandaloneConfiguration);
                _azureLocalResources = new StandaloneAzureLocalResourceSource(
                    environmentDefinition.StandaloneConfiguration,
                    StandaloneEnvironment.LightBlueDataDirectory);
            }
            else
            {
                ConfigureElementsNotAvailableExternalToHost();
            }

            _azureStorageFactory = connectionString => new StandaloneAzureStorage(connectionString);
            _azureBlobContainerFactory = uri => new StandaloneAzureBlobContainer(uri);
            _azureBlockBlobFactory = blobUri => new StandaloneAzureBlockBlob(blobUri);
        }

        private static void InitialiseAsHosted(EnvironmentDefinition environmentDefinition)
        {
            if (environmentDefinition.HostingType == HostingType.Role)
            {
                _roleName = RoleEnvironment.CurrentRoleInstance.Role.Name;
                _azureSettings = new HostedAzureSettings();
                _azureLocalResources = new HostedAzureLocalResourceSource();
            }
            else
            {
                ConfigureElementsNotAvailableExternalToHost();
            }

            _azureStorageFactory = connectionString => new HostedAzureStorage(connectionString);
            _azureBlobContainerFactory = uri => new HostedAzureBlobContainer(uri);
            _azureBlockBlobFactory = blobUri => new HostedAzureBlockBlob(blobUri);
        }

        private static void ConfigureElementsNotAvailableExternalToHost()
        {
            _roleName = "External";
            _azureSettings = new ExternalAzureSettings();
            _azureLocalResources = new ExternalAzureLocalResourceSource();
        }
    }
}