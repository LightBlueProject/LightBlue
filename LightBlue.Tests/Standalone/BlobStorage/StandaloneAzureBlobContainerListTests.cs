using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using LightBlue.Standalone;

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
            _container.CreateIfNotExists();

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

        [Fact]
        public async Task WillGetExpectedNumberOfResultsForFlatListing()
        {
            var results = await _container.GetBlobs(
                "",
                blobStates: BlobStates.None);

            Assert.Equal(10, results.Count());
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
            var results = await _container.GetBlobs(
                "",
                blobStates: BlobStates.None);

            var expectedUri = new Uri(Path.Combine(BasePath, directoryName, fileName));

            Assert.Contains(results.OfType<IAzureBlockBlob>(), b => b.Uri == expectedUri);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        public async Task ListingWillContainExpectedFiles(string fileName)
        {
            var results = await _container.GetBlobs(
                "",
                blobStates: BlobStates.None);

            var expectedUri = new Uri(Path.Combine(BasePath, fileName));

            Assert.Contains(results.OfType<IAzureBlockBlob>(), b => b.Uri == expectedUri);
        }

        [Fact]
        public async Task WillHaveExpectedNumberOfItemsWithFlatListingAndPrefix()
        {
            var results = await _container.GetBlobs(
                "1",
                blobStates: BlobStates.None);

            Assert.Equal(2, results.Count());
        }

        [Theory]
        [InlineData("1", "")]
        [InlineData("1", "1a")]
        public async Task FlatListingContainsExpectedItemsWithPrefix(string fileName, string directoryName)
        {
            var results = await _container.GetBlobs(
                "1",
                blobStates: BlobStates.None);

            var expectedUri = new Uri(Path.Combine(BasePath, directoryName, fileName));

            Assert.Contains(results, b => b.Uri == expectedUri);
        }
    }
}