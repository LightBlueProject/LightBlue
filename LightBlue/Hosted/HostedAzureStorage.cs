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

        public IAzureBlobStorage CreateAzureBlobStorageClient()
        {
            return new HostedAzureBlobStorage(_cloudStorageAccount.CreateCloudBlobClient()      );
        }
    }
}