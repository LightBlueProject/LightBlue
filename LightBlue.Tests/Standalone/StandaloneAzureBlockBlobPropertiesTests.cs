using System.IO;
using System.Threading.Tasks;

using AssertExLib;

using ExpectedObjects;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureBlockBlobPropertiesTests : StandaloneAzureTestsBase
    {
        public const string BlobName = "someblob";

        public StandaloneAzureBlockBlobPropertiesTests()
            : base(DirectoryType.Container)
        {
            var metadataDirectoryPath = Path.Combine(BasePath, ".meta");
            Directory.CreateDirectory(metadataDirectoryPath);
        }

        [Fact]
        public void WillThrowOnSaveOfPropertiesIfBlobDoesNotExist()
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, BlobName);

            Assert.Throws<StorageException>(() => blob.SetProperties());
        }

        [Fact]
        public void WillThrowOnAsyncSaveOfPropertiesIfBlobDoesNotExist()
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, BlobName);

            AssertEx.Throws<StorageException>(() => blob.SetPropertiesAsync());
        }

        [Fact]
        public void CanPersistAndRetrieveProperties()
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";
            sourceBlob.SetProperties();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
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

        [Fact]
        public void DefaultsToOctetStreamWhenLoadingPropertiesWhenPreviouslyUnset()
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, BlobName);
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

        [Fact]
        public async Task CanPersistAndRetrievePropertiesAsync()
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";
            sourceBlob.SetProperties();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
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

        [Fact]
        public async Task DefaultsToOctetStreamWhenLoadingPropertiesWhenPreviouslyUnsetAsync()
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, BlobName);
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

        [Fact]
        public void CanPersistPropertiesAsync()
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";
            sourceBlob.SetPropertiesAsync().Wait();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
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

        [Fact]
        public void PropertiesNotPersistedUntilSet()
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);

            new
            {
                Properties = new
                {
                    ContentType = (string) null,
                    Length = (long) -1,
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Fact]
        public void FetchingAttributesOverwritesAnyUnsavedPropertyValues()
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";
            sourceBlob.SetProperties();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
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
    }
}