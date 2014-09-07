using System.IO;
using System.Threading.Tasks;

using AssertExLib;

using ExpectedObjects;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage;

using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Standalone
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
        [PropertyData("BlobNames")]
        public void WillThrowOnSaveOfPropertiesIfBlobDoesNotExist(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);

            Assert.Throws<StorageException>(() => blob.SetProperties());
        }

        [Theory]
        [PropertyData("BlobNames")]
        public void WillThrowOnAsyncSaveOfPropertiesIfBlobDoesNotExist(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);

            AssertEx.Throws<StorageException>(() => blob.SetPropertiesAsync());
        }

        [Theory]
        [PropertyData("BlobNames")]
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
        [PropertyData("BlobNames")]
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
        [PropertyData("BlobNames")]
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
        [PropertyData("BlobNames")]
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
        [PropertyData("BlobNames")]
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
        [PropertyData("BlobNames")]
        public void PropertiesNotPersistedUntilSet(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);

            new
            {
                Properties = new
                {
                    ContentType = (string) null,
                    Length = (long) -1,
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Theory]
        [PropertyData("BlobNames")]
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
    }
}