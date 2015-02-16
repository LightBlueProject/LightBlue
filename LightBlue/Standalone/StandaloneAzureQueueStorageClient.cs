using System.Collections.Generic;
using System.IO;

using LightBlue.Infrastructure;

namespace LightBlue.Standalone
{
    public class StandaloneAzureQueueStorageClient : IAzureQueueStorageClient
    {
        private readonly string _queueStorageDirectory;

        public StandaloneAzureQueueStorageClient(string storageAccountDirectory)
        {
            StringValidation.NotNullOrWhitespace(storageAccountDirectory, "storageAccountDirectory");

            _queueStorageDirectory = Path.Combine(storageAccountDirectory, "blob");

            Directory.CreateDirectory(_queueStorageDirectory);
        }

        public IAzureQueue GetQueueReference(string queueName)
        {
            return new StandaloneAzureQueue(_queueStorageDirectory, queueName);
        }

        public IEnumerable<IAzureQueue> ListQueues()
        {
            yield break;
        }
    }
}