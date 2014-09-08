using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue
{
    public interface IAzureBlobDirectory : IAzureListBlobItem
    {
        IAzureBlockBlob GetBlockBlobReference(string blobName);
        Task<IAzureBlobResultSegment> ListBlobsSegmentedAsync(
            BlobListing blobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken currentToken);

    }
}