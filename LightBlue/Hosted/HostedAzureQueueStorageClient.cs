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
            QueueContinuationToken token = null;
            var result = new List<CloudQueue>();
            do
            {
                var segment = _cloudQueueClient
                    .ListQueuesSegmentedAsync(token)
                    .GetAwaiter()
                    .GetResult();
                result.AddRange(segment.Results);
                token = segment.ContinuationToken;
            } while (token != null);

            return result.Select(q => new HostedAzureQueue(q));
        }
    }
}