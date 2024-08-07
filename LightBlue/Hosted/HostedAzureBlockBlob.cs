﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

namespace LightBlue.Hosted
{
    public class HostedAzureBlockBlob : IAzureBlockBlob
    {
        private readonly BlockBlobClient _cloudBlockBlob;
        private HostedAzureBlobProperties _properties = new HostedAzureBlobProperties();

        public HostedAzureBlockBlob(BlockBlobClient cloudBlockBlob, IDictionary<string, string> metadata = null)
        {
            _cloudBlockBlob = cloudBlockBlob;

            if (metadata?.Any() == true)
            {
                foreach (var m in metadata)
                    _properties.Metadata[m.Key] = m.Value;
            }
        }

        public HostedAzureBlockBlob(Uri blobUri)
        {
            _cloudBlockBlob = new BlockBlobClient(blobUri);
        }

        public HostedAzureBlockBlob(Uri blobUri, StorageSharedKeyCredential storageCredentials)
        {
            _cloudBlockBlob = new BlockBlobClient(blobUri, storageCredentials);
        }

        public HostedAzureBlockBlob(Uri blobUri, AzureSasCredential storageCredentials)
        {
            _cloudBlockBlob = new BlockBlobClient(blobUri, storageCredentials);
        }

        public Uri Uri => _cloudBlockBlob.Uri;
        public string Name => _cloudBlockBlob.Name;
        public IAzureBlobProperties Properties => _properties;
        public IAzureCopyState CopyState => _properties.CopyState;
        public IDictionary<string, string> Metadata => _properties.Metadata;

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

        public async Task<bool> ExistsAsync()
        {
            return (await _cloudBlockBlob.ExistsAsync().ConfigureAwait(false)).Value;
        }

        public void FetchAttributes()
        {
            _properties = new HostedAzureBlobProperties(_cloudBlockBlob.GetProperties().Value);
        }

        public async Task FetchAttributesAsync()
        {
            _properties = new HostedAzureBlobProperties(await _cloudBlockBlob.GetPropertiesAsync().ConfigureAwait(false));
        }

        public Stream OpenRead()
        {
            var download = _cloudBlockBlob.DownloadStreaming().Value;
            _properties = new HostedAzureBlobProperties(download.Details);
            return download.Content;
        }

        public void SetMetadata()
        {
            _cloudBlockBlob.SetMetadata(_properties.Metadata);
        }

        public Task SetMetadataAsync()
        {
            return _cloudBlockBlob.SetMetadataAsync(_properties.Metadata);
        }

        public async Task SetPropertiesAsync()
        {
            await _cloudBlockBlob.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = _properties.ContentType });
        }

        public string GetSharedAccessReadSignature(DateTimeOffset expiresOn)
        {
            return _cloudBlockBlob.GenerateSasUri(BlobSasPermissions.Read, expiresOn).Query;
        }

        public string GetSharedAccessWriteSignature(DateTimeOffset expiresOn)
        {
            return _cloudBlockBlob.GenerateSasUri(BlobSasPermissions.Write, expiresOn).Query;
        }

        public string GetSharedAccessReadWriteSignature(DateTimeOffset expiresOn)
        {
            return _cloudBlockBlob.GenerateSasUri(BlobSasPermissions.Read | BlobSasPermissions.Write, expiresOn).Query;
        }

        public void DownloadToStream(Stream target, CancellationToken cancellationToken = default)
        {
            var response = _cloudBlockBlob.DownloadTo(target, cancellationToken);
            _properties = new HostedAzureBlobProperties(response.Headers);
        }

        public async Task DownloadToStreamAsync(Stream target, CancellationToken cancellationToken = default)
        {
            var response = await _cloudBlockBlob.DownloadToAsync(target, cancellationToken).ConfigureAwait(false);
            _properties = new HostedAzureBlobProperties(response.Headers);
        }

        public Task UploadFromStreamAsync(Stream source)
        {
            return _cloudBlockBlob.UploadAsync(source, GenerateBlobUploadOptions());
        }

        public async Task UploadFromFileAsync(string path)
        {
            using (var source = File.OpenRead(path))
            {
                await _cloudBlockBlob.UploadAsync(source, GenerateBlobUploadOptions()).ConfigureAwait(false);
            }
        }

        public async Task UploadFromByteArrayAsync(byte[] buffer)
        {
            using (var source = new MemoryStream(buffer))
            {
                await _cloudBlockBlob.UploadAsync(source, GenerateBlobUploadOptions()).ConfigureAwait(false);
            }
        }

        public Task UploadFromByteArrayAsync(byte[] buffer, int index, int count)
        {
            return UploadFromByteArrayAsync(buffer.Skip(index).Take(count).ToArray());
        }

        public string StartCopyFromBlob(IAzureBlockBlob source)
        {
            if (!(source is HostedAzureBlockBlob hostedAzureBlockBlob))
            {
                throw new ArgumentException("Can only copy between blobs in the same hosting environment");
            }

            return _cloudBlockBlob.StartCopyFromUri(hostedAzureBlockBlob.Uri).Id;
        }

        public string StartCopyFromBlob(Uri source)
        {
            return _cloudBlockBlob.StartCopyFromUri(source).Id;
        }

        private BlobUploadOptions GenerateBlobUploadOptions()
        {
            return new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders()
                {
                    ContentType = _properties.ContentType
                },
                Metadata = _properties.Metadata
            };
        }
    }
}