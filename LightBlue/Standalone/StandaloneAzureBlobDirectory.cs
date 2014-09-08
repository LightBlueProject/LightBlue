using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Standalone
{
    public class StandaloneAzureBlobDirectory : IAzureBlobDirectory
    {
        private readonly string _containerDirectory;
        private readonly string _directoryPath;

        public StandaloneAzureBlobDirectory(string containerDirectory, string directoryName)
        {
            _containerDirectory = containerDirectory;
            _directoryPath = Path.Combine(_containerDirectory, directoryName);
        }

        public Uri Uri { get { return new Uri(_directoryPath);} }

        public IAzureBlockBlob GetBlockBlobReference(string blobName)
        {
            return new StandaloneAzureBlockBlob(_directoryPath, blobName);
        }

        public Task<IAzureBlobResultSegment> ListBlobsSegmentedAsync(BlobListing blobListing, BlobListingDetails blobListingDetails, int? maxResults, BlobContinuationToken currentToken)
        {
            return Task.FromResult(StandaloneList.ListBlobsSegmentedAsync(
                _containerDirectory,
                _directoryPath,
                "",
                blobListing,
                blobListingDetails,
                maxResults,
                currentToken));
        }
    }
}