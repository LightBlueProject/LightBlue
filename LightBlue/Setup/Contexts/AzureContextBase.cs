using System;
using LightBlue.Hosted;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Auth;

namespace LightBlue.Setup.Contexts
{
    abstract class AzureContextBase : ILightBlueContext
    {
        public abstract string RoleName { get; }
        public abstract IAzureSettings Settings { get; }
        public abstract IAzureLocalResourceSource LocalResources { get; }

        public AzureEnvironment AzureEnvironment
        {
            get
            {
                return RoleEnvironment.IsEmulated
                    ? AzureEnvironment.Emulator
                    : AzureEnvironment.ActualAzure;
            }
        }

        public IAzureStorage GetStorageAccount(string connectionString)
        {
            return new HostedAzureStorage(connectionString);
        }

        public IAzureBlobContainer GetBlobContainer(Uri containerUri)
        {
            return new HostedAzureBlobContainer(containerUri);
        }

        public IAzureBlobContainer GetBlobContainer(Uri containerUri, StorageCredentials storageCredentials)
        {
            return new HostedAzureBlobContainer(containerUri, storageCredentials);
        }

        public IAzureBlockBlob GetBlockBlob(Uri blobUri)
        {
            return new HostedAzureBlockBlob(blobUri);
        }

        public IAzureBlockBlob GetBlockBlob(Uri blobUri, StorageCredentials storageCredentials)
        {
            return new HostedAzureBlockBlob(blobUri, storageCredentials);
        }

        public IAzureQueue GetQueue(Uri queueUri)
        {
            return new HostedAzureQueue(queueUri);
        }
    }
}