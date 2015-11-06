using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage.Blob;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class ListMetadataTests : StandaloneAzureTestsBase
    {
        public ListMetadataTests()
            : base(DirectoryType.Container)
        {
            var flatBlob = new StandaloneAzureBlockBlob(BasePath, "flat");
            flatBlob.UploadFromByteArrayAsync(Encoding.UTF8.GetBytes("flat"));
            flatBlob.Metadata["thing"] = "flat";
            flatBlob.SetMetadata();

            var withPath = new StandaloneAzureBlockBlob(BasePath, @"random\path\blob");
            withPath.UploadFromByteArrayAsync(Encoding.UTF8.GetBytes("withPath"));
            withPath.Metadata["thing"] = "withPath";
            withPath.SetMetadata();
        }

        public static IEnumerable<object[]> DetailsWithoutMetadata
        {
            get
            {
                yield return new object[] {BlobListingDetails.None};
                yield return new object[] {BlobListingDetails.Snapshots};
                yield return new object[] {BlobListingDetails.UncommittedBlobs};
                yield return new object[] {BlobListingDetails.Copy};
                yield return new object[] {BlobListingDetails.Snapshots | BlobListingDetails.UncommittedBlobs};
                yield return new object[] {BlobListingDetails.Snapshots | BlobListingDetails.Copy};
                yield return new object[] {BlobListingDetails.UncommittedBlobs | BlobListingDetails.Copy};
                yield return new object[] {BlobListingDetails.UncommittedBlobs | BlobListingDetails.Copy | BlobListingDetails.Snapshots};
            }
        }

        public static IEnumerable<object[]> DetailsWithoutMetadataNoSnapshots
        {
            get
            {
                yield return new object[] {BlobListingDetails.None};
                yield return new object[] {BlobListingDetails.UncommittedBlobs};
                yield return new object[] {BlobListingDetails.Copy};
                yield return new object[] {BlobListingDetails.UncommittedBlobs | BlobListingDetails.Copy};
            }
        }

        public static IEnumerable<object[]> DetailsWithMetadata
        {
            get
            {
                yield return new object[] {BlobListingDetails.Metadata};
                yield return new object[] {BlobListingDetails.Metadata | BlobListingDetails.Snapshots};
                yield return new object[] {BlobListingDetails.Metadata | BlobListingDetails.Copy};
                yield return new object[] {BlobListingDetails.Metadata | BlobListingDetails.UncommittedBlobs};
                yield return new object[] {BlobListingDetails.All};
            }
        }

        public static IEnumerable<object[]> DetailsWithMetadataNoSnapshots
        {
            get
            {
                yield return new object[] {BlobListingDetails.Metadata};
                yield return new object[] {BlobListingDetails.Metadata | BlobListingDetails.Copy};
                yield return new object[] {BlobListingDetails.Metadata | BlobListingDetails.UncommittedBlobs};
                yield return new object[] {BlobListingDetails.Metadata | BlobListingDetails.UncommittedBlobs | BlobListingDetails.Copy};
            }
        }

        [Theory]
        [MemberData("DetailsWithoutMetadata")]
        public async Task ContainerWillNotLoadMetadataIfNotSpecifiedForFlatListing(BlobListingDetails blobListingDetails)
        {
            var results = await new StandaloneAzureBlobContainer(BasePath)
                .ListBlobsSegmentedAsync(
                    "",
                    BlobListing.Flat,
                    blobListingDetails,
                    null,
                    null);

            Assert.Empty(results.Results.OfType<IAzureBlockBlob>().First(r => r.Name == "flat").Metadata);
        }

        [Theory]
        [MemberData("DetailsWithMetadata")]
        public async Task ContainerWillLoadMetadataIfSpecifiedForFlatListing(BlobListingDetails blobListingDetails)
        {
            var results = await new StandaloneAzureBlobContainer(BasePath)
                .ListBlobsSegmentedAsync(
                    "",
                    BlobListing.Flat,
                    blobListingDetails,
                    null,
                    null);

            var blob = results.Results.OfType<IAzureBlockBlob>().First(r => r.Name == "flat");

            Assert.Equal("flat", blob.Metadata["thing"]);
        }

        [Theory]
        [MemberData("DetailsWithoutMetadataNoSnapshots")]
        public async Task ContainerWillNotLoadMetadataIfNotSpecifiedForHierarchicalListing(BlobListingDetails blobListingDetails)
        {
            var results = await new StandaloneAzureBlobContainer(BasePath)
                .ListBlobsSegmentedAsync(
                    "",
                    BlobListing.Hierarchical,
                    blobListingDetails,
                    null,
                    null);

            Assert.Empty(results.Results.OfType<IAzureBlockBlob>().First(r => r.Name == "flat").Metadata);
        }

        [Theory]
        [MemberData("DetailsWithMetadataNoSnapshots")]
        public async Task ContainerWillLoadMetadataIfSpecifiedForHierarchicalListing(BlobListingDetails blobListingDetails)
        {
            var results = await new StandaloneAzureBlobContainer(BasePath)
                .ListBlobsSegmentedAsync(
                    "",
                    BlobListing.Hierarchical,
                    blobListingDetails,
                    null,
                    null);

            var blob = results.Results.OfType<IAzureBlockBlob>().First(r => r.Name == "flat");

            Assert.Equal("flat", blob.Metadata["thing"]);
        }

        [Theory]
        [MemberData("DetailsWithoutMetadata")]
        public async Task DirectoryWillNotLoadMetadataIfNotSpecifiedForFlatListing(BlobListingDetails blobListingDetails)
        {
            var results = await new StandaloneAzureBlobDirectory(BasePath, Path.Combine(BasePath, @"random\path"))
                .ListBlobsSegmentedAsync(
                    BlobListing.Flat,
                    blobListingDetails,
                    null,
                    null);

            Assert.Empty(results.Results.OfType<IAzureBlockBlob>().First(r => r.Name == @"random\path\blob").Metadata);
        }

        [Theory]
        [MemberData("DetailsWithMetadata")]
        public async Task DirectoryWillLoadMetadataIfSpecifiedForFlatListing(BlobListingDetails blobListingDetails)
        {
            var results = await new StandaloneAzureBlobDirectory(BasePath, Path.Combine(BasePath, @"random\path"))
                .ListBlobsSegmentedAsync(
                    BlobListing.Flat,
                    blobListingDetails,
                    null,
                    null);

            var blob = results.Results.OfType<IAzureBlockBlob>().First(r => r.Name == @"random\path\blob");

            Assert.Equal("withPath", blob.Metadata["thing"]);
        }

        [Theory]
        [MemberData("DetailsWithoutMetadataNoSnapshots")]
        public async Task DirectoryWillNotLoadMetadataIfNotSpecifiedForHierarchicalListing(BlobListingDetails blobListingDetails)
        {
            var results = await new StandaloneAzureBlobDirectory(BasePath, Path.Combine(BasePath, @"random\path"))
                .ListBlobsSegmentedAsync(
                    BlobListing.Hierarchical,
                    blobListingDetails,
                    null,
                    null);

            Assert.Empty(results.Results.OfType<IAzureBlockBlob>().First(r => r.Name == @"random\path\blob").Metadata);
        }

        [Theory]
        [MemberData("DetailsWithMetadataNoSnapshots")]
        public async Task DirectoryWillLoadMetadataIfSpecifiedForHierarchicalListing(BlobListingDetails blobListingDetails)
        {
            var results = await new StandaloneAzureBlobDirectory(BasePath, Path.Combine(BasePath, @"random\path"))
                .ListBlobsSegmentedAsync(
                    BlobListing.Hierarchical,
                    blobListingDetails,
                    null,
                    null);

            var blob = results.Results.OfType<IAzureBlockBlob>().First(r => r.Name == @"random\path\blob");

            Assert.Equal("withPath", blob.Metadata["thing"]);
        }
    }
}