using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using LightBlue.Standalone;
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
                yield return new object[] {BlobTraits.None};
                yield return new object[] {BlobTraits.ImmutabilityPolicy};
                yield return new object[] {BlobTraits.LegalHold};
                yield return new object[] {BlobTraits.CopyStatus };
                yield return new object[] {BlobTraits.ImmutabilityPolicy | BlobTraits.LegalHold};
                yield return new object[] {BlobTraits.ImmutabilityPolicy | BlobTraits.CopyStatus};
                yield return new object[] {BlobTraits.LegalHold | BlobTraits.CopyStatus};
                yield return new object[] { BlobTraits.LegalHold | BlobTraits.CopyStatus | BlobTraits.ImmutabilityPolicy};
            }
        }

        public static IEnumerable<object[]> DetailsWithMetadata
        {
            get
            {
                yield return new object[] {BlobTraits.Metadata};
                yield return new object[] {BlobTraits.Metadata | BlobTraits.ImmutabilityPolicy};
                yield return new object[] {BlobTraits.Metadata | BlobTraits.CopyStatus};
                yield return new object[] {BlobTraits.Metadata | BlobTraits.LegalHold};
                yield return new object[] { BlobTraits.All};
            }
        }

        [Theory]
        [MemberData(nameof(DetailsWithoutMetadata))]
        public async Task ContainerWillNotLoadMetadataIfNotSpecified(BlobTraits blobTraits)
        {
            var results = await new StandaloneAzureBlobContainer(BasePath)
                .GetBlobs(
                    "",
                    blobTraits: blobTraits);

            Assert.Empty(results.OfType<IAzureBlockBlob>().First(r => r.Name == "flat").Metadata);
        }

        [Theory]
        [MemberData(nameof(DetailsWithMetadata))]
        public async Task ContainerWillLoadMetadataIfSpecified(BlobTraits blobTraits)
        {
            var results = await new StandaloneAzureBlobContainer(BasePath)
                .GetBlobs(
                    "",
                    blobTraits: blobTraits);

            var blob = results.OfType<IAzureBlockBlob>().First(r => r.Name == "flat");

            Assert.Equal("flat", blob.Metadata["thing"]);
        }
    }
}