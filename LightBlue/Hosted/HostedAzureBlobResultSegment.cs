using System.Collections.Generic;
using System.Linq;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Hosted
{
    public class HostedAzureBlobResultSegment : IAzureBlobResultSegment
    {
        private readonly BlobResultSegment _blobResultSegment;

        public HostedAzureBlobResultSegment(BlobResultSegment blobResultSegment)
        {
            _blobResultSegment = blobResultSegment;
        }

        public IEnumerable<IAzureListBlobItem> Results
        {
            get { return _blobResultSegment.Results.OfType<CloudBlockBlob>().Select(s => new HostedAzureBlockBlob(s));  }
        }

        public BlobContinuationToken ContinuationToken
        {
            get { return _blobResultSegment.ContinuationToken; }
        }
    }
}