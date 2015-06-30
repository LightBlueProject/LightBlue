using System;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Queue;

namespace LightBlue.Hosted
{
    public class HostedAzureQueue : IAzureQueue
    {
        private readonly CloudQueue _cloudQueue;

        public HostedAzureQueue(CloudQueue cloudQueue)
        {
            _cloudQueue = cloudQueue;
        }

        public Uri Uri
        {
            get { return _cloudQueue.Uri; }
        }

        public string Name
        {
            get { return _cloudQueue.Name; }
        }

        public Task CreateIfNotExistsAsync()
        {
            return _cloudQueue.CreateIfNotExistsAsync();
        }

        public Task DeleteAsync()
        {
            return _cloudQueue.DeleteAsync();
        }

        public Task DeleteIfExistsAsync()
        {
            return _cloudQueue.DeleteIfExistsAsync();
        }

        public Task AddMessageAsync(CloudQueueMessage message)
        {
            return _cloudQueue.AddMessageAsync(message);
        }

        public Task<CloudQueueMessage> GetMessageAsync()
        {
            return _cloudQueue.GetMessageAsync();
        }

        public Task DeleteMessageAsync(CloudQueueMessage message)
        {
            return _cloudQueue.DeleteMessageAsync(message);
        }
    }
}