using System;

using LightBlue.Standalone;

using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Standalone.BlobStorage
{
    public class StandaloneAzureBlobStorageClientTests
    {
        [Fact]
        public void WillThrowIfConstructorStorageAccountDirectoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StandaloneAzureBlobStorageClient(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void WillThrowIfConstructorStorageAccountDirectoryIsEmpty(string storageAccountDirectory)
        {
            Assert.Throws<ArgumentException>(() => new StandaloneAzureBlobStorageClient(storageAccountDirectory));
        }
    }
}