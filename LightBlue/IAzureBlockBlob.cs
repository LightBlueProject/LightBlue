using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue
{
    public interface IAzureBlockBlob : IAzureListBlobItem
    {
        string Name { get; }
        BlobProperties Properties { get; }
        IAzureCopyState CopyState { get; }
        IDictionary<string, string> Metadata { get; }

        bool Exists();
        Task<bool> ExistsAsync();
        void FetchAttributes();
        void SetMetadata();
        Task SetMetadataAsync();
        string GetSharedAccessSignature(SharedAccessBlobPolicy policy);
        Task DownloadToStreamAsync(Stream target);
        Task UploadFromStreamAsync(Stream source);
        Task UploadFromByteArrayAsync(byte[] buffer, int index, int count);
    }
}