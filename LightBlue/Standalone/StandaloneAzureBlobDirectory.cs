using System;

namespace LightBlue.Standalone
{
    public class StandaloneAzureBlobDirectory : IAzureBlobDirectory
    {
        private readonly string _blobDirectoryPath;

        public StandaloneAzureBlobDirectory(string blobDirectoryPath)
        {
            _blobDirectoryPath = blobDirectoryPath;
        }

        public Uri Uri { get { return new Uri(_blobDirectoryPath);} }

        public IAzureBlockBlob GetBlockBlobReference(string blobName)
        {
            return new StandaloneAzureBlockBlob(_blobDirectoryPath, blobName);
        }
    }
}