using System;
using System.Runtime.Remoting.Messaging;
using LightBlue.External;
using LightBlue.Hosted;
using LightBlue.Setup;
using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Auth;

namespace LightBlue
{
    public class LightBlueContextState
    {
        public bool _initialised;
        public string _roleName;
        public IAzureSettings _azureSettings;
        public IAzureLocalResourceSource _azureLocalResources;
        public Func<string, IAzureStorage> _azureStorageFactory;
        public Func<Uri, IAzureBlobContainer> _azureBlobContainerFactory;
        public Func<Uri, StorageCredentials, IAzureBlobContainer> _azureBlobContainerWithCredentialsFactory;
        public Func<Uri, IAzureBlockBlob> _azureBlockBlobFactory;
        public Func<Uri, StorageCredentials, IAzureBlockBlob> _azureBlockBlobWithCredentialsFactory;
        public AzureEnvironment _azureEnvironment;
    }

    public class LightBlueContext
    {
        private static Func<LightBlueContextState> _getConfiguration = LightBlueContextStorageFactory.FromAppDomainContext("LightBlueContextState", () => new LightBlueContextState());

        public static LightBlueContextState Instance
        {
            get
            {
                return _getConfiguration();
            }
        }

        public static string RoleName
        {
            get
            {
                Initialise();
                return Instance._roleName;
            }
        }

        public static IAzureSettings AzureSettings
        {
            get
            {
                Initialise();
                return Instance._azureSettings;
            }
        }

        public static IAzureLocalResourceSource AzureLocalResources
        {
            get
            {
                Initialise();
                return Instance._azureLocalResources;
            }
        }

        public static Func<string, IAzureStorage> AzureStorageFactory
        {
            get
            {
                Initialise();
                return Instance._azureStorageFactory;
            }
        }

        public static AzureEnvironment AzureEnvironment
        {
            get
            {
                Initialise();
                return Instance._azureEnvironment;
            }
        }

        public static IAzureBlobContainer AzureBlobContainerFactory(Uri containerUri)
        {
            Initialise();
            return Instance._azureBlobContainerFactory(containerUri);
        }

        public static IAzureBlobContainer AzureBlobContainerFactory(Uri containerUri, StorageCredentials storageCredentials)
        {
            Initialise();
            return Instance._azureBlobContainerWithCredentialsFactory(containerUri, storageCredentials);
        }

        public static IAzureBlockBlob AzureBlockBlobFactory(Uri blobUri)
        {
            Initialise();
            return Instance._azureBlockBlobFactory(blobUri);
        }

        public static IAzureBlockBlob AzureBlockBlobFactory(Uri blobUri, StorageCredentials storageCredentials)
        {
            Initialise();
            return Instance._azureBlockBlobWithCredentialsFactory(blobUri, storageCredentials);
        }

        private static void Initialise()
        {
            if (Instance._initialised)
            {
                return;
            }

            var environmentDefinition = LightBlueConfiguration.DetermineEnvironmentDefinition();

            Instance._azureEnvironment = environmentDefinition.AzureEnvironment;

            if (Instance._azureEnvironment == AzureEnvironment.LightBlue)
            {
                InitialiseAsLightBlue(environmentDefinition);
            }
            else
            {
                InitialiseAsHosted(environmentDefinition);
            }
            Instance._initialised = true;
        }

        private static void InitialiseAsLightBlue(EnvironmentDefinition environmentDefinition)
        {
            if (environmentDefinition.HostingType == HostingType.Role)
            {
                Instance._roleName = environmentDefinition.StandaloneConfiguration.RoleName;
                Instance._azureSettings = new StandaloneAzureSettings(environmentDefinition.StandaloneConfiguration);
                Instance._azureLocalResources = new StandaloneAzureLocalResourceSource(
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
                Instance._roleName = RoleEnvironment.CurrentRoleInstance.Role.Name;
                Instance._azureSettings = new HostedAzureSettings();
                Instance._azureLocalResources = new HostedAzureLocalResourceSource();
            }
            else
            {
                ConfigureElementsNotAvailableExternalToHost();
            }

            ConfigureHostedStorage();
        }

        private static void ConfigureStandaloneStorage()
        {
            Instance._azureStorageFactory = connectionString => new StandaloneAzureStorage(connectionString);
            Instance._azureBlobContainerFactory = uri => new StandaloneAzureBlobContainer(uri);
            Instance._azureBlobContainerWithCredentialsFactory = (uri, credentials) => new StandaloneAzureBlobContainer(uri);
            Instance._azureBlockBlobFactory = blobUri =>
            {
                var locationParts = StandaloneEnvironment.SeparateBlobUri(blobUri);
                return new StandaloneAzureBlockBlob(locationParts.ContainerPath, locationParts.BlobPath);
            };
            Instance._azureBlockBlobWithCredentialsFactory = (blobUri, credentials) =>
            {
                var locationParts = StandaloneEnvironment.SeparateBlobUri(blobUri);
                return new StandaloneAzureBlockBlob(locationParts.ContainerPath, locationParts.BlobPath);
            };
        }

        private static void ConfigureHostedStorage()
        {
            Instance._azureStorageFactory = connectionString => new HostedAzureStorage(connectionString);
            Instance._azureBlobContainerFactory = uri => new HostedAzureBlobContainer(uri);
            Instance._azureBlobContainerWithCredentialsFactory = (uri, credentials) => new HostedAzureBlobContainer(uri, credentials);
            Instance._azureBlockBlobFactory = blobUri => new HostedAzureBlockBlob(blobUri);
            Instance._azureBlockBlobWithCredentialsFactory = (uri, credentials) => new HostedAzureBlockBlob(uri, credentials);
        }

        private static void ConfigureElementsNotAvailableExternalToHost()
        {
            Instance._roleName = "External";
            Instance._azureSettings = new ExternalAzureSettings();
            Instance._azureLocalResources = new ExternalAzureLocalResourceSource();
        }

        public static void RunAsMultiHost()
        {
            _getConfiguration = LightBlueContextStorageFactory.FromCallContext("LightBlueContextState", () => new LightBlueContextState());
        }
    }
}