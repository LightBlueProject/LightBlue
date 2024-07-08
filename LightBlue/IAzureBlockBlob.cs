using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
        Task SetPropertiesAsync();
        string GetSharedAccessReadSignature(DateTimeOffset expiresOn);
        string GetSharedAccessWriteSignature(DateTimeOffset expiresOn);
        string GetSharedAccessReadWriteSignature(DateTimeOffset expiresOn);
        void DownloadToStream(Stream target, CancellationToken cancellationToken = default);
        Task DownloadToStreamAsync(Stream target, CancellationToken cancellationToken = default);
        Task UploadFromStreamAsync(Stream source);
        Task UploadFromFileAsync(string path);
        Task UploadFromByteArrayAsync(byte[] buffer);
        Task UploadFromByteArrayAsync(byte[] buffer, int index, int count);
        string StartCopyFromBlob(IAzureBlockBlob source);
        string StartCopyFromBlob(Uri source);
    }
}