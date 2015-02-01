using System.Collections.Generic;
using System.Linq;

using Microsoft.WindowsAzure.Storage.Queue;

namespace LightBlue.Hosted
{
    public class HostedAzureQueueStorageClient : IAzureQueueStorageClient
    {
        private readonly CloudQueueClient _cloudQueueClient;

        public HostedAzureQueueStorageClient(CloudQueueClient cloudQueueClient)
        {
            _cloudQueueClient = cloudQueueClient;
        }

        public IAzureQueue GetQueueReference(string queueName)
        {
            return new HostedAzureQueue(_cloudQueueClient.GetQueueReference(queueName));
        }

        public IEnumerable<IAzureQueue> ListQueues()
        {
            return _cloudQueueClient.ListQueues()
                .Select(q => new HostedAzureQueue(q))
                .ToArray();
        }
    }
}