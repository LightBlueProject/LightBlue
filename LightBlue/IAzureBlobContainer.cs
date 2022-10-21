using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace LightBlue
{
    public interface IAzureBlobContainer
    {
        Uri Uri { get; }
        bool CreateIfNotExists(PublicAccessType accessType);
        Task<bool> CreateIfNotExistsAsync(PublicAccessType accessType);
        bool Exists();
        Task<bool> ExistsAsync();
        IAzureBlockBlob GetBlockBlobReference(string blobName);
        string GetSharedAccessSignature(BlobContainerSasPermissions permissions, DateTimeOffset expiresOn);
        Task<IAzureListBlobItem[]> GetBlobs(string prefix, BlobTraits blobTraits = BlobTraits.None, BlobStates blobStates = BlobStates.None, int maxResults = int.MaxValue);
    }
}