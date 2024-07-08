using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LightBlue.Standalone;
using Xunit;

namespace LightBlue.Tests.Standalone.QueueStorage
{
    public class GetMessageTests : StandaloneAzureTestsBase
    {
        private readonly IAzureQueue _queue;

        public GetMessageTests()
            : base(DirectoryType.Queue)
        {
            _queue = new StandaloneAzureQueue(BasePath);
        }

        [Fact]
        public async Task CanGetWrittenMessage()
        {
            await _queue.AddMessageAsync("Test message");

            var message = await _queue.GetMessageAsync();

            Assert.Equal("Test message", message.AsString);
        }

        [Fact]
        public async Task WillGetNothingIfQueueEmpty()
        {
            Assert.Null(await _queue.GetMessageAsync());
        }

        [Fact]
        public async Task WillGetFirstMessageWritten()
        {
            await _queue.AddMessageAsync("First test message");
            await _queue.AddMessageAsync("Second test message");

            var message = await _queue.GetMessageAsync();

            Assert.Equal("First test message", message.AsString);
        }

        [Fact]
        public async Task WillGetSecondMessageIfFirstLocked()
       {
           await _queue.AddMessageAsync("First test message");
            await _queue.AddMessageAsync("Second test message");

            await _queue.GetMessageAsync();
            var message = await _queue.GetMessageAsync();

            Assert.Equal("Second test message", message.AsString);
        }

        [Fact]
        public async Task WillAssignFilenameAsMessageId()
        {
            await _queue.AddMessageAsync("First test message");

            var fileName = Path.GetFileName(Directory.GetFiles(BasePath).OrderBy(f => f).First());

            var message = await _queue.GetMessageAsync();

            Assert.Equal(fileName, message.MessageId);
        }
    }
}