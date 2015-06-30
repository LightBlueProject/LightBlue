using System.IO;
using System.Threading.Tasks;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage.Queue;

using Xunit;

namespace LightBlue.Tests.Standalone.QueueStorage
{
    public class DeleteMessageTests : StandaloneAzureTestsBase
    {
        private readonly IAzureQueue _queue;

        public DeleteMessageTests()
            : base(DirectoryType.Queue)
        {
            _queue = new StandaloneAzureQueue(BasePath);
        }

        [Fact]
        public async Task DeleteWillRemoveFile()
        {
            await _queue.AddMessageAsync(new CloudQueueMessage("First test message"));

            var message = await _queue.GetMessageAsync();

            await _queue.DeleteMessageAsync(message);

            var files = Directory.GetFiles(BasePath);
            Assert.Empty(files);
        }
    }
}