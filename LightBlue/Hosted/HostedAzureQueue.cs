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

        public Task DeleteIsExistsAsync()
        {
            return _cloudQueue.DeleteIfExistsAsync();
        }

        public Task AddMessageAsync(IAzureQueueMessage message)
        {
            return _cloudQueue.AddMessageAsync(message.ToCloudQueueMessage());
        }

        public async Task<IAzureQueueMessage> GetMessageAsync()
        {
            return new HostedAzureQueueMessage(await _cloudQueue.GetMessageAsync());
        }

        public Task DeleteMessageAsync(IAzureQueueMessage message)
        {
            return _cloudQueue.DeleteMessageAsync(message.ToCloudQueueMessage());
        }
    }
}