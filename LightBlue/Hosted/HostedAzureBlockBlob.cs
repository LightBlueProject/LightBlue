using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Hosted
{
    public class HostedAzureBlockBlob : IAzureBlockBlob
    {
        private readonly CloudBlockBlob _cloudBlockBlob;

        public HostedAzureBlockBlob(CloudBlockBlob cloudBlockBlob)
        {
            _cloudBlockBlob = cloudBlockBlob;
        }

        public HostedAzureBlockBlob(Uri blobUri)
        {
            _cloudBlockBlob = new CloudBlockBlob(blobUri);
        }

        public Uri Uri
        {
            get { return _cloudBlockBlob.Uri; }
        }

        public string Name
        {
            get { return _cloudBlockBlob.Name; }
        }

        public BlobProperties Properties
        {
            get { return _cloudBlockBlob.Properties; }
        }

        public IAzureCopyState CopyState
        {
            get
            {
                var copyState = _cloudBlockBlob.CopyState;
                return copyState != null
                    ? new HostedAzureCopyState(copyState)
                    : null;
            }
        }

        public IDictionary<string, string> Metadata
        {
            get { return _cloudBlockBlob.Metadata; }
        }

        public bool Exists()
        {
            return _cloudBlockBlob.Exists();
        }

        public Task<bool> ExistsAsync()
        {
            return _cloudBlockBlob.ExistsAsync();
        }

        public void FetchAttributes()
        {
            _cloudBlockBlob.FetchAttributes();
        }

        public void SetMetadata()
        {
            _cloudBlockBlob.SetMetadata();
        }

        public Task SetMetadataAsync()
        {
            return _cloudBlockBlob.SetMetadataAsync();
        }

        public string GetSharedAccessSignature(SharedAccessBlobPolicy policy)
        {
            return _cloudBlockBlob.GetSharedAccessSignature(policy);
        }

        public Task DownloadToStreamAsync(Stream target)
        {
            return _cloudBlockBlob.DownloadToStreamAsync(target);
        }

        public Task UploadFromStreamAsync(Stream source)
        {
            return _cloudBlockBlob.UploadFromStreamAsync(source);
        }

        public Task UploadFromByteArrayAsync(byte[] buffer, int index, int count)
        {
            return _cloudBlockBlob.UploadFromByteArrayAsync(buffer, index, count);
        }
    }
}