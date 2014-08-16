using LightBlue.Infrastructure;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Hosted
{
    public class HostedAzureBlobStorageClient : IAzureBlobStorageClient
    {
        private readonly CloudBlobClient _cloudBlobClient;

        public HostedAzureBlobStorageClient(CloudBlobClient cloudBlobClient)
        {
            _cloudBlobClient = cloudBlobClient;
        }

        public IAzureBlobContainer GetBlockBlobReference(string containerName)
        {
            NameValidation.Container(containerName, "containerName");

            return new HostedAzureBlobContainer(_cloudBlobClient.GetContainerReference(containerName));
        }
    }
}