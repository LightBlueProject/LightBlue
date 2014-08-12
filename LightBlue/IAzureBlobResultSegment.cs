using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue
{
    public interface IAzureBlobResultSegment
    {
        IEnumerable<IAzureListBlobItem> Results { get; }
        BlobContinuationToken ContinuationToken { get; }
    }
}