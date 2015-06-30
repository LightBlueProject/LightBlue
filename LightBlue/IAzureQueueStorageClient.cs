using System.Collections.Generic;

namespace LightBlue
{
    public interface IAzureQueueStorageClient
    {
        IAzureQueue GetQueueReference(string queueName);

        IEnumerable<IAzureQueue> ListQueues();
    }
}