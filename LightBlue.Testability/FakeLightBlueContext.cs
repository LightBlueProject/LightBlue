using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Auth;

namespace LightBlue.Testability
{
    public class FakeLightBlueContext : ILightBlueContext
    {
        public Func<string, IAzureStorage> StorageProvider { get; set; }
        public Func<Uri, IAzureBlobContainer> BlobContainerProvider { get; set; }
        public Func<Uri, StorageCredentials, IAzureBlobContainer> BlobContainerProviderWithCredentials { get; set; }
        public Func<Uri, IAzureBlockBlob> BlockBlobProvider { get; set; }
        public Func<Uri, StorageCredentials, IAzureBlockBlob> BlockBlobWithCredentials { get; set; }
        public Func<Uri, IAzureQueue> QueueProvider { get; set; }

        public string RoleName { get; set; }
        public IAzureSettings Settings { get; set; }
        public AzureEnvironment AzureEnvironment { get; set; }

        public IAzureStorage GetStorageAccount(string connectionString)
        {
            CheckPropertyIsSet(() => StorageProvider);
            
            return StorageProvider(connectionString);
        }

        public IAzureBlobContainer GetBlobContainer(Uri containerUri)
        {
            CheckPropertyIsSet(() => BlobContainerProvider);

            return BlobContainerProvider(containerUri);
        }

        public IAzureBlobContainer GetBlobContainer(Uri containerUri, StorageCredentials storageCredentials)
        {
            CheckPropertyIsSet(() => BlobContainerProviderWithCredentials);

            return BlobContainerProviderWithCredentials(containerUri, storageCredentials);
        }

        public IAzureBlockBlob GetBlockBlob(Uri blobUri)
        {
            CheckPropertyIsSet(() => BlockBlobProvider);

            return BlockBlobProvider(blobUri);
        }

        public IAzureBlockBlob GetBlockBlob(Uri blobUri, StorageCredentials storageCredentials)
        {
            CheckPropertyIsSet(() => BlockBlobWithCredentials);

            return BlockBlobWithCredentials(blobUri, storageCredentials);
        }

        public IAzureQueue GetQueue(Uri queueUri)
        {
            CheckPropertyIsSet(() => QueueProvider);

            return QueueProvider(queueUri);
        }

        private void CheckPropertyIsSet<T>(Expression<Func<T>> expression)
        {
            var body = expression.Body as MemberExpression;
            var member = body.Member as PropertyInfo;
            var propValue = member.GetValue(this);

            var propertyName = GetNameFromProperty(expression);

            if (propValue == null)
                throw new NotSupportedException(string.Format("'{0}' not set, if you want to use this, set the property first.", propertyName));
        }

        private string GetNameFromProperty<T>(Expression<Func<T>> exp)
        {
            var body = exp.Body as MemberExpression;

            if (body != null)
                return body.Member.Name;

            var ubody = (UnaryExpression)exp.Body;

            body = ubody.Operand as MemberExpression;

            return body.Member.Name;
        }
    }
}