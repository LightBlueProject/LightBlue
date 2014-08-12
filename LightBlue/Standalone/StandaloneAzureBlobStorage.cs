using System.IO;

namespace LightBlue.Standalone
{
    public class StandaloneAzureBlobStorage : IAzureBlobStorage
    {
        private readonly string _blobStorageDirectory;

        public StandaloneAzureBlobStorage(string storageAccountDirectory)
        {
            _blobStorageDirectory = Path.Combine(storageAccountDirectory, "blob");
            Directory.CreateDirectory(_blobStorageDirectory);
        }

        public IAzureBlobContainer GetAzureBlobContainer(string containerName)
        {
            return new StandaloneAzureBlobContainer(_blobStorageDirectory, containerName);
        }
    }
}