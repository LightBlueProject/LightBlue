using System;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Standalone
{
    public class StandaloneAzureBlobResultSegment : IAzureBlobResultSegment
    {
        public StandaloneAzureBlobResultSegment(IEnumerable<IAzureListBlobItem> results, BlobContinuationToken continuationToken)
        {
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            if (continuationToken == null)
            {
                throw new ArgumentNullException("continuationToken");
            }
            Results = results;
            ContinuationToken = continuationToken;
        }

        public IEnumerable<IAzureListBlobItem> Results { get; private set; }
        public BlobContinuationToken ContinuationToken { get; private set; }
    }
}