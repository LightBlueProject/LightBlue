using System.Collections.Generic;
using System.IO;

using AssertExLib;

using ExpectedObjects;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureBlockBlobMetadataTests : StandaloneAzureTestsBase
    {
        private const string BlobName = "someblob";
        public StandaloneAzureBlockBlobMetadataTests()
            : base(DirectoryType.Container)
        {
            var metadataDirectoryPath = Path.Combine(BasePath, ".meta");
            Directory.CreateDirectory(metadataDirectoryPath);
        }

        [Fact]
        public void WillThrowOnFetchOfAttributesIfBlobDoesNotExist()
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, BlobName);

            Assert.Throws<StorageException>(() => blob.FetchAttributes());
        }

        [Fact]
        public void WillThrowOnSaveOfMetadataIfBlobDoesNotExist()
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, BlobName);

            Assert.Throws<StorageException>(() => blob.SetMetadata());
        }

        [Fact]
        public void WillThrowOnAsyncSaveOfMetadataIfBlobDoesNotExist()
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, BlobName);

            AssertEx.Throws<StorageException>(() => blob.SetMetadataAsync());
        }

        [Fact]
        public void DefaultsToEmptyMetadataOnFetchWithNoSavedMetadata()
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.FetchAttributes();

            new
            {
                Metadata = new Dictionary<string, string>()
            }.ToExpectedObject().ShouldMatch(sourceBlob);
        }

        [Fact]
        public void CanPersistAndRetrieveMetadata()
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.SetMetadata();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            loadedBlob.FetchAttributes();

            new
            {
                Metadata = new Dictionary<string, string>
                {
                    { "thing", "something"}
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }
        
        [Fact]
        public void CanAppendToExistingPersistedMetadata()
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.SetMetadata();

            sourceBlob.FetchAttributes();
            sourceBlob.Metadata["other thing"] = "whatever";
            sourceBlob.SetMetadata();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            loadedBlob.FetchAttributes();
            new
            {
                Metadata = new Dictionary<string, string>
                {
                    { "thing", "something"},
                    {"other thing", "whatever"}
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Fact]
        public void CanPersistMetadataASync()
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.SetMetadataAsync().Wait();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            loadedBlob.FetchAttributes();

            new
            {
                Metadata = new Dictionary<string, string>
                {
                    { "thing", "something"}
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }
        
        [Fact]
        public void MetadataNotPersistedUntilSet()
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Metadata["thing"] = "something";

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            loadedBlob.FetchAttributes();

            new
            {
                Metadata = new Dictionary<string, string>()
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Fact]
        public void FetchingPropertiesOverwritesAnyUnsavedMetadataValues()
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.SetMetadata();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            loadedBlob.FetchAttributes();
            loadedBlob.Metadata["other thing"] = "whatever";
            loadedBlob.FetchAttributes();

            new
            {
                Metadata = new Dictionary<string, string>
                {
                    { "thing", "something"}
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }
        
        [Fact]
        public void WillThrowOnSaveOfMetadataWhenFileWriteRetriesExhausted()
        {
            var metadataPath = Path.Combine(BasePath, ".meta", BlobName);
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.SetMetadata();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            loadedBlob.FetchAttributes();
            loadedBlob.Metadata["other thing"] = "whatever";
            
            using (File.Open(metadataPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                Assert.Throws<StorageException>(() => loadedBlob.SetMetadata());
            }
        }
    }
}