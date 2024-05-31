using System.IO;
using System.Threading.Tasks;
using Azure;
using ExpectedObjects;
using LightBlue.Standalone;
using Xunit;

namespace LightBlue.Tests.Standalone.BlobStorage
{
    public class StandaloneAzureBlockBlobPropertiesTests : StandaloneAzureTestsBase
    {
        public StandaloneAzureBlockBlobPropertiesTests()
            : base(DirectoryType.Container)
        {
            var metadataDirectoryPath = Path.Combine(BasePath, ".meta");
            Directory.CreateDirectory(metadataDirectoryPath);
        }

        [Theory]
        [MemberData(nameof(BlobNames))]
        public async Task WillThrowOnSaveOfContentTypeIfBlobDoesNotExist(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);

            blob.Properties.ContentType = "something";

            await Assert.ThrowsAsync<RequestFailedException>(() => blob.SetPropertiesAsync());
        }

        [Theory]
        [MemberData(nameof(BlobNames))]
        public async Task CanPersistAndRetrieveContentType(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";
            await sourceBlob.SetPropertiesAsync();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            loadedBlob.FetchAttributes();

            new
            {
                Properties = new
                {
                    ContentType = "something",
                    Length = (long) 12
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Theory]
        [MemberData(nameof(BlobNames))]
        public void DefaultsToOctetStreamWhenLoadingPropertiesWhenPreviouslyUnset(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(blob);

            blob.FetchAttributes();

            new
            {
                Properties = new
                {
                    ContentType = "application/octet-stream",
                    Length = (long) 12
                }
            }.ToExpectedObject().ShouldMatch(blob);
        }

        [Theory]
        [MemberData(nameof(BlobNames))]
        public async Task CanPersistAndRetrieveContentTypeAsync(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);
            sourceBlob.Properties.ContentType = "something";
            await sourceBlob.SetPropertiesAsync();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            await loadedBlob.FetchAttributesAsync();

            new
            {
                Properties = new
                {
                    ContentType = "something",
                    Length = (long) 12
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Theory]
        [MemberData(nameof(BlobNames))]
        public async Task DefaultsToOctetStreamWhenLoadingPropertiesWhenPreviouslyUnsetAsync(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(blob);

            await blob.FetchAttributesAsync();

            new
            {
                Properties = new
                {
                    ContentType = "application/octet-stream",
                    Length = (long) 12
                }
            }.ToExpectedObject().ShouldMatch(blob);
        }

        [Theory]
        [MemberData(nameof(BlobNames))]
        public void CanPersistContentTypeAsync(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);
            sourceBlob.Properties.ContentType = "something";
            sourceBlob.SetPropertiesAsync().Wait();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            loadedBlob.FetchAttributes();

            new
            {
                Properties = new
                {
                    ContentType = "something",
                    Length = (long) 12
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Theory]
        [MemberData(nameof(BlobNames))]
        public void ContentTypeCanBeSetRepeatedly(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";
            sourceBlob.SetPropertiesAsync().Wait();
            sourceBlob.Properties.ContentType = "something else";
            sourceBlob.SetPropertiesAsync().Wait();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            loadedBlob.FetchAttributes();

            new
            {
                Properties = new
                {
                    ContentType = "something else"
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Theory]
        [MemberData(nameof(BlobNames))]
        [Trait("Category", "Slow")]
        public async Task WillThrowOnSaveOfContentTypeWhenFileWriteRetriesExhausted(string blobName)
        {
            var metadataPath = Path.Combine(BasePath, ".meta", blobName);
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "thing";
            await sourceBlob.SetPropertiesAsync();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            loadedBlob.FetchAttributes();

            using (File.Open(metadataPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                loadedBlob.Properties.ContentType = "otherthing";
                await Assert.ThrowsAsync<RequestFailedException>(() => loadedBlob.SetPropertiesAsync());
            }
        }
    }
}