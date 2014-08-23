using System;

using LightBlue.Hosted;
using LightBlue.Setup;
using LightBlue.Standalone;

namespace LightBlue
{
    public static class LightBlueContext
    {
        private static bool _initialised;
        private static IAzureSettings _azureSettings;
        private static IAzureLocalResourceSource _azureLocalResources;
        private static Func<string, IAzureStorage> _azureStorageFactory;
        private static Func<Uri, IAzureBlobContainer> _azureBlobContainerFactory;
        private static Func<Uri, IAzureBlockBlob> _azureBlockBlobFactory;
        private static AzureEnvironment _azureEnvironment;

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

            _azureEnvironment = LightBlueConfiguration.DetermineEnvironment();

            if (_azureEnvironment == AzureEnvironment.LightBlue)
            {
                InitialiseAsLightBlue();
            }
            else
            {
                InitialiseAsHosted();
            }
            _initialised = true;
        }

        private static void InitialiseAsLightBlue()
        {
            _azureSettings = new StandaloneAzureSettings(LightBlueConfiguration.StandaloneConfiguration);
            _azureLocalResources = new StandaloneAzureLocalResourceSource(LightBlueConfiguration.StandaloneConfiguration, StandaloneEnvironment.LightBlueDataDirectory);
            _azureStorageFactory = connectionString => new StandaloneAzureStorage(connectionString);
            _azureBlobContainerFactory = uri => new StandaloneAzureBlobContainer(uri);
            _azureBlockBlobFactory = blobUri => new StandaloneAzureBlockBlob(blobUri);
        }

        private static void InitialiseAsHosted()
        {
            _azureSettings = new HostedAzureSettings();
            _azureLocalResources = new HostedAzureLocalResourceSource();
            _azureStorageFactory = connectionString => new HostedAzureStorage(connectionString);
            _azureBlobContainerFactory = uri => new HostedAzureBlobContainer(uri);
            _azureBlockBlobFactory = blobUri => new HostedAzureBlockBlob(blobUri);
        }
    }
}