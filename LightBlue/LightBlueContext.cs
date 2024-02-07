using System;
using Azure;
using Azure.Storage;
using LightBlue.Setup;

namespace LightBlue
{
    public static class LightBlueContext
    {
        private static ILightBlueContext _context;

        public static string RoleName
        {
            get
            {
                Initialise();
                return _context.RoleName;
            }
        }

        public static IAzureSettings AzureSettings
        {
            get
            {
                Initialise();
                return _context.Settings;
            }
        }

        public static AzureEnvironment AzureEnvironment
        {
            get
            {
                Initialise();
                return _context.AzureEnvironment;
            }
        }

        public static IAzureStorage GetStorageAccount(string connectionString)
        {
            Initialise();
            return _context.GetStorageAccount(connectionString);
        }

        public static IAzureBlobContainer GetBlobContainer(Uri containerUri)
        {
            Initialise();
            return _context.GetBlobContainer(containerUri);
        }

        public static IAzureBlobContainer GetBlobContainer(Uri containerUri, string accountName, string accountKey)
        {
            Initialise();
            return _context.GetBlobContainer(containerUri, new StorageSharedKeyCredential(accountName, accountKey));
        }

        public static IAzureBlockBlob GetBlockBlob(Uri blobUri)
        {
            Initialise();
            return _context.GetBlockBlob(blobUri);
        }

        public static IAzureBlockBlob GetBlockBlob(Uri blobUri, string accountName, string accountKey)
        {
            Initialise();
            return _context.GetBlockBlob(blobUri, new StorageSharedKeyCredential(accountName, accountKey));
        }

        public static IAzureBlockBlob GetBlockBlob(Uri blobUri, string sharedAccessSignature)
        {
            Initialise();
            return _context.GetBlockBlob(blobUri, new AzureSasCredential(sharedAccessSignature));
        }

        public static IAzureQueue GetQueue(Uri containerUri)
        {
            Initialise();
            return _context.GetQueue(containerUri);
        }

        private static void Initialise()
        {
            if (_context == null)
            {
                _context = LightBlueConfiguration.GetConfiguredContext();
            }
        }
    }
}