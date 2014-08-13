using System;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Hosted
{
    public class HostedAzureBlobDirectory : IAzureBlobDirectory
    {
        private readonly CloudBlobDirectory _cloudBlobDirectory;

        public HostedAzureBlobDirectory(CloudBlobDirectory cloudBlobDirectory)
        {
            _cloudBlobDirectory = cloudBlobDirectory;
        }

        public Uri Uri
        {
            get { return _cloudBlobDirectory.Uri; }
        }

        public IAzureBlockBlob GetBlockBlobReference(string blobName)
        {
            return new HostedAzureBlockBlob(_cloudBlobDirectory.GetBlockBlobReference(blobName));
        }
    }
}