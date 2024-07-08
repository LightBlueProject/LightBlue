using Azure.Storage.Blobs;
using LightBlue.Infrastructure;

namespace LightBlue.Hosted
{
    public class HostedAzureBlobStorageClient : IAzureBlobStorageClient
    {
        private readonly BlobServiceClient _cloudBlobClient;

        public HostedAzureBlobStorageClient(BlobServiceClient cloudBlobClient)
        {
            _cloudBlobClient = cloudBlobClient;
        }

        public IAzureBlobContainer GetContainerReference(string containerName)
        {
            NameValidation.Container(containerName, "containerName");

            return new HostedAzureBlobContainer(_cloudBlobClient.GetBlobContainerClient(containerName));
        }
    }
}