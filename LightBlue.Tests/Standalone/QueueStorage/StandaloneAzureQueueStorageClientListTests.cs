using System.Linq;
using System.Threading.Tasks;

using LightBlue.Standalone;

using Xunit;

namespace LightBlue.Tests.Standalone.QueueStorage
{
    public class StandaloneAzureQueueStorageClientListTests : StandaloneAzureTestsBase
    {
        private readonly StandaloneAzureQueueStorageClient _client;
        public StandaloneAzureQueueStorageClientListTests()
            : base(DirectoryType.Account)
        {
            _client = new StandaloneAzureQueueStorageClient(BasePath);
        }

        [Fact]
        public void WillReturnEmptyListWhenNoQueues()
        {
            Assert.Empty(_client.ListQueues());
        }

        [Fact]
        public async Task WillListQueue()
        {
            var queue = _client.GetQueueReference("testqueue");
            await queue.CreateIfNotExistsAsync();

            Assert.Equal(queue.Uri.LocalPath, _client.ListQueues().First().Uri.LocalPath);
        }

        [Fact]
        public async Task WillListMultipleQueues()
        {
            await _client.GetQueueReference("testqueue1").CreateIfNotExistsAsync();
            await _client.GetQueueReference("testqueue2").CreateIfNotExistsAsync();
            await _client.GetQueueReference("testqueue3").CreateIfNotExistsAsync();

            Assert.Equal(3, _client.ListQueues().Count());
        }
    }
}