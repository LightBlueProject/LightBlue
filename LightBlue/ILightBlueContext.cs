using System;
using Azure;
using Azure.Storage;

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
        IAzureBlockBlob GetBlockBlob(Uri blobUri, AzureSasCredential storageCredentials);
        IAzureQueue GetQueue(Uri queueUri);
    }
}