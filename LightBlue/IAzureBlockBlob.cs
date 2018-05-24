using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

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
        void SetProperties();
        Task SetPropertiesAsync();
        string GetSharedAccessSignature(SharedAccessBlobPolicy policy);
        void DownloadToStream(Stream target, AccessCondition accessCondition = null, BlobRequestOptions options = null, OperationContext operationContext = null);
        Task DownloadToStreamAsync(Stream target, AccessCondition accessCondition = null, BlobRequestOptions options = null, OperationContext operationContext = null);
        Task UploadFromStreamAsync(Stream source);
        Task UploadFromFileAsync(string path);
        Task UploadFromByteArrayAsync(byte[] buffer);
        Task UploadFromByteArrayAsync(byte[] buffer, int index, int count);
        string StartCopyFromBlob(IAzureBlockBlob source);
        string StartCopyFromBlob(Uri source);
    }
}