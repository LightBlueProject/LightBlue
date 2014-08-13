using LightBlue.Infrastructure;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Hosted
{
    public class HostedAzureBlobStorage : IAzureBlobStorage
    {
        private readonly CloudBlobClient _cloudBlobClient;

        public HostedAzureBlobStorage(CloudBlobClient cloudBlobClient)
        {
            _cloudBlobClient = cloudBlobClient;
        }

        public IAzureBlobContainer GetAzureBlobContainer(string containerName)
        {
            NameValidation.Container(containerName, "containerName");

            return new HostedAzureBlobContainer(_cloudBlobClient.GetContainerReference(containerName));
        }
    }
}