using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Hosted
{
    public class HostedAzureBlobContainer : IAzureBlobContainer
    {
        private readonly CloudBlobContainer _cloudBlobContainer;

        public HostedAzureBlobContainer(CloudBlobContainer cloudBlobContainer)
        {
            _cloudBlobContainer = cloudBlobContainer;
        }

        public bool CreateIfNotExists(BlobContainerPermissions blobContainerPermissions)
        {
            if (!_cloudBlobContainer.CreateIfNotExists())
            {
                return false;
            }

            _cloudBlobContainer.SetPermissions(blobContainerPermissions);

            return true;
        }

        public bool Exists()
        {
            return _cloudBlobContainer.Exists();
        }

        public IAzureBlockBlob GetBlockBlob(string blobName)
        {
            return new HostedAzureBlockBlob(_cloudBlobContainer.GetBlockBlobReference(blobName));
        }

        public async Task<IAzureBlobResultSegment> ListBlobsSegmentedAsync(string prefix, BlobListing blobListing, BlobListingDetails blobListingDetails, int? maxResults, BlobContinuationToken currentToken)
        {
            return new HostedAzureBlobResultSegment(await _cloudBlobContainer.ListBlobsSegmentedAsync(
                prefix,
                blobListing == BlobListing.Flat,
                blobListingDetails,
                maxResults,
                currentToken,
                null,
                null));
        }
    }
}