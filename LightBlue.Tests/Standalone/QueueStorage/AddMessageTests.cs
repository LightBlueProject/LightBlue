using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LightBlue.Standalone;
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
            await _queue.AddMessageAsync("Test message");

            var file = Directory.GetFiles(BasePath).FirstOrDefault();

            Assert.NotNull(file);
        }

        [Fact]
        public async Task WillWriteMessageContentToQueueFile()
        {
            await _queue.AddMessageAsync("Test message");

            var file = Directory.GetFiles(BasePath).First();

            Assert.Equal("Test message", File.ReadAllText(file));
        }
    }
}