using System;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Auth;
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

        public HostedAzureBlobContainer(Uri containerUri)
        {
            _cloudBlobContainer = new CloudBlobContainer(containerUri);
        }

        public HostedAzureBlobContainer(Uri containerUri, StorageCredentials credentials)
        {
            _cloudBlobContainer = new CloudBlobContainer(containerUri, credentials);
        }

        public Uri Uri
        {
            get { return _cloudBlobContainer.Uri; }
        }

        public bool CreateIfNotExists(BlobContainerPublicAccessType accessType)
        {
            return _cloudBlobContainer.CreateIfNotExistsAsync(accessType, null, null).GetAwaiter().GetResult();
        }

        public Task<bool> CreateIfNotExistsAsync(BlobContainerPublicAccessType accessType)
        {
            return _cloudBlobContainer.CreateIfNotExistsAsync(accessType, null, null);
        }

        public bool Exists()
        {
            return _cloudBlobContainer.ExistsAsync().GetAwaiter().GetResult();
        }

        public Task<bool> ExistsAsync()
        {
            return _cloudBlobContainer.ExistsAsync();
        }

        public IAzureBlockBlob GetBlockBlobReference(string blobName)
        {
            return new HostedAzureBlockBlob(_cloudBlobContainer.GetBlockBlobReference(blobName));
        }

        public string GetSharedAccessSignature(SharedAccessBlobPolicy policy)
        {
            return _cloudBlobContainer.GetSharedAccessSignature(policy);
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