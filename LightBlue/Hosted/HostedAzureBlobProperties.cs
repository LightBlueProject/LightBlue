using Azure.Storage.Blobs.Models;

namespace LightBlue.Hosted
{
    public class HostedAzureBlobProperties : IAzureBlobProperties
    {
        private readonly BlobProperties _blobProperties;

        public HostedAzureBlobProperties(BlobProperties blobProperties)
        {
            _blobProperties = blobProperties;
        }

        public long Length { get { return _blobProperties.ContentLength; } }

        public string ContentType
        {
            get { return _blobProperties.ContentType; }
        }
    }
}