using System.IO;
using System.Linq;
using System.Threading.Tasks;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage.Queue;

using Xunit;

namespace LightBlue.Tests.Standalone.QueueStorage
{
    public class AddMessageTests : StandaloneAzureTestsBase
    {
        private readonly IAzureQueue _queue;

        public AddMessageTests()
            : base(DirectoryType.Queue)
        {
            _queue = new StandaloneAzureQueue(BasePath);
        }

        [Fact]
        public async Task WillWriteMessageFileToQueueDirectory()
        {
            var message = new CloudQueueMessage("Test message");

            await _queue.AddMessageAsync(message);

            var file = Directory.GetFiles(BasePath).FirstOrDefault();

            Assert.NotNull(file);
        }

        [Fact]
        public async Task WillWriteMessageContentToQueueFile()
        {
            var message = new CloudQueueMessage("Test message");

            await _queue.AddMessageAsync(message);

            var file = Directory.GetFiles(BasePath).First();

            Assert.Equal("Test message", File.ReadAllText(file));
        }
    }
}