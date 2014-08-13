using System;
using System.Globalization;
using System.IO;
using System.Linq;
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

        public bool CreateIfNotExists(BlobContainerPublicAccessType accessType)
        {
            Directory.CreateDirectory(_containerDirectory);
            return true;
        }

        public bool Exists()
        {
            return Directory.Exists(_containerDirectory);
        }

        public Task ExistsAsynx()
        {
            return Task.FromResult(Directory.Exists(_containerDirectory));
        }

        public IAzureBlockBlob GetBlockBlobReference(string blobName)
        {
            return new StandaloneAzureBlockBlob(_containerDirectory, blobName);
        }

        public Task<IAzureBlobResultSegment> ListBlobsSegmentedAsync(string prefix, BlobListing blobListing, BlobListingDetails blobListingDetails, int? maxResults, BlobContinuationToken currentToken)
        {
            if (blobListing == BlobListing.Hierarchical && (blobListingDetails & BlobListingDetails.Snapshots) == BlobListingDetails.Snapshots)
            {
                throw new ArgumentException("Listing snapshots is only supported in flat mode.");
            }

            var numberToSkip = DetermineNumberToSkip(currentToken);

            var resultSegment = blobListing == BlobListing.Flat
                ? FindFilesFlattened(prefix, maxResults, numberToSkip)
                : FindFilesHierarchical(prefix, maxResults, numberToSkip);

            return Task.FromResult((IAzureBlobResultSegment) resultSegment);
        }

        private StandaloneAzureBlobResultSegment FindFilesFlattened(string prefix, int? maxResults, int numberToSkip)
        {
            var files = new DirectoryInfo(_containerDirectory).EnumerateFiles((prefix ?? "") + "*", SearchOption.AllDirectories)
                .Skip(numberToSkip)
                .Take(maxResults.HasValue ? maxResults.Value : Int32.MaxValue)
                .Select(f => new StandaloneAzureBlockBlob(new Uri(f.FullName)))
                .ToList();

            var resultSegment = new StandaloneAzureBlobResultSegment(
                files,
                new BlobContinuationToken
                {
                    NextMarker = DetermineNextMarker(numberToSkip, files.Count)
                });
            return resultSegment;
        }

        private StandaloneAzureBlobResultSegment FindFilesHierarchical(string prefix, int? maxResults, int numberToSkip)
        {
            var directories = new DirectoryInfo(_containerDirectory).EnumerateDirectories((prefix ?? "") + "*", SearchOption.TopDirectoryOnly)
                .Select(f => (IAzureListBlobItem)  new StandaloneAzureBlobDirectory(f.FullName));
            var files = new DirectoryInfo(_containerDirectory).EnumerateFiles((prefix ?? "") + "*", SearchOption.TopDirectoryOnly)
                .Select(f => (IAzureListBlobItem)  new StandaloneAzureBlockBlob(new Uri(f.FullName)));

            var combined = directories.Concat(files)
                .Skip(numberToSkip)
                .Take(maxResults.HasValue ? maxResults.Value : Int32.MaxValue)
                .ToList();

            var resultSegment = new StandaloneAzureBlobResultSegment(
                combined,
                new BlobContinuationToken
                {
                    NextMarker = DetermineNextMarker(numberToSkip, combined.Count)
                });
            return resultSegment;
        }

        private static string DetermineNextMarker(int numberToSkip, int count)
        {
            return (numberToSkip + count).ToString(CultureInfo.InvariantCulture);
        }

        private static int DetermineNumberToSkip(BlobContinuationToken currentToken)
        {
            if (currentToken == null)
            {
                return 0;
            }
            
            int numberToSkip;
            return Int32.TryParse(currentToken.NextMarker, out numberToSkip) ? numberToSkip : 0;
        }
    }
}