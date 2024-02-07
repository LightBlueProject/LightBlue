using Azure.Storage.Queues;
using System.Collections.Generic;
using System.Linq;

namespace LightBlue.Hosted
{
    public class HostedAzureQueueStorageClient : IAzureQueueStorageClient
    {
        private readonly QueueServiceClient _cloudQueueClient;

        public HostedAzureQueueStorageClient(QueueServiceClient cloudQueueClient)
        {
            _cloudQueueClient = cloudQueueClient;
        }

        public IAzureQueue GetQueueReference(string queueName)
        {
            return new HostedAzureQueue(_cloudQueueClient.GetQueueClient(queueName));
        }

        public IEnumerable<IAzureQueue> ListQueues()
        {
            return _cloudQueueClient.GetQueues()
                .Select(q => new HostedAzureQueue(_cloudQueueClient.GetQueueClient(q.Name)))
                .ToArray();
        }
    }
}