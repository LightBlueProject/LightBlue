using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Auth;
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

        public HostedAzureBlockBlob(Uri blobUri, StorageCredentials storageCredentials)
        {
            _cloudBlockBlob = new CloudBlockBlob(blobUri, storageCredentials);
        }

        public Uri Uri
        {
            get { return _cloudBlockBlob.Uri; }
        }

        public string Name
        {
            get { return _cloudBlockBlob.Name; }
        }

        public IAzureBlobProperties Properties
        {
            get
            {
                return new HostedAzureBlobProperties(_cloudBlockBlob.Properties);
            }
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

        public void Delete()
        {
            _cloudBlockBlob.Delete();
        }

        public Task DeleteAsync()
        {
            return _cloudBlockBlob.DeleteAsync();
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

        public Task FetchAttributesAsync()
        {
            return _cloudBlockBlob.FetchAttributesAsync();
        }

        public void SetMetadata()
        {
            _cloudBlockBlob.SetMetadata();
        }

        public Task SetMetadataAsync()
        {
            return _cloudBlockBlob.SetMetadataAsync();
        }

        public void SetProperties()
        {
            _cloudBlockBlob.SetProperties();
        }

        public Task SetPropertiesAsync()
        {
            return _cloudBlockBlob.SetPropertiesAsync();
        }

        public string GetSharedAccessSignature(SharedAccessBlobPolicy policy)
        {
            return _cloudBlockBlob.GetSharedAccessSignature(policy);
        }

        public void DownloadToStream(Stream target)
        {
            _cloudBlockBlob.DownloadToStream(target);
        }

        public Task DownloadToStreamAsync(Stream target)
        {
            return _cloudBlockBlob.DownloadToStreamAsync(target);
        }

        public Task UploadFromStreamAsync(Stream source)
        {
            return _cloudBlockBlob.UploadFromStreamAsync(source);
        }

        public Task UploadFromFileAsync(string path)
        {
            return _cloudBlockBlob.UploadFromFileAsync(path, FileMode.Open);
        }

        public Task UploadFromByteArrayAsync(byte[] buffer)
        {
            return _cloudBlockBlob.UploadFromByteArrayAsync(buffer, 0, buffer.Length);
        }

        public Task UploadFromByteArrayAsync(byte[] buffer, int index, int count)
        {
            return _cloudBlockBlob.UploadFromByteArrayAsync(buffer, index, count);
        }

        public string StartCopyFromBlob(IAzureBlockBlob source)
        {
            var hostedAzureBlockBlob = source as HostedAzureBlockBlob;
            if (hostedAzureBlockBlob == null)
            {
                throw new ArgumentException("Can only copy between blobs in the same hosting environment");
            }

            return _cloudBlockBlob.StartCopy(hostedAzureBlockBlob._cloudBlockBlob);
        }

        public string StartCopyFromBlob(Uri source)
        {
            return _cloudBlockBlob.StartCopy(source);
        }
    }
}