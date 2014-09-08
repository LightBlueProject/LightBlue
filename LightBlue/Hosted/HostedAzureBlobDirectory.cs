using System;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Hosted
{
    public class HostedAzureBlobDirectory : IAzureBlobDirectory
    {
        private readonly CloudBlobDirectory _cloudBlobDirectory;

        public HostedAzureBlobDirectory(CloudBlobDirectory cloudBlobDirectory)
        {
            _cloudBlobDirectory = cloudBlobDirectory;
        }

        public Uri Uri
        {
            get { return _cloudBlobDirectory.Uri; }
        }

        public IAzureBlockBlob GetBlockBlobReference(string blobName)
        {
            return new HostedAzureBlockBlob(_cloudBlobDirectory.GetBlockBlobReference(blobName));
        }

        public async Task<IAzureBlobResultSegment> ListBlobsSegmentedAsync(BlobListing blobListing, BlobListingDetails blobListingDetails, int? maxResults, BlobContinuationToken currentToken)
        {
            return new HostedAzureBlobResultSegment(await _cloudBlobDirectory.ListBlobsSegmentedAsync(
                blobListing == BlobListing.Flat,
                blobListingDetails,
                maxResults,
                currentToken,
                null,
                null));
        }
    }
}