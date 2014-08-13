using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Standalone
{
    public class StandaloneAzureBlockBlob : IAzureBlockBlob
    {
        private const int BufferSize = 4096;

        private readonly string _blobName;
        private readonly string _blobPath;

        public StandaloneAzureBlockBlob(string containerDirectory, string blobName)
        {
            _blobName = blobName;
            _blobPath = Path.Combine(containerDirectory, blobName);
        }

        public StandaloneAzureBlockBlob(Uri blobUri)
        {
            _blobName = new FileInfo(blobUri.LocalPath).Name;
            _blobPath = blobUri.LocalPath;
        }

        public Uri Uri
        {
            get { return new Uri(_blobPath); }
        }

        public string Name
        {
            get { return _blobName; }
        }

        public BlobProperties Properties { get; private set; }
        public IAzureCopyState CopyState { get; private set; }

        public IDictionary<string, string> Metadata
        {
            get {  return new Dictionary<string, string>(); }
        }

        public bool Exists()
        {
            return File.Exists(_blobPath);
        }

        public Task<bool> ExistsAsync()
        {
            return Task.FromResult(File.Exists(_blobPath));
        }

        public void FetchAttributes()
        {}

        public void SetMetadata()
        {}

        public Task SetMetadataAsync()
        {
            return Task.FromResult(new object());
        }

        public string GetSharedAccessSignature(SharedAccessBlobPolicy policy)
        {
            return "";
        }

        public async Task DownloadToStreamAsync(Stream target)
        {
            using (var fileStream = new FileStream(_blobPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, BufferSize, true))
            {
                var buffer = new byte[BufferSize];
                int bytesRead;
                do
                {
                    bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                    await target.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                } while (bytesRead == BufferSize);
            }
        }

        public async Task UploadFromStreamAsync(Stream source)
        {
            using (var fileStream = new FileStream(_blobPath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true))
            {
                var buffer = new byte[BufferSize];
                int bytesRead;
                do
                {
                    bytesRead = await source.ReadAsync(buffer, 0, buffer.Length);

                    await fileStream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                } while (bytesRead == BufferSize);
            }
        }

        public async Task UploadFromByteArrayAsync(byte[] buffer, int index, int count)
        {
            using (var fileStream = new FileStream(_blobPath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true))
            {
                await fileStream.WriteAsync(buffer, index, count).ConfigureAwait(false);
            }
        }
    }
}