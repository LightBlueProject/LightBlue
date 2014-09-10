using System;
using System.Globalization;
using System.IO;
using System.Linq;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Standalone
{
    internal static class StandaloneList
    {
        public static IAzureBlobResultSegment ListBlobsSegmentedAsync(
            string containerDirectory,
            string searchDirectory,
            string prefix,
            BlobListing blobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken currentToken)
        {
            if (blobListing == BlobListing.Hierarchical && (blobListingDetails & BlobListingDetails.Snapshots) == BlobListingDetails.Snapshots)
            {
                throw new ArgumentException("Listing snapshots is only supported in flat mode.");
            }

            var numberToSkip = DetermineNumberToSkip(currentToken);

            var resultSegment = blobListing == BlobListing.Flat
                ? FindFilesFlattened(containerDirectory, searchDirectory, prefix, maxResults, numberToSkip)
                : FindFilesHierarchical(containerDirectory, searchDirectory, prefix, maxResults, numberToSkip);

            if ((blobListingDetails & BlobListingDetails.Metadata) == BlobListingDetails.Metadata)
            {
                foreach (var blob in resultSegment.Results.OfType<StandaloneAzureBlockBlob>())
                {
                    blob.FetchAttributes();
                }
            }

            return resultSegment;
        }

        private static StandaloneAzureBlobResultSegment FindFilesFlattened(
            string containerDirectory,
            string searchDirectory,
            string prefix,
            int? maxResults,
            int numberToSkip)
        {
            var files = new DirectoryInfo(searchDirectory ?? containerDirectory).EnumerateFiles((prefix ?? "") + "*", SearchOption.AllDirectories)
                .Where(f => !(f.DirectoryName ?? "").EndsWith(".meta"))
                .Skip(numberToSkip)
                .Take(maxResults.HasValue ? maxResults.Value : Int32.MaxValue)
                .Select(f =>
                    new StandaloneAzureBlockBlob(
                        containerDirectory,
                        f.FullName.Substring(containerDirectory.Length + 1)))
                .ToList();

            var resultSegment = new StandaloneAzureBlobResultSegment(
                files,
                new BlobContinuationToken
                {
                    NextMarker = DetermineNextMarker(numberToSkip, files.Count)
                });
            return resultSegment;
        }

        private static StandaloneAzureBlobResultSegment FindFilesHierarchical(
            string containerDirectory,
            string searchDirectory,
            string prefix,
            int? maxResults,
            int numberToSkip)
        {
            var directories = new DirectoryInfo(searchDirectory ?? containerDirectory).EnumerateDirectories((prefix ?? "") + "*", SearchOption.TopDirectoryOnly)
                .Where(f => !f.Name.EndsWith(".meta"))
                .Select(f => (IAzureListBlobItem)new StandaloneAzureBlobDirectory(containerDirectory, f.FullName.Substring(containerDirectory.Length + 1)));
            var files = new DirectoryInfo(searchDirectory ?? containerDirectory).EnumerateFiles((prefix ?? "") + "*", SearchOption.TopDirectoryOnly)
                .Where(f => !(f.DirectoryName ?? "").EndsWith(".meta"))
                .Select(f =>
                    (IAzureListBlobItem)new StandaloneAzureBlockBlob(
                        containerDirectory,
                        f.FullName.Substring(containerDirectory.Length + 1)));

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