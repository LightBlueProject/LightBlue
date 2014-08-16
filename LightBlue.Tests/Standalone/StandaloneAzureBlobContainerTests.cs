using System;
using System.IO;
using System.Threading.Tasks;

using ExpectedObjects;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage.Blob;

using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureBlobContainerTests : StandaloneAzureTestsBase
    {
        [Fact]
        public void DoesNotCreateContainerDirectoryOnConstruction()
        {
            new StandaloneAzureBlobContainer(BasePath);
        }

        [Fact]
        public void WillCreateContainerPathOnCreateIfNotExists()
        {
            new StandaloneAzureBlobContainer(BasePath).CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(BasePath));
        }

        [Fact]
        public void WillCreateMetadataPathOnCreateIfNotExists()
        {
            new StandaloneAzureBlobContainer(BasePath).CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(Path.Combine(BasePath, ".meta")));
        }

        [Fact]
        public void WillUseCorrectContainerPathWhenGivenBasePathAndContainerName()
        {
            new StandaloneAzureBlobContainer(BasePath, SubPathElement).CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(SubPath));
        }

        [Fact]
        public void WillUseCorrectContainerPathWhenGiveUri()
        {
            new StandaloneAzureBlobContainer(SubPathUri)
                .CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(SubPath));
        }

        [Fact]
        public void WillHaveCorrectUriWhenGivenContainerDirectory()
        {
            var container = new StandaloneAzureBlobContainer(BasePath);

            Assert.Equal(new Uri(BasePath), container.Uri);
        }

        [Fact]
        public void WillHaveCorrectUriWhenGivenBasePathAndContainerName()
        {
            var container = new StandaloneAzureBlobContainer(BasePath, SubPathElement);

            Assert.Equal(SubPathUri, container.Uri);
        }

        [Fact]
        public void WillHaveCorrectUriWhenGivenUri()
        {
            var container = new StandaloneAzureBlobContainer(SubPathUri);

            Assert.Equal(SubPathUri, container.Uri);
        }

        [Fact]
        public void CanDetermineContainerDoesNotExist()
        {
            var container = new StandaloneAzureBlobContainer(BasePath);

            Assert.False(container.Exists());
        }

        [Fact]
        public void CanDetermineContainerExists()
        {
            var container = new StandaloneAzureBlobContainer(BasePath);

            container.CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(container.Exists());
        }

        [Fact]
        public async Task CanDetermineContainerDoesNotExistAsync()
        {
            var container = new StandaloneAzureBlobContainer(BasePath);

            Assert.False(await container.ExistsAsynx());
        }

        [Fact]
        public async Task CanDetermineContainerExistsAsync()
        {
            var container = new StandaloneAzureBlobContainer(BasePath);

            container.CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(await container.ExistsAsynx());
        }

        [Fact]
        public void CanGetBlobInstance()
        {
            var container = new StandaloneAzureBlobContainer(BasePath);
            container.CreateIfNotExists(BlobContainerPublicAccessType.Off);

            var blob = container.GetBlockBlobReference("testblob");

            new
            {
                Name = "testblob",
                Uri = new Uri(Path.Combine(BasePath, "testblob"))
            }.ToExpectedObject().ShouldMatch(blob);
        }

        [Theory]
        [InlineData(SharedAccessBlobPermissions.Delete)]
        [InlineData(SharedAccessBlobPermissions.List)]
        [InlineData(SharedAccessBlobPermissions.Read)]
        [InlineData(SharedAccessBlobPermissions.Write)]
        public void WillReturnEmptyStringForSharedAccessKeySignature(SharedAccessBlobPermissions permissions)
        {
            var container = new StandaloneAzureBlobContainer(BasePath);

            Assert.Equal("", container.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = permissions
            }));
        }
    }
}