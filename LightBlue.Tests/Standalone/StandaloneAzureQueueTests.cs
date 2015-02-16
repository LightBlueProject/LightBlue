using System.IO;
using System.Threading.Tasks;

using LightBlue.Standalone;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureQueueTests : StandaloneAzureTestsBase
    {
        private const string QueueName = "queue";

        private readonly string _queuePath;

        public StandaloneAzureQueueTests()
            : base(DirectoryType.Account)
        {
            _queuePath = Path.Combine(BasePath, QueueName);
        }

        [Fact]
        public void DoesNotCreateContainerDirectoryOnConstructionWithQueuePath()
        {
            new StandaloneAzureQueue(_queuePath);

            Assert.False(Directory.Exists(_queuePath));
        }

        [Fact]
        public void DoesNotCreateContainerDirectoryOnConstructionWithStorageDirectoryAndQueueName()
        {
            new StandaloneAzureQueue(BasePath, QueueName);

            Assert.False(Directory.Exists(_queuePath));
        }

        [Fact]
        public async Task WillCreateContainerPathOnCreateIfNotExists()
        {
            await new StandaloneAzureQueue(_queuePath).CreateIfNotExistsAsync();

            Assert.True(Directory.Exists(_queuePath));
        }

        [Fact]
        public async Task WillCreateContainerPathOnCreateIfNotExistsFromStorageDirectoryAndQueuePath()
        {
            await new StandaloneAzureQueue(BasePath, QueueName).CreateIfNotExistsAsync();

            Assert.True(Directory.Exists(_queuePath));
        }

    }
}