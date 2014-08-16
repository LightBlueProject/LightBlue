using System.IO;

using LightBlue.Infrastructure;

namespace LightBlue.Standalone
{
    public class StandaloneAzureBlobStorageClient : IAzureBlobStorageClient
    {
        private readonly string _blobStorageDirectory;

        public StandaloneAzureBlobStorageClient(string storageAccountDirectory)
        {
            _blobStorageDirectory = Path.Combine(storageAccountDirectory, "blob");
            Directory.CreateDirectory(_blobStorageDirectory);
        }

        public IAzureBlobContainer GetBlockBlobReference(string containerName)
        {
            NameValidation.Container(containerName, "containerName");

            return new StandaloneAzureBlobContainer(_blobStorageDirectory, containerName);
        }
    }
}