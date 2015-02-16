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
        private const string ContainerName = "container";

        private readonly string _containerPath;

        public StandaloneAzureBlobContainerTests()
            : base(DirectoryType.BlobStorage)
        {
            _containerPath = Path.Combine(BasePath, ContainerName);
        }

        [Fact]
        public void DoesNotCreateContainerDirectoryOnConstruction()
        {
            new StandaloneAzureBlobContainer(_containerPath);

            Assert.False(Directory.Exists(_containerPath));
        }

        [Fact]
        public void WillCreateContainerPathOnCreateIfNotExists()
        {
            new StandaloneAzureBlobContainer(_containerPath).CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(_containerPath));
        }

        [Fact]
        public void WillCreateMetadataPathOnCreateIfNotExists()
        {
            new StandaloneAzureBlobContainer(_containerPath).CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(Path.Combine(_containerPath, MetadataDirectory)));
        }

        [Fact]
        public void WillUseCorrectContainerPathWhenGivenBasePathAndContainerName()
        {
            new StandaloneAzureBlobContainer(BasePath, ContainerName).CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(_containerPath));
        }

        [Fact]
        public async Task WillCreateContainerPathOnCreateIfNotExistsAsync()
        {
            await new StandaloneAzureBlobContainer(_containerPath).CreateIfNotExistsAsync(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(_containerPath));
        }

        [Fact]
        public async Task WillCreateMetadataPathOnCreateIfNotExistsAsync()
        {
            await new StandaloneAzureBlobContainer(_containerPath).CreateIfNotExistsAsync(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(Path.Combine(_containerPath, MetadataDirectory)));
        }

        [Fact]
        public async Task WillUseCorrectContainerPathWhenGivenBasePathAndContainerNameAsync()
        {
            await new StandaloneAzureBlobContainer(BasePath, ContainerName).CreateIfNotExistsAsync(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(_containerPath));
        }

        [Fact]
        public void WillUseCorrectContainerPathWhenGiveUri()
        {
            new StandaloneAzureBlobContainer(new Uri(_containerPath))
                .CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(Directory.Exists(_containerPath));
        }

        [Fact]
        public void WillHaveCorrectUriWhenGivenContainerDirectory()
        {
            var container = new StandaloneAzureBlobContainer(_containerPath);

            Assert.Equal(new Uri(_containerPath), container.Uri);
        }

        [Fact]
        public void WillHaveCorrectUriWhenGivenBasePathAndContainerName()
        {
            var container = new StandaloneAzureBlobContainer(BasePath, ContainerName);

            Assert.Equal(new Uri(_containerPath), container.Uri);
        }

        [Fact]
        public void WillHaveCorrectUriWhenGivenUri()
        {
            var container = new StandaloneAzureBlobContainer(new Uri(_containerPath));

            Assert.Equal(new Uri(_containerPath), container.Uri);
        }

        [Fact]
        public void WillHaveCorrectUriWhenGivenUriWithToken()
        {
            var container = new StandaloneAzureBlobContainer(new Uri(_containerPath + "?some=token"));

            Assert.Equal(new Uri(_containerPath), container.Uri);
        }

        [Fact]
        public void CanDetermineContainerDoesNotExist()
        {
            var container = new StandaloneAzureBlobContainer(_containerPath);

            Assert.False(container.Exists());
        }

        [Fact]
        public void CanDetermineContainerExists()
        {
            var container = new StandaloneAzureBlobContainer(_containerPath);

            container.CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(container.Exists());
        }

        [Fact]
        public async Task CanDetermineContainerDoesNotExistAsync()
        {
            var container = new StandaloneAzureBlobContainer(_containerPath);

            Assert.False(await container.ExistsAsync());
        }

        [Fact]
        public async Task CanDetermineContainerExistsAsync()
        {
            var container = new StandaloneAzureBlobContainer(_containerPath);

            container.CreateIfNotExists(BlobContainerPublicAccessType.Off);

            Assert.True(await container.ExistsAsync());
        }

        [Fact]
        public void CanGetBlobInstance()
        {
            var container = new StandaloneAzureBlobContainer(_containerPath);
            container.CreateIfNotExists(BlobContainerPublicAccessType.Off);

            var blob = container.GetBlockBlobReference("testblob");

            new
            {
                Name = "testblob",
                Uri = new Uri(Path.Combine(_containerPath, "testblob"))
            }.ToExpectedObject().ShouldMatch(blob);
        }

        [Fact]
        public void WillThrowIfBlobNameNotGiven()
        {
            var container = new StandaloneAzureBlobContainer(_containerPath);

            Assert.Throws<ArgumentNullException>(() => container.GetBlockBlobReference(null));
        }

        [Fact]
        public void WillThrowIfBlobNameEmpty()
        {
            var container = new StandaloneAzureBlobContainer(_containerPath);

            Assert.Throws<ArgumentException>(() => container.GetBlockBlobReference(""));
        }

        [Theory]
        [InlineData(SharedAccessBlobPermissions.Delete, "sp=d")]
        [InlineData(SharedAccessBlobPermissions.List, "sp=l")]
        [InlineData(SharedAccessBlobPermissions.Read, "sp=r")]
        [InlineData(SharedAccessBlobPermissions.Write, "sp=w")]
        [InlineData(SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write, "sp=rw")]
        public void WillReturnEmptyStringForSharedAccessKeySignature(
            SharedAccessBlobPermissions permissions, 
            string expectedPermissions)
        {
            var container = new StandaloneAzureBlobContainer(new Uri(_containerPath));

            Assert.Contains(expectedPermissions, container.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = permissions
            }));
        }

        [Fact]
        public void WillThrowIfConstructorBlobStorageDirectoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StandaloneAzureBlobContainer(null, "container"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void WillThrowIfConstructorBlobStorageDirectoryIsEmpty(string blobStorageDirectory)
        {
            Assert.Throws<ArgumentException>(() => new StandaloneAzureBlobContainer(blobStorageDirectory, "container"));
        }

        [Fact]
        public void WillThrowIfConstructorContainerNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StandaloneAzureBlobContainer(BasePath, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void WillThrowIfConstructorContainerNameIsEmpty(string containerName)
        {
            Assert.Throws<ArgumentException>(() => new StandaloneAzureBlobContainer(BasePath, containerName));
        }

        [Fact]
        public void WillThrowIfConstructorContainerDirectoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StandaloneAzureBlobContainer((string) null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void WillThrowIfConstructorContainerDirectoryIsEmpty(string containerDirectory)
        {
            Assert.Throws<ArgumentException>(() => new StandaloneAzureBlobContainer(containerDirectory));
        }

        [Fact]
        public void WillThrowIfConstructorContainerUriIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StandaloneAzureBlobContainer((Uri) null));
        }
    }
}