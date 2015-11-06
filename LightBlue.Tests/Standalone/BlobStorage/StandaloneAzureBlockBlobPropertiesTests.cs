using System.IO;
using System.Threading.Tasks;

using AssertExLib;

using ExpectedObjects;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage;

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
        [MemberData("BlobNames")]
        public void WillThrowOnSaveOfPropertiesIfBlobDoesNotExist(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);

            Assert.Throws<StorageException>(() => blob.SetProperties());
        }

        [Theory]
        [MemberData("BlobNames")]
        public void WillThrowOnAsyncSaveOfPropertiesIfBlobDoesNotExist(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);

            AssertEx.Throws<StorageException>(() => blob.SetPropertiesAsync());
        }

        [Theory]
        [MemberData("BlobNames")]
        public void CanPersistAndRetrieveProperties(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";
            sourceBlob.SetProperties();

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
        [MemberData("BlobNames")]
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
        [MemberData("BlobNames")]
        public async Task CanPersistAndRetrievePropertiesAsync(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";
            sourceBlob.SetProperties();

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
        [MemberData("BlobNames")]
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
        [MemberData("BlobNames")]
        public void CanPersistPropertiesAsync(string blobName)
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
        [MemberData("BlobNames")]
        public void PropertiesNotPersistedUntilSet(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            loadedBlob.FetchAttributes();

            new
            {
                Properties = new
                {
                    ContentType = "application/octet-stream",
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Theory]
        [MemberData("BlobNames")]
        public void PropertiesCanBeSetRepeatedly(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";
            sourceBlob.SetProperties();
            sourceBlob.Properties.ContentType = "something else";
            sourceBlob.SetProperties();

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
        [MemberData("BlobNames")]
        public void FetchingAttributesOverwritesAnyUnsavedPropertyValues(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";
            sourceBlob.SetProperties();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            loadedBlob.FetchAttributes();
            sourceBlob.Properties.ContentType = "something else";
            loadedBlob.FetchAttributes();

            new
            {
                Properties = new
                {
                    ContentType = "something",
                    Length = (long)12
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Theory]
        [MemberData("BlobNames")]
        [Trait("Category", "Slow")]
        public void WillThrowOnSaveOfMetadataWhenFileWriteRetriesExhausted(string blobName)
        {
            var metadataPath = Path.Combine(BasePath, ".meta", blobName);
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "thing";
            sourceBlob.SetProperties();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            loadedBlob.FetchAttributes();
            loadedBlob.Properties.ContentType = "otherthing";

            using (File.Open(metadataPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                Assert.Throws<StorageException>(() => loadedBlob.SetProperties());
            }
        }
    }
}