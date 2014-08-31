using System;

using LightBlue.Hosted;
using LightBlue.Standalone;

namespace LightBlue
{
    public static class AzureBlockBlobFactory
    {
        public static IAzureBlockBlob Build(Uri blobUri)
        {
            if (blobUri.Scheme != "file")
            {
                return new HostedAzureBlockBlob(blobUri);
            }

            var locationParts = StandaloneEnvironment.SeparateBlobUri(blobUri);
            return new StandaloneAzureBlockBlob(locationParts.ContainerPath, locationParts.BlobPath);
        }
    }
}