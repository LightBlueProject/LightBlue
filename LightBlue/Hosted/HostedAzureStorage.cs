using Azure.Storage.Blobs;
using Azure.Storage.Queues;

namespace LightBlue.Hosted
{
    public class HostedAzureStorage : IAzureStorage
    {
        private readonly string _connectionString;

        public HostedAzureStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IAzureBlobStorageClient CreateAzureBlobStorageClient()
        {
            return new HostedAzureBlobStorageClient(new BlobServiceClient(_connectionString));
        }

        public IAzureQueueStorageClient CreateAzureQueueStorageClient()
        {
            var queueServiceClient = new QueueServiceClient(_connectionString,
                options: new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 }); // for backwards compatability with v11 storage library

            return new HostedAzureQueueStorageClient(queueServiceClient);
        }
    }
}