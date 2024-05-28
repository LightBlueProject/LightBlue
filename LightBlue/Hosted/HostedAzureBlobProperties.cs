using Azure.Core;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBlue.Hosted
{
    public class HostedAzureBlobProperties : IAzureBlobProperties
    {
        public HostedAzureBlobProperties() : this(contentType: null, contentLength: 0)
        {
        }

        public HostedAzureBlobProperties(string contentType, int contentLength)
        {
            Length = contentLength;
            ContentType = contentType;
            Metadata = new Dictionary<string, string>();
        }

        public HostedAzureBlobProperties(BlobProperties blobProperties)
        {
            Length = blobProperties.ContentLength;
            ContentType = blobProperties.ContentType;
            Metadata = blobProperties.Metadata;
            CopyState = blobProperties.BlobCopyStatus != null
                ? new HostedAzureCopyState((LightBlueBlobCopyStatus)blobProperties.BlobCopyStatus, blobProperties.CopyStatusDescription)
                : null;
        }

        public HostedAzureBlobProperties(ResponseHeaders headers)
        {
            Length = headers.ContentLength ?? 0;
            ContentType = headers.ContentType;

            Metadata = headers
                .Where(h => h.Name.StartsWith("x-ms-meta-"))
                .ToDictionary(h => h.Name.Substring(10), h => h.Value);

            headers.TryGetValue("BlobCopyStatus", out var blobCopyStatusString);
            headers.TryGetValue("CopyStatusDescription", out var copyStatusDescription);
            CopyState = blobCopyStatusString != null && Enum.TryParse(blobCopyStatusString, out CopyStatus copyStatus)
                ? new HostedAzureCopyState((LightBlueBlobCopyStatus)copyStatus, copyStatusDescription)
                : null;
        }

        public long Length { get; private set; }

        public string ContentType { get; private set; }

        internal IDictionary<string, string> Metadata { get; private set; }

        internal HostedAzureCopyState CopyState { get; private set; }
    }
}