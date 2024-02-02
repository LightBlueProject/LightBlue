using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Sas;

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

        public async Task<LightBlueQueueMessage> GetMessageAsync()
        {
            var response = await _cloudQueue.ReceiveMessageAsync();
            var cloudMessage = response.Value;
            return new LightBlueQueueMessage
            {
                MessageId = cloudMessage.MessageId,
                Body = cloudMessage.Body,
                PopReceipt = cloudMessage.PopReceipt
            };
        }

        public Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            return _cloudQueue.DeleteMessageAsync(messageId, popReceipt);
        }

        public string GetSharedAccessReadSignature(DateTimeOffset expiresOn)
        {
            return _cloudQueue.GenerateSasUri(QueueSasPermissions.Read, expiresOn).Query;
        }
    }
}