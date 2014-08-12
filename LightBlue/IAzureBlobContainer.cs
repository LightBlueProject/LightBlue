using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue
{
    public interface IAzureBlobContainer
    {
        bool CreateIfNotExists(BlobContainerPermissions blobContainerPermissions);
        bool Exists();
        IAzureBlockBlob GetBlockBlob(string blobName);
        Task<IAzureBlobResultSegment> ListBlobsSegmentedAsync(
            string prefix,
            BlobListing blobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken currentToken);
    }
}