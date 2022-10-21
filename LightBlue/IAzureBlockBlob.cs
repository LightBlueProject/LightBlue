using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Azure.Storage;
using Azure.Storage.Sas;

namespace LightBlue
{
    public interface IAzureBlockBlob : IAzureListBlobItem
    {
        string Name { get; }
        IAzureBlobProperties Properties { get; }
        IAzureCopyState CopyState { get; }
        IDictionary<string, string> Metadata { get; }
        
        void Delete();
        Task DeleteAsync();
        bool Exists();
        Task<bool> ExistsAsync();
        void FetchAttributes();
        Task FetchAttributesAsync();
        Stream OpenRead();
        void SetMetadata();
        Task SetMetadataAsync();
        Task SetContentTypeAsync(string contentType);
        string GetSharedAccessSignature(BlobSasPermissions permissions, DateTimeOffset expiresOn);
        void DownloadToStream(Stream target, BlobRequestConditions conditions = default, StorageTransferOptions options = default, CancellationToken cancellationToken = default);
        Task DownloadToStreamAsync(Stream target, BlobRequestConditions conditions = default, StorageTransferOptions options = default, CancellationToken cancellationToken = default);
        Task UploadFromStreamAsync(Stream source);
        Task UploadFromFileAsync(string path);
        Task UploadFromByteArrayAsync(byte[] buffer);
        Task UploadFromByteArrayAsync(byte[] buffer, int index, int count);
        string StartCopyFromBlob(IAzureBlockBlob source);
        string StartCopyFromBlob(Uri source);
    }
}