using System;
using System.Collections.Generic;
using System.Globalization;
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
            get { return _blobResultSegment.Results.Select(s =>
            {
                var cloudBlockBlob = s as CloudBlockBlob;
                if (cloudBlockBlob != null)
                {
                    return (IAzureListBlobItem) new HostedAzureBlockBlob(cloudBlockBlob);
                }
                var cloudBlobDirectory = s as CloudBlobDirectory;
                if (cloudBlobDirectory != null)
                {
                    return (IAzureListBlobItem) new HostedAzureBlobDirectory(cloudBlobDirectory);
                }

                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Unknown result type from list '{0}'.",
                        s.GetType()));
            }); }
        }

        public BlobContinuationToken ContinuationToken
        {
            get { return _blobResultSegment.ContinuationToken; }
        }
    }
}