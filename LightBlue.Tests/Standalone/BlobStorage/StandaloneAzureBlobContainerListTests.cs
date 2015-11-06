using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage.Blob;

using Xunit;

namespace LightBlue.Tests.Standalone.BlobStorage
{
    public class StandaloneAzureBlobContainerListTests : StandaloneAzureTestsBase, IDisposable
    {
        private readonly StandaloneAzureBlobContainer _container;

        public StandaloneAzureBlobContainerListTests()
            : base(DirectoryType.Container)
        {
            _container = new StandaloneAzureBlobContainer(BasePath);
            _container.CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Directory.CreateDirectory(Path.Combine(BasePath, "1a"));
            Directory.CreateDirectory(Path.Combine(BasePath, "1b"));
            Directory.CreateDirectory(Path.Combine(BasePath, "1c"));
            Directory.CreateDirectory(Path.Combine(BasePath, "1d"));
            Directory.CreateDirectory(Path.Combine(BasePath, "1e"));
            Directory.CreateDirectory(Path.Combine(BasePath, "2a"));
            Directory.CreateDirectory(Path.Combine(BasePath, "2b"));
            Directory.CreateDirectory(Path.Combine(BasePath, "2c"));
            Directory.CreateDirectory(Path.Combine(BasePath, "2d"));
            Directory.CreateDirectory(Path.Combine(BasePath, "2e"));

            File.WriteAllText(Path.Combine(BasePath, "1"), "Test File");
            File.WriteAllText(Path.Combine(BasePath, "2"), "Test File");
            File.WriteAllText(Path.Combine(BasePath, "3"), "Test File");
            File.WriteAllText(Path.Combine(BasePath, "4"), "Test File");
            File.WriteAllText(Path.Combine(BasePath, "5"), "Test File");

            File.WriteAllText(Path.Combine(BasePath, "1a", "1"), "Test File");
            File.WriteAllText(Path.Combine(BasePath, "1a", "2"), "Test File");
            File.WriteAllText(Path.Combine(BasePath, "1a", "3"), "Test File");
            File.WriteAllText(Path.Combine(BasePath, "1a", "4"), "Test File");
            File.WriteAllText(Path.Combine(BasePath, "1a", "5"), "Test File");
        }

        [Theory]
        [InlineData(BlobListingDetails.Snapshots)]
        [InlineData(BlobListingDetails.All)]
        public async Task AllowsSnapshotsOnlyInFlatMode(BlobListingDetails blobListingDetails)
        {
            var container = new StandaloneAzureBlobContainer(BasePath);
            await Assert.ThrowsAsync<ArgumentException>(() => container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Hierarchical,
                blobListingDetails,
                500,
                null));
        }

        [Fact]
        public async Task WillGetExpectedNumberOfResultsForFlatListing()
        {
            var results = await _container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Flat,
                BlobListingDetails.None,
                null,
                null);

            Assert.Equal(10, results.Results.Count());
        }

        [Fact]
        public async Task FlatListingExcludesDirectories()
        {
            var results = await _container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Flat,
                BlobListingDetails.None,
                null,
                null);

            Assert.Empty(results.Results.OfType<IAzureBlobDirectory>());
        }

        [Theory]
        [InlineData("1", "")]
        [InlineData("2", "")]
        [InlineData("3", "")]
        [InlineData("4", "")]
        [InlineData("5", "")]
        [InlineData("1", "1a")]
        [InlineData("2", "1a")]
        [InlineData("3", "1a")]
        [InlineData("4", "1a")]
        [InlineData("5", "1a")]
        public async Task FlatListingContainsExpectedItems(string fileName, string directoryName)
        {
            var results = await _container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Flat,
                BlobListingDetails.None,
                null,
                null);

            var expectedUri = new Uri(Path.Combine(BasePath, directoryName, fileName));

            Assert.True(results.Results.OfType<IAzureBlockBlob>().Any(b => b.Uri == expectedUri));
        }

        [Fact]
        public async Task WillGetExpectedNumberOfResultsForHierarchicalListing()
        {
            var results = await _container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                null,
                null);

            Assert.Equal(15, results.Results.Count());
        }

        [Fact]
        public async Task HierarchicalListingWillContainExpectedNumberOfDirectories()
        {
            var results = await _container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                null,
                null);

            Assert.Equal(10, results.Results.OfType<IAzureBlobDirectory>().Count());
        }

        [Theory]
        [InlineData("1a")]
        [InlineData("1b")]
        [InlineData("1c")]
        [InlineData("1d")]
        [InlineData("1e")]
        [InlineData("2a")]
        [InlineData("2b")]
        [InlineData("2c")]
        [InlineData("2d")]
        [InlineData("2e")]
        public async Task HierarchicalListingWillContainExpectedDirectories(string directoryName)
        {
            var results = await _container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                null,
                null);

            var expectedUri = new Uri(Path.Combine(BasePath, directoryName));

            Assert.True(results.Results.OfType<IAzureBlobDirectory>().Any(d => d.Uri == expectedUri));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        public async Task HierarchicalListingWillContainExpectedFiles(string fileName)
        {
            var results = await _container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                null,
                null);

            var expectedUri = new Uri(Path.Combine(BasePath, fileName));

            Assert.True(results.Results.OfType<IAzureBlockBlob>().Any(b => b.Uri == expectedUri));
        }

        [Fact]
        public async Task WillHaveExpectedNumberOfItemsWithFlatListingAndPrefix()
        {
            var results = await _container.ListBlobsSegmentedAsync(
                "1",
                BlobListing.Flat,
                BlobListingDetails.None,
                null,
                null);

            Assert.Equal(2, results.Results.Count());
        }

        [Theory]
        [InlineData("1", "")]
        [InlineData("1", "1a")]
        public async Task FlatListingContainsExpectedItemsWithPrefix(string fileName, string directoryName)
        {
            var results = await _container.ListBlobsSegmentedAsync(
                "1",
                BlobListing.Flat,
                BlobListingDetails.None,
                null,
                null);

            var expectedUri = new Uri(Path.Combine(BasePath, directoryName, fileName));

            Assert.True(results.Results.Any(b => b.Uri == expectedUri));
        }

        [Fact]
        public async Task WillHaveExpectedNumberOfItemsWithHierarchicalListingAndPrefix()
        {
            var results = await _container.ListBlobsSegmentedAsync(
                "1",
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                null,
                null);

            Assert.Equal(6, results.Results.Count());
        }

        [Theory]
        [InlineData("1a")]
        [InlineData("1b")]
        [InlineData("1c")]
        [InlineData("1d")]
        [InlineData("1e")]
        public async Task HierarchicalListingWillContainExpectedDirectoriesWithPrefix(string directoryName)
        {
            var results = await _container.ListBlobsSegmentedAsync(
                "1",
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                null,
                null);

            var expectedUri = new Uri(Path.Combine(BasePath, directoryName));

            Assert.True(results.Results.OfType<IAzureBlobDirectory>().Any(d => d.Uri == expectedUri));
        }

        [Fact]
        public async Task HierarchicalListingWillContainExpectedFileWithPrefix()
        {
            var results = await _container.ListBlobsSegmentedAsync(
                "1",
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                null,
                null);

            var expectedUri = new Uri(Path.Combine(BasePath, "1"));

            Assert.True(results.Results.OfType<IAzureBlockBlob>().Any(b => b.Uri == expectedUri));
        }

        [Fact]
        public async Task WillHaveExpectedNumberOfFilesWhenContinuingWithFlatListing()
        {
            var token = (await _container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Flat,
                BlobListingDetails.None,
                4,
                null)).ContinuationToken;

            var results = await _container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Flat,
                BlobListingDetails.None,
                null,
                token);

            Assert.Equal(6, results.Results.Count());
        }

        [Theory]
        [InlineData("4", "")]
        [InlineData("5", "")]
        [InlineData("1", "1a")]
        [InlineData("2", "1a")]
        [InlineData("3", "1a")]
        [InlineData("4", "1a")]
        [InlineData("5", "1a")]
        public async Task WillHaveExpectedFilesWhenContinuingWithFlatListing(string fileName, string directoryName)
        {
            var token = (await _container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Flat,
                BlobListingDetails.None,
                3,
                null)).ContinuationToken;

            var results = await _container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Flat,
                BlobListingDetails.None,
                null,
                token);

            var expectedUri = new Uri(Path.Combine(BasePath, directoryName, fileName));

            Assert.True(results.Results.OfType<IAzureBlockBlob>().Any(b => b.Uri == expectedUri));
        }

        [Fact]
        public async Task WillHaveExpectedNumberOfItemsWhenContinuingWithHierarchicalListing()
        {
            var token = (await _container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                9,
                null)).ContinuationToken;

            var results = await _container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                null,
                token);

            Assert.Equal(6, results.Results.Count());
        }
    }
}