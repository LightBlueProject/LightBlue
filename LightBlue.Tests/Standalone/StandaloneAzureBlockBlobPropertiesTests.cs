using System.Collections.Generic;
using System.IO;

using AssertExLib;

using ExpectedObjects;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureBlockBlobPropertiesTests : StandaloneAzureTestsBase
    {
        public StandaloneAzureBlockBlobPropertiesTests()
        {
            var metadataDirectoryPath = Path.Combine(BasePath, ".meta");
            Directory.CreateDirectory(metadataDirectoryPath);
        }

        [Fact]
        public void WillThrowOnSaveOfPropertiesIfBlobDoesNotExist()
        {
            var blob = new StandaloneAzureBlockBlob(SubPathUri);

            Assert.Throws<StorageException>(() => blob.SetProperties());
        }

        [Fact]
        public void WillThrowOnAsyncSaveOfPropertiesIfBlobDoesNotExist()
        {
            var blob = new StandaloneAzureBlockBlob(SubPathUri);

            AssertEx.Throws<StorageException>(() => blob.SetPropertiesAsync());
        }

        [Fact]
        public void CanPersistAndRetrieveProperties()
        {
            var sourceBlob = new StandaloneAzureBlockBlob(SubPathUri);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";
            sourceBlob.SetProperties();

            var loadedBlob = new StandaloneAzureBlockBlob(SubPathUri);
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
            var blob = new StandaloneAzureBlockBlob(SubPathUri);
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
        public void CanPersistPropertiesAsync()
        {
            var sourceBlob = new StandaloneAzureBlockBlob(SubPathUri);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";
            sourceBlob.SetPropertiesAsync().Wait();

            var loadedBlob = new StandaloneAzureBlockBlob(SubPathUri);
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
            var sourceBlob = new StandaloneAzureBlockBlob(SubPathUri);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";

            var loadedBlob = new StandaloneAzureBlockBlob(SubPathUri);

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
            var sourceBlob = new StandaloneAzureBlockBlob(SubPathUri);
            CreateBlobContent(sourceBlob);

            sourceBlob.Properties.ContentType = "something";
            sourceBlob.SetProperties();

            var loadedBlob = new StandaloneAzureBlockBlob(SubPathUri);
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