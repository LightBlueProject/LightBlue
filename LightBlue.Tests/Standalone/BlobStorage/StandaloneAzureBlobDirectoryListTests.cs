using System.IO;
using System.Linq;
using System.Threading.Tasks;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage.Blob;

using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Standalone.BlobStorage
{
    public class StandaloneAzureBlobDirectoryListTests : StandaloneAzureTestsBase
    {
        private readonly StandaloneAzureBlobDirectory _directory;

        public StandaloneAzureBlobDirectoryListTests()
            : base(DirectoryType.Container)
        {
            _directory = new StandaloneAzureBlobDirectory(BasePath, "directory");
            var directoryPath = Path.Combine(BasePath, "directory");

            Directory.CreateDirectory(Path.Combine(directoryPath, "1a"));
            Directory.CreateDirectory(Path.Combine(directoryPath, "1b"));
            Directory.CreateDirectory(Path.Combine(directoryPath, "1c"));
            Directory.CreateDirectory(Path.Combine(directoryPath, "1d"));
            Directory.CreateDirectory(Path.Combine(directoryPath, "1e"));

            File.WriteAllText(Path.Combine(directoryPath, "1"), "Test File");
            File.WriteAllText(Path.Combine(directoryPath, "2"), "Test File");
            File.WriteAllText(Path.Combine(directoryPath, "3"), "Test File");
            File.WriteAllText(Path.Combine(directoryPath, "4"), "Test File");
            File.WriteAllText(Path.Combine(directoryPath, "5"), "Test File");

            File.WriteAllText(Path.Combine(directoryPath, "1a", "1"), "Test File");
            File.WriteAllText(Path.Combine(directoryPath, "1a", "2"), "Test File");
            File.WriteAllText(Path.Combine(directoryPath, "1a", "3"), "Test File");
            File.WriteAllText(Path.Combine(directoryPath, "1a", "4"), "Test File");
            File.WriteAllText(Path.Combine(directoryPath, "1b", "5"), "Test File");
        }

        [Theory]
        [InlineData("1a")]
        [InlineData("1b")]
        [InlineData("1c")]
        [InlineData("1d")]
        [InlineData("1e")]
        public async Task HasExpectedSubdirectoriesInHierarchicalListing(string subDirectory)
        {
            var result = await _directory.ListBlobsSegmentedAsync(
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                null,
                null);

            var directories = result.Results.OfType<StandaloneAzureBlobDirectory>();

            var expectedPath = Path.Combine(_directory.Uri.LocalPath, subDirectory);

            Assert.True(directories.Any(d => d.Uri.LocalPath == expectedPath));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        public async Task HasExpectedFilesInHierarchicalListing(string file)
        {
            var result = await _directory.ListBlobsSegmentedAsync(
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                null,
                null);

            var directories = result.Results.OfType<StandaloneAzureBlockBlob>();

            var expectedPath = Path.Combine(_directory.Uri.LocalPath, file);

            Assert.True(directories.Any(d => d.Uri.LocalPath == expectedPath));
        }

        [Fact]
        public async Task HasNoSubdirectoriesInFlatListing()
        {
            var result = await _directory.ListBlobsSegmentedAsync(
                BlobListing.Flat,
                BlobListingDetails.None,
                null,
                null);

            var directories = result.Results.OfType<StandaloneAzureBlobDirectory>();

            Assert.Empty(directories);
        }

        [Theory]
        [InlineData(@"1a\1")]
        [InlineData(@"1a\2")]
        [InlineData(@"1a\3")]
        [InlineData(@"1a\4")]
        [InlineData(@"1b\5")]
        public async Task HasExpectedFilesInFlatListing(string file)
        {
            var result = await _directory.ListBlobsSegmentedAsync(
                BlobListing.Flat,
                BlobListingDetails.None,
                null,
                null);

            var directories = result.Results.OfType<StandaloneAzureBlockBlob>();

            var expectedPath = Path.Combine(_directory.Uri.LocalPath, file);

            Assert.True(directories.Any(d => d.Uri.LocalPath == expectedPath));
        }

        [Theory]
        [InlineData("1a")]
        [InlineData("1b")]
        public async Task CanLimitItemsInHierarchicalListing(string subDirectory)
        {
            var result = await _directory.ListBlobsSegmentedAsync(
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                2,
                null);

            var expectedPath = Path.Combine(_directory.Uri.LocalPath, subDirectory);

            Assert.True(result.Results.Any(d => d.Uri.LocalPath == expectedPath));
        }

        [Theory]
        [InlineData("1a")]
        [InlineData("1b")]
        public async Task CanLimitItemsInFlatListing(string file)
        {
            var result = await _directory.ListBlobsSegmentedAsync(
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                2,
                null);

            var expectedPath = Path.Combine(_directory.Uri.LocalPath, file);

            Assert.True(result.Results.Any(d => d.Uri.LocalPath == expectedPath));
        }

        [Theory]
        [InlineData("1c")]
        [InlineData("1d")]
        public async Task CanPageItemsInHierarchicalListing(string subDirectory)
        {
            var token = (await _directory.ListBlobsSegmentedAsync(
                    BlobListing.Hierarchical,
                    BlobListingDetails.None,
                    2,
                    null))
                .ContinuationToken;
            var result = await _directory.ListBlobsSegmentedAsync(
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                2,
                token);

            var expectedPath = Path.Combine(_directory.Uri.LocalPath, subDirectory);

            Assert.True(result.Results.Any(d => d.Uri.LocalPath == expectedPath));
        }

        [Theory]
        [InlineData("1c")]
        [InlineData("1d")]
        public async Task CanPageItemsInFlatListing(string file)
        {
            var token = (await _directory.ListBlobsSegmentedAsync(
                    BlobListing.Hierarchical,
                    BlobListingDetails.None,
                    2,
                    null))
                .ContinuationToken;
            var result = await _directory.ListBlobsSegmentedAsync(
                BlobListing.Hierarchical,
                BlobListingDetails.None,
                2,
                token);

            var expectedPath = Path.Combine(_directory.Uri.LocalPath, file);

            Assert.True(result.Results.Any(d => d.Uri.LocalPath == expectedPath));
        }
    }
}