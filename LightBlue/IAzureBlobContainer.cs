using System;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue
{
    public interface IAzureBlobContainer
    {
        Uri Uri { get; }
        bool CreateIfNotExists(BlobContainerPublicAccessType accessType);
        Task<bool> CreateIfNotExistsAsync(BlobContainerPublicAccessType accessType);
        bool Exists();
        Task<bool> ExistsAsync();
        IAzureBlockBlob GetBlockBlobReference(string blobName);
        string GetSharedAccessSignature(SharedAccessBlobPolicy policy);
        Task<IAzureBlobResultSegment> ListBlobsSegmentedAsync(
            string prefix,
            BlobListing blobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken currentToken);
    }
}