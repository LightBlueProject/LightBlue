using System;

using LightBlue.Standalone;

using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Standalone.QueueStorage
{
    public class StandaloneAzureQueueStorageClientTests
    {
        [Fact]
        public void WillThrowIfConstructorStorageAccountDirectoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StandaloneAzureQueueStorageClient(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void WillThrowIfConstructorStorageAccountDirectoryIsEmpty(string storageAccountDirectory)
        {
            Assert.Throws<ArgumentException>(() => new StandaloneAzureQueueStorageClient(storageAccountDirectory));
        }
    }
}