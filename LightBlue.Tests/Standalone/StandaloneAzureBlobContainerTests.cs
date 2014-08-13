using System;
using System.IO;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage.Blob;

using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureBlobContainerTests : IDisposable
    {
        private readonly string _containerPath;

        public StandaloneAzureBlobContainerTests()
        {
            _containerPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        [Theory]
        [InlineData(BlobListingDetails.Snapshots)]
        [InlineData(BlobListingDetails.All)]
        public void AllowsSnapshotsOnlyInFlatMode(BlobListingDetails blobListingDetails)
        {
            var container = new StandaloneAzureBlobContainer(_containerPath);
            Assert.Throws<ArgumentException>(() => container.ListBlobsSegmentedAsync(
                "",
                BlobListing.Hierarchical,
                blobListingDetails,
                500,
                null));
        }

        public void Dispose()
        {
            if (!string.IsNullOrWhiteSpace(_containerPath) && Directory.Exists(_containerPath))
            {
                Directory.Delete(_containerPath, true);
            }
        }
    }
}