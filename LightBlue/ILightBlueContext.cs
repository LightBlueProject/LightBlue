using Azure.Storage;
using System;

namespace LightBlue
{
    internal interface ILightBlueContext
    {
        string RoleName { get; }
        IAzureSettings Settings { get; }
        AzureEnvironment AzureEnvironment { get; }
        IAzureStorage GetStorageAccount(string connectionString);
        IAzureBlobContainer GetBlobContainer(Uri containerUri);
        IAzureBlobContainer GetBlobContainer(Uri containerUri, StorageSharedKeyCredential storageCredentials);
        IAzureBlockBlob GetBlockBlob(Uri blobUri);
        IAzureBlockBlob GetBlockBlob(Uri blobUri, StorageSharedKeyCredential storageCredentials);
        IAzureQueue GetQueue(Uri queueUri);
    }
}