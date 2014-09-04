using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.OData;
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

        public Task<bool> ExistsAsynx()
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
                .Where(f => !(f.DirectoryName ?? "").EndsWith(".meta"))
                .Skip(numberToSkip)
                .Take(maxResults.HasValue ? maxResults.Value : Int32.MaxValue)
                .Select(f =>
                    new StandaloneAzureBlockBlob(
                        _containerDirectory,
                        f.FullName.Substring(_containerDirectory.Length + 1)))
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
                .Where(f => !f.Name.EndsWith(".meta"))
                .Select(f => (IAzureListBlobItem) new StandaloneAzureBlobDirectory(f.FullName));
            var files = new DirectoryInfo(_containerDirectory).EnumerateFiles((prefix ?? "") + "*", SearchOption.TopDirectoryOnly)
                .Where(f => !(f.DirectoryName ?? "").EndsWith(".meta"))
                .Select(f =>
                    (IAzureListBlobItem)new StandaloneAzureBlockBlob(
                        _containerDirectory,
                        f.FullName.Substring(_containerDirectory.Length + 1)));

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