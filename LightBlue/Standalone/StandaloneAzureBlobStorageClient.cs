using System.IO;

using LightBlue.Infrastructure;

namespace LightBlue.Standalone
{
    public class StandaloneAzureBlobStorageClient : IAzureBlobStorageClient
    {
        private readonly string _blobStorageDirectory;

        public StandaloneAzureBlobStorageClient(string storageAccountDirectory)
        {
            StringValidation.NotNullOrWhitespace(storageAccountDirectory, "storageAccountDirectory");

            _blobStorageDirectory = Path.Combine(storageAccountDirectory, "blob");

            Directory.CreateDirectory(_blobStorageDirectory);
        }

        public IAzureBlobContainer GetContainerReference(string containerName)
        {
            NameValidation.Container(containerName, "containerName");

            return new StandaloneAzureBlobContainer(_blobStorageDirectory, containerName);
        }
    }
}