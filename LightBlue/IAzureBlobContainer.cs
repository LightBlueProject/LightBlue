using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace LightBlue
{
    public interface IAzureBlobContainer
    {
        Uri Uri { get; }
        bool CreateIfNotExists();
        Task<bool> CreateIfNotExistsAsync();
        bool Exists();
        Task<bool> ExistsAsync();
        IAzureBlockBlob GetBlockBlobReference(string blobName);
        string GetSharedAccessReadSignature(DateTimeOffset expiresOn);
        string GetSharedAccessWriteSignature(DateTimeOffset expiresOn);
        string GetSharedAccessReadWriteSignature(DateTimeOffset expiresOn);
        Task<IAzureListBlobItem[]> GetBlobs(string prefix, BlobTraits blobTraits = BlobTraits.None, BlobStates blobStates = BlobStates.None, int maxResults = int.MaxValue);
    }
}