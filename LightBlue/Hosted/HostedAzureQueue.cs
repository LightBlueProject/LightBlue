using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Azure.Storage.Sas;
using System;
using System.Threading.Tasks;

namespace LightBlue.Hosted
{
    public class HostedAzureQueue : IAzureQueue
    {
        private readonly QueueClient _cloudQueue;

        public HostedAzureQueue(QueueClient cloudQueue)
        {
            _cloudQueue = cloudQueue;
        }

        public HostedAzureQueue(Uri queueUri)
        {
            _cloudQueue = new QueueClient(queueUri,
                options: new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 }); // for backwards compatability with v11 storage library defaults
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

        public Task AddMessageAsync(string message)
        {
            return _cloudQueue.SendMessageAsync(message);
        }

        public async Task<QueueMessage> GetMessageAsync()
        {
            return await _cloudQueue.ReceiveMessageAsync().ConfigureAwait(false);
        }

        public Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            return _cloudQueue.DeleteMessageAsync(messageId, popReceipt);
        }

        public string GetSharedAccessSignature(QueueSasPermissions permissions, DateTimeOffset expiresOn)
        {
            return _cloudQueue.GenerateSasUri(permissions, expiresOn).Query;
        }
    }
}