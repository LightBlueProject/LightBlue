using System;
using System.IO;

using LightBlue.Standalone;

using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Standalone.BlobStorage
{
    public class StandaloneAzureBlobDirectoryTests : StandaloneAzureTestsBase
    {
        public StandaloneAzureBlobDirectoryTests()
            : base(DirectoryType.Container)
        {}

        [Theory]
        [InlineData("directory")]
        [InlineData(@"directory\subdirectory")]
        [InlineData("directory/subdirectory")]
        public void WillHaveCorrectUriWhenGivenDirectory(string directoryName)
        {
            var directoryPath = Path.Combine(BasePath, directoryName);

            var directory = new StandaloneAzureBlobDirectory(BasePath, directoryName);

            Assert.Equal(new Uri(directoryPath), directory.Uri);
        }

        [Fact]
        public void WillThrowIfConstructorContainerDirectoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StandaloneAzureBlobDirectory(null, "directory"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void WillThrowIfConstructorContainerDirectoryIsEmpty(string containerDirectory)
        {
            Assert.Throws<ArgumentException>(() => new StandaloneAzureBlobDirectory(containerDirectory, "directory"));
        }

        [Fact]
        public void WillThrowIfConstructorDirectoryNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StandaloneAzureBlobDirectory(BasePath, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void WillThrowIfConstructorDirectoryNNameIsEmpty(string directoryName)
        {
            Assert.Throws<ArgumentException>(() => new StandaloneAzureBlobDirectory(BasePath, directoryName));
        }


        [Fact]
        public void WillThrowIfBlobNameNotGiven()
        {
            var directory = new StandaloneAzureBlobDirectory(BasePath, "somedirectory");

            Assert.Throws<ArgumentNullException>(() => directory.GetBlockBlobReference(null));
        }

        [Fact]
        public void WillThrowIfBlobNameEmpty()
        {
            var directory = new StandaloneAzureBlobDirectory(BasePath, "somedirectory");

            Assert.Throws<ArgumentException>(() => directory.GetBlockBlobReference(""));
        }
    }
}