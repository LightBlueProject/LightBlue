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

        public bool CreateIfNotExists(BlobContainerPermissions blobContainerPermissions)
        {
            Directory.CreateDirectory(_containerDirectory);
            return true;
        }

        public bool Exists()
        {
            return Directory.Exists(_containerDirectory);
        }

        public IAzureBlockBlob GetBlockBlob(string blobName)
        {
            return new StandaloneAzureBlockBlob(_containerDirectory, blobName);
        }

        public Task<IAzureBlobResultSegment> ListBlobsSegmentedAsync(string prefix, BlobListing blobListing, BlobListingDetails blobListingDetails, int? maxResults, BlobContinuationToken currentToken)
        {
            return null;
        }
    }
}