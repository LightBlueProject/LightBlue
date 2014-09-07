using System.Collections.Generic;
using System.IO;

using System.Threading.Tasks;
using System.Linq;

using AssertExLib;

using ExpectedObjects;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureBlockBlobMetadataTests : StandaloneAzureTestsBase
    {
        public StandaloneAzureBlockBlobMetadataTests()
            : base(DirectoryType.Container)
        {
            var metadataDirectoryPath = Path.Combine(BasePath, ".meta");
            Directory.CreateDirectory(metadataDirectoryPath);
        }

        [Theory]
        [PropertyData("BlobNames")]
        public void WillThrowOnFetchOfAttributesIfBlobDoesNotExist(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);

            Assert.Throws<StorageException>(() => blob.FetchAttributes());
        }

        [Theory]
        [PropertyData("BlobNames")]
        public void WillThrowOnSaveOfMetadataIfBlobDoesNotExist(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);

            Assert.Throws<StorageException>(() => blob.SetMetadata());
        }

        [Theory]
        [PropertyData("BlobNames")]
        public void WillThrowOnAsyncSaveOfMetadataIfBlobDoesNotExist(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);

            AssertEx.Throws<StorageException>(() => blob.SetMetadataAsync());
        }

        [Theory]
        [PropertyData("BlobNames")]
        public void DefaultsToEmptyMetadataOnFetchWithNoSavedMetadata(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.FetchAttributes();

            new
            {
                Metadata = new Dictionary<string, string>()
            }.ToExpectedObject().ShouldMatch(sourceBlob);
        }

        [Theory]
        [PropertyData("BlobNames")]
        public void CanPersistAndRetrieveMetadata(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.SetMetadata();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            loadedBlob.FetchAttributes();

            new
            {
                Metadata = new Dictionary<string, string>
                {
                    { "thing", "something"}
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Theory]
        [PropertyData("BlobNames")]
        public async Task CanPersistAndRetrieveMetadataAsync(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.SetMetadata();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            await loadedBlob.FetchAttributesAsync();

            new
            {
                Metadata = new Dictionary<string, string>
                {
                    { "thing", "something"}
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Theory]
        [PropertyData("BlobNames")]
        public void CanAppendToExistingPersistedMetadata(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.SetMetadata();

            sourceBlob.FetchAttributes();
            sourceBlob.Metadata["other thing"] = "whatever";
            sourceBlob.SetMetadata();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
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

        [Theory]
        [PropertyData("BlobNames")]
        public void CanPersistMetadataAsync(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.SetMetadataAsync().Wait();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            loadedBlob.FetchAttributes();

            new
            {
                Metadata = new Dictionary<string, string>
                {
                    { "thing", "something"}
                }
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Theory]
        [PropertyData("BlobNames")]
        public void MetadataNotPersistedUntilSet(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Metadata["thing"] = "something";

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            loadedBlob.FetchAttributes();

            new
            {
                Metadata = new Dictionary<string, string>()
            }.ToExpectedObject().ShouldMatch(loadedBlob);
        }

        [Theory]
        [PropertyData("BlobNames")]
        public void FetchingPropertiesOverwritesAnyUnsavedMetadataValues(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.SetMetadata();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
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

        [Theory]
        [PropertyData("BlobNames")]
        [Trait("Category", "Slow")]
        public void WillThrowOnSaveOfMetadataWhenFileWriteRetriesExhausted(string blobName)
        {
            var metadataPath = Path.Combine(BasePath, ".meta", blobName);
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);

            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.SetMetadata();

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            loadedBlob.FetchAttributes();
            loadedBlob.Metadata["other thing"] = "whatever";
            
            using (File.Open(metadataPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                Assert.Throws<StorageException>(() => loadedBlob.SetMetadata());
            }
        }

        [Theory]
        [PropertyData("BlobNames")]
        public void SavingPreservesUnknownMetadata(string blobName)
        {
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            CreateBlobContent(sourceBlob);
            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.SetMetadata();

            var metadataPath = Path.Combine(BasePath, ".meta", blobName);
            var expectedProperties = new Dictionary<string, string>
            {
                {"foo", "bar"}, {"couci-couça","svo-svo"}
            };
            WriteJsonPropertiesToFile(metadataPath, expectedProperties);

            var loadedBlob = new StandaloneAzureBlockBlob(BasePath, blobName);
            loadedBlob.FetchAttributes();
            loadedBlob.Metadata["another thing"] = "whatever else";
            loadedBlob.SetMetadata();

            using (var reader = new StreamReader(metadataPath))
            {
                var loaded = JObject.Load(new JsonTextReader(reader));
                var actualMetadata = loaded["Metadata"];
                Assert.Equal("something", actualMetadata["thing"]);
                Assert.Equal("whatever else", actualMetadata["another thing"]);
                expectedProperties.ToList().ForEach(kvp =>
                    Assert.Equal(kvp.Value, loaded[kvp.Key])
                );
            }
        }
        
        private static void WriteJsonPropertiesToFile(string metadataPath, IEnumerable<KeyValuePair<string, string>> properties)
        {
            JObject jObject;
            using (var reader = new StreamReader(metadataPath))
            {
                jObject = JObject.Load(new JsonTextReader(reader));
                foreach (var pair in properties)
                {
                    jObject[pair.Key] = pair.Value;
                }
            }
            using (var writer = new JsonTextWriter(new StreamWriter(metadataPath)))
            {
                new JsonSerializer().Serialize(writer, jObject);
            }
        }
    }
}