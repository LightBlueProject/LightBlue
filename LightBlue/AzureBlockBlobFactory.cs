using System;

using LightBlue.Hosted;
using LightBlue.Standalone;

namespace LightBlue
{
    public static class AzureBlockBlobFactory
    {
        public static IAzureBlockBlob Build(Uri blobUri)
        {
            return blobUri.Scheme == "file"
                ? (IAzureBlockBlob)new StandaloneAzureBlockBlob(blobUri)
                : new HostedAzureBlockBlob(blobUri);
        }
    }
}