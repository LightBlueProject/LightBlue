using System;
using Microsoft.WindowsAzure.Storage.Auth;

namespace LightBlue
{
    internal interface ILightBlueContext
    {
        string RoleName { get; }
        IAzureSettings Settings { get; }
        IAzureLocalResourceSource LocalResources { get; }
        AzureEnvironment AzureEnvironment { get; }
        IAzureStorage GetStorageAccount(string connectionString);
        IAzureBlobContainer GetBlobContainer(Uri containerUri);
        IAzureBlobContainer GetBlobContainer(Uri containerUri, StorageCredentials storageCredentials);
        IAzureBlockBlob GetBlockBlob(Uri blobUri);
        IAzureBlockBlob GetBlockBlob(Uri blobUri, StorageCredentials storageCredentials);
        IAzureQueue GetQueue(Uri queueUri);
    }
}