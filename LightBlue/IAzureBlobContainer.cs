using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue
{
    public interface IAzureBlobContainer
    {
        bool CreateIfNotExists(BlobContainerPublicAccessType accessType);
        bool Exists();
        Task ExistsAsynx();
        IAzureBlockBlob GetBlockBlobReference(string blobName);
        Task<IAzureBlobResultSegment> ListBlobsSegmentedAsync(
            string prefix,
            BlobListing blobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken currentToken);
    }
}