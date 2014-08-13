using System;
using System.IO;
using System.Threading.Tasks;

using ExpectedObjects;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage.Blob;

using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureBlobContainerTests : StandaloneAzureTestsBase, IDisposable
    {
        [Fact]
        public void DoesNotCreateContainerDirectoryOnConstruction()
        {
            new StandaloneAzureBlobContainer(BasePath);

        }

        [Fact]
        public void WillCreateContainerPathOnCreateIfNotExists()
        {
            new StandaloneAzureBlobContainer(BasePath).CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(BasePath));
        }

        [Fact]
        public void WillCreateMetadataPathOnCreateIfNotExists()
        {
            new StandaloneAzureBlobContainer(BasePath).CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(Path.Combine(BasePath, ".meta")));
        }

        [Fact]
        public void WillUseCorrectContainerPathWhenGivenBasePathAndContainerName()
        {
            new StandaloneAzureBlobContainer(BasePath, "test").CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(Path.Combine(BasePath, "test")));
        }

        [Fact]
        public void CanDetermineContainerDoesNotExist()
        {
            var container = new StandaloneAzureBlobContainer(BasePath);

            Assert.False(container.Exists());
        }

        [Fact]
        public void CanDetermineContainerExists()
        {
            var container = new StandaloneAzureBlobContainer(BasePath);

            container.CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(container.Exists());
        }

        [Fact]
        public async Task CanDetermineContainerDoesNotExistAsync()
        {
            var container = new StandaloneAzureBlobContainer(BasePath);

            Assert.False(await container.ExistsAsynx());
        }

        [Fact]
        public async Task CanDetermineContainerExistsAsync()
        {
            var container = new StandaloneAzureBlobContainer(BasePath);

            container.CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(await container.ExistsAsynx());
        }

        [Fact]
        public void CanGetBlobInstance()
        {
            var container = new StandaloneAzureBlobContainer(BasePath);
            container.CreateIfNotExists(BlobContainerPublicAccessType.Off);

            var blob = container.GetBlockBlobReference("testblob");

            new
            {
                Name = "testblob",
                Uri = new Uri(Path.Combine(BasePath, "testblob"))
            }.ToExpectedObject().ShouldMatch(blob);
        }

        [Theory]
        [InlineData(BlobListingDetails.Snapshots)]
        [InlineData(BlobListingDetails.All)]
        public void AllowsSnapshotsOnlyInFlatMode(BlobListingDetails blobListingDetails)
        {
            var container = new StandaloneAzureBlobContainer(BasePath);
            Assert.Throws<ArgumentException>(() => container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Hierarchical,
                blobListingDetails,
                500,
                null));
        }
    }
}