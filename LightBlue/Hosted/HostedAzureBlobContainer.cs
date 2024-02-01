using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

namespace LightBlue.Hosted
{
    public class HostedAzureBlobContainer : IAzureBlobContainer
    {
        private readonly BlobContainerClient _cloudBlobContainer;

        public HostedAzureBlobContainer(BlobContainerClient cloudBlobContainer)
        {
            _cloudBlobContainer = cloudBlobContainer;
        }

        public HostedAzureBlobContainer(Uri containerUri)
        {
            _cloudBlobContainer = new BlobContainerClient(containerUri);
        }

        public HostedAzureBlobContainer(Uri containerUri, StorageSharedKeyCredential credentials)
        {
            _cloudBlobContainer = new BlobContainerClient(containerUri, credentials);
        }

        public Uri Uri
        {
            get { return _cloudBlobContainer.Uri; }
        }

        public bool CreateIfNotExists()
        {
            try
            {
                var createResponse = _cloudBlobContainer.CreateIfNotExists(PublicAccessType.None);
                if (createResponse == null)
                    return false; // ref https://github.com/Azure/azure-sdk-for-net/issues/9758#issuecomment-755779972
            }
            catch (RequestFailedException requestFailedException)
            when (requestFailedException.ErrorCode == BlobErrorCode.ContainerAlreadyExists)
            {
                return false; // can occur due to concurrency
            }

            return true;
        }

        public async Task<bool> CreateIfNotExistsAsync()
        {
            try
            {
                var createResponse = await _cloudBlobContainer.CreateIfNotExistsAsync(PublicAccessType.None, null, null).ConfigureAwait(false);
                if (createResponse == null)
                    return false; // ref https://github.com/Azure/azure-sdk-for-net/issues/9758#issuecomment-755779972
            }
            catch (RequestFailedException requestFailedException)
            when (requestFailedException.ErrorCode == BlobErrorCode.ContainerAlreadyExists)
            {
                return false; // can occur due to concurrency
            }

            return true;
        }

        public bool Exists()
        {
            return _cloudBlobContainer.Exists();
        }

        public async Task<bool> ExistsAsync()
        {
            return (await _cloudBlobContainer.ExistsAsync().ConfigureAwait(false)).Value;
        }

        public IAzureBlockBlob GetBlockBlobReference(string blobName)
        {
            return new HostedAzureBlockBlob(_cloudBlobContainer.GetBlockBlobClient(blobName));
        }

        public string GetSharedAccessSignature(BlobContainerSasPermissions permissions, DateTimeOffset expiresOn)
        {
            return _cloudBlobContainer.GenerateSasUri(permissions, expiresOn).Query;
        }

        public async Task<IAzureListBlobItem[]> GetBlobs(string prefix, BlobTraits blobTraits = BlobTraits.None, BlobStates blobStates = BlobStates.None, int maxResults = int.MaxValue)
        {
            return await _cloudBlobContainer.GetBlobsAsync(blobTraits, blobStates, prefix, CancellationToken.None)
                .Where(i => i.Properties.BlobType == BlobType.Block || i.Properties.BlobType == BlobType.Page)
                .Take(maxResults)
                .Select(i => i.Properties.BlobType == BlobType.Block
                    ? (IAzureListBlobItem)new HostedAzureBlockBlob(_cloudBlobContainer.GetBlockBlobClient(i.Name), i.Metadata)
                    : new HostedAzurePageBlob(_cloudBlobContainer.GetPageBlobClient(i.Name)))
                .ToArrayAsync()
                .ConfigureAwait(false);
        }

        public string GetSharedAccessReadSignature(DateTimeOffset expiresOn)
        {
            return GetSharedAccessSignature(BlobContainerSasPermissions.Read, expiresOn);
        }

        public string GetSharedAccessWriteSignature(DateTimeOffset expiresOn)
        {
            return GetSharedAccessSignature(BlobContainerSasPermissions.Write, expiresOn);
        }

        public string GetSharedAccessReadWriteSignature(DateTimeOffset expiresOn)
        {
            return GetSharedAccessSignature(BlobContainerSasPermissions.Read | BlobContainerSasPermissions.Write, expiresOn);
        }
    }
}