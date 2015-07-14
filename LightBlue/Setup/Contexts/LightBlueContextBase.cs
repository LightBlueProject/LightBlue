using System;
using LightBlue.Standalone;
using Microsoft.WindowsAzure.Storage.Auth;

namespace LightBlue.Setup.Contexts
{
    abstract class LightBlueContextBase : ILightBlueContext
    {
        public abstract string RoleName { get; }
        public abstract IAzureSettings Settings { get; }
        public abstract IAzureLocalResourceSource LocalResources { get; }

        public AzureEnvironment AzureEnvironment
        {
            get { return AzureEnvironment.LightBlue; }
        }

        public IAzureStorage GetStorageAccount(string connectionString)
        {
            return new StandaloneAzureStorage(connectionString);
        }

        public IAzureBlobContainer GetBlobContainer(Uri containerUri)
        {
            return new StandaloneAzureBlobContainer(containerUri);
        }

        public IAzureBlobContainer GetBlobContainer(Uri containerUri, StorageCredentials storageCredentials)
        {
            return new StandaloneAzureBlobContainer(containerUri);
        }

        public IAzureBlockBlob GetBlockBlob(Uri blobUri)
        {
            var locationParts = StandaloneEnvironment.SeparateBlobUri(blobUri);
            return new StandaloneAzureBlockBlob(locationParts.ContainerPath, locationParts.BlobPath);
        }

        public IAzureBlockBlob GetBlockBlob(Uri blobUri, StorageCredentials storageCredentials)
        {
            var locationParts = StandaloneEnvironment.SeparateBlobUri(blobUri);
            return new StandaloneAzureBlockBlob(locationParts.ContainerPath, locationParts.BlobPath);
        }

        public IAzureQueue GetQueue(Uri queueUri)
        {
            return new StandaloneAzureQueue(queueUri);
        }
    }
}