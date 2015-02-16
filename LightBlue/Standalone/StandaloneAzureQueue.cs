using System;
using System.IO;
using System.Threading.Tasks;

namespace LightBlue.Standalone
{
    public class StandaloneAzureQueue : IAzureQueue
    {
        private readonly string _queueDirectory;

        public StandaloneAzureQueue(string queueStorageDirectory, string queueName)
        {
            _queueDirectory = Path.Combine(queueStorageDirectory, queueName);
        }

        public StandaloneAzureQueue(string queueDirectory)
        {
            _queueDirectory = queueDirectory;
        }

        public Uri Uri
        {
            get { return null; }
        }

        public string Name
        {
            get { return null; }
        }

        public Task CreateIfNotExistsAsync()
        {
            Directory.CreateDirectory(_queueDirectory);

            return Task.FromResult(new object());
        }

        public Task DeleteAsync()
        {
            return null;
        }

        public Task DeleteIsExistsAsync()
        {
            return null;
        }

        public Task AddMessageAsync(IAzureQueueMessage message)
        {
            return null;
        }

        public Task<IAzureQueueMessage> GetMessageAsync()
        {
            return null;
        }

        public Task DeleteMessageAsync(IAzureQueueMessage message)
        {
            return null;
        }
    }
}