using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Standalone
{
    public class StandaloneAzureBlobContainer : IAzureBlobContainer
    {
        private readonly string _containerDirectory;

        public StandaloneAzureBlobContainer(string blobStorageDirectory, string containerName)
        {
            _containerDirectory = Path.Combine(blobStorageDirectory, containerName);
        }

        public StandaloneAzureBlobContainer(string containerDirectory)
        {
            _containerDirectory = containerDirectory;
        }

        public StandaloneAzureBlobContainer(Uri containerUri)
        {
            _containerDirectory = containerUri.RemoveToken();
        }

        private string MetadataDirectory
        {
            get { return Path.Combine(_containerDirectory, ".meta"); }
        }

        public Uri Uri
        {
            get { return new Uri(_containerDirectory); }
        }

        public bool CreateIfNotExists(BlobContainerPublicAccessType accessType)
        {
            Directory.CreateDirectory(_containerDirectory);
            Directory.CreateDirectory(MetadataDirectory);
            return true;
        }

        public Task<bool> CreateIfNotExistsAsync(BlobContainerPublicAccessType accessType)
        {
            CreateIfNotExists(accessType);
            return Task.FromResult(true);
        }

        public bool Exists()
        {
            return Directory.Exists(_containerDirectory);
        }

        public Task<bool> ExistsAsync()
        {
            return Task.FromResult(Directory.Exists(_containerDirectory));
        }

        public IAzureBlockBlob GetBlockBlobReference(string blobName)
        {
            return new StandaloneAzureBlockBlob(_containerDirectory, blobName);
        }

        public string GetSharedAccessSignature(SharedAccessBlobPolicy policy)
        {
            if (policy == null)
            {
                throw new ArgumentNullException("policy");
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "?sv={0:yyyy-MM-dd}&sr=c&sig=s&sp={1}",
                DateTime.Today,
                policy.Permissions.DeterminePermissionsString());
        }

        public Task<IAzureBlobResultSegment> ListBlobsSegmentedAsync(
            string prefix,
            BlobListing blobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken currentToken)
        {
            return Task.FromResult(StandaloneList.ListBlobsSegmentedAsync(
                _containerDirectory,
                null,
                prefix,
                blobListing,
                blobListingDetails,
                maxResults,
                currentToken));
        }
    }
}