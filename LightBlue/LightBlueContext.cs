using System;

using LightBlue.External;
using LightBlue.Hosted;
using LightBlue.Setup;
using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Auth;

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
        private static Func<Uri, StorageCredentials, IAzureBlobContainer> _azureBlobContainerWithCredentialsFactory;
        private static Func<Uri, IAzureBlockBlob> _azureBlockBlobFactory;
        private static Func<Uri, StorageCredentials, IAzureBlockBlob> _azureBlockBlobWithCredentialsFactory;
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

        public static AzureEnvironment AzureEnvironment
        {
            get
            {
                Initialise();
                return _azureEnvironment;
            }
        }

        public static IAzureBlobContainer AzureBlobContainerFactory(Uri containerUri)
        {
            Initialise();
            return _azureBlobContainerFactory(containerUri);
        }

        public static IAzureBlobContainer AzureBlobContainerFactory(Uri containerUri, StorageCredentials storageCredentials)
        {
            Initialise();
            return _azureBlobContainerWithCredentialsFactory(containerUri, storageCredentials);
        }

        public static IAzureBlockBlob AzureBlockBlobFactory(Uri blobUri)
        {
            Initialise();
            return _azureBlockBlobFactory(blobUri);
        }

        public static IAzureBlockBlob AzureBlockBlobFactory(Uri blobUri, StorageCredentials storageCredentials)
        {
            Initialise();
            return _azureBlockBlobWithCredentialsFactory(blobUri, storageCredentials);
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

            if (environmentDefinition.StandaloneConfiguration.UseHostedStorage)
            {
                ConfigureHostedStorage();
            }
            else
            {
                ConfigureStandaloneStorage();
            }
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

            ConfigureHostedStorage();
        }

        private static void ConfigureStandaloneStorage()
        {
            _azureStorageFactory = connectionString => new StandaloneAzureStorage(connectionString);
            _azureBlobContainerFactory = uri => new StandaloneAzureBlobContainer(uri);
            _azureBlobContainerWithCredentialsFactory = (uri, credentials) => new StandaloneAzureBlobContainer(uri);
            _azureBlockBlobFactory = blobUri =>
            {
                var locationParts = StandaloneEnvironment.SeparateBlobUri(blobUri);
                return new StandaloneAzureBlockBlob(locationParts.ContainerPath, locationParts.BlobPath);
            };
            _azureBlockBlobWithCredentialsFactory = (blobUri, credentials) =>
            {
                var locationParts = StandaloneEnvironment.SeparateBlobUri(blobUri);
                return new StandaloneAzureBlockBlob(locationParts.ContainerPath, locationParts.BlobPath);
            };
        }

        private static void ConfigureHostedStorage()
        {
            _azureStorageFactory = connectionString => new HostedAzureStorage(connectionString);
            _azureBlobContainerFactory = uri => new HostedAzureBlobContainer(uri);
            _azureBlobContainerWithCredentialsFactory = (uri, credentials) => new HostedAzureBlobContainer(uri, credentials);
            _azureBlockBlobFactory = blobUri => new HostedAzureBlockBlob(blobUri);
            _azureBlockBlobWithCredentialsFactory = (uri, credentials) => new HostedAzureBlockBlob(uri, credentials);
        }

        private static void ConfigureElementsNotAvailableExternalToHost()
        {
            _roleName = "External";
            _azureSettings = new ExternalAzureSettings();
            _azureLocalResources = new ExternalAzureLocalResourceSource();
        }
    }
}