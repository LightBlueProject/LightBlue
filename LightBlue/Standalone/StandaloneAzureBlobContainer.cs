using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using LightBlue.Infrastructure;

namespace LightBlue.Standalone
{
    public class StandaloneAzureBlobContainer : IAzureBlobContainer
    {
        private readonly string _containerDirectory;

        public StandaloneAzureBlobContainer(string blobStorageDirectory, string containerName)
        {
            StringValidation.NotNullOrWhitespace(blobStorageDirectory, "blobStorageDirectory");
            StringValidation.NotNullOrWhitespace(containerName, "containerName");

            _containerDirectory = Path.Combine(blobStorageDirectory, containerName);
        }

        public StandaloneAzureBlobContainer(string containerDirectory)
        {
            StringValidation.NotNullOrWhitespace(containerDirectory, "containerDirectory");

            _containerDirectory = containerDirectory;
        }

        public StandaloneAzureBlobContainer(Uri containerUri)
        {
            if (containerUri == null)
            {
                throw new ArgumentNullException("containerUri");
            }

            _containerDirectory = containerUri.GetLocalPathWithoutToken();
        }

        private string MetadataDirectory
        {
            get { return Path.Combine(_containerDirectory, ".meta"); }
        }

        public Uri Uri
        {
            get { return new Uri(_containerDirectory); }
        }

        public bool CreateIfNotExists()
        {
            Directory.CreateDirectory(_containerDirectory);
            Directory.CreateDirectory(MetadataDirectory);
            return true;
        }

        public Task<bool> CreateIfNotExistsAsync()
        {
            CreateIfNotExists();
            return Task.FromResult(true);
        }

        public bool Exists()
        {
            return Directory.Exists(_containerDirectory);
        }

        public Task<bool> ExistsAsync()
        {
            return Task.FromResult(Directory.Exists(_containerDirectory));
        }

        public IAzureBlockBlob GetBlockBlobReference(string blobName)
        {
            StringValidation.NotNullOrEmpty(blobName, "blobName");

            return new StandaloneAzureBlockBlob(_containerDirectory, blobName);
        }

        public string GetSharedAccessSignature(BlobContainerSasPermissions permissions, DateTimeOffset expiresOn)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "?sv={0:yyyy-MM-dd}&sr=c&sig=s&sp={1}",
                DateTime.Today,
                permissions.DeterminePermissionsString());
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

        public Task<IAzureListBlobItem[]> GetBlobs(string prefix, BlobTraits blobTraits = BlobTraits.None, BlobStates blobStates = BlobStates.None, int maxResults = int.MaxValue)
        {
            return Task.FromResult(StandaloneList.ListBlobsSegmentedAsync(
                        _containerDirectory,
                        prefix,
                        blobTraits,
                        maxResults)
                        .OfType<IAzureListBlobItem>()
                        .ToArray());
        }


    }
}