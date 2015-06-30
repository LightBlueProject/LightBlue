using Microsoft.WindowsAzure.Storage;

namespace LightBlue.Hosted
{
    public class HostedAzureStorage : IAzureStorage
    {
        private readonly CloudStorageAccount _cloudStorageAccount;

        public HostedAzureStorage(string connectionString)
        {
            _cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public IAzureBlobStorageClient CreateAzureBlobStorageClient()
        {
            return new HostedAzureBlobStorageClient(_cloudStorageAccount.CreateCloudBlobClient());
        }

        public IAzureQueueStorageClient CreateAzureQueueStorageClient()
        {
            return new HostedAzureQueueStorageClient(_cloudStorageAccount.CreateCloudQueueClient());
        }
    }
}