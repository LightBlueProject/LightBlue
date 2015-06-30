using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using LightBlue.Standalone;

namespace LightBlue.Tests.Standalone
{
    public enum DirectoryType
    {
        Account = 0,
        BlobStorage = 1,
        Container = 2,
        QueueStorage = 3,
        Queue = 4
    }

    public abstract class StandaloneAzureTestsBase
    {
        public const string MetadataDirectory = ".meta";

        protected readonly string BasePath;
        private readonly string _appDataDirectory;

        protected StandaloneAzureTestsBase(DirectoryType directoryType)
        {
            _appDataDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var lightBlueDirectory = Path.Combine(_appDataDirectory, "LightBlue");
            var accountDirectory = Path.Combine(lightBlueDirectory, "dev");
            var blobDirectory = Path.Combine(accountDirectory, "blob");
            var queuesDirectory = Path.Combine(accountDirectory, "queuse");

            StandaloneEnvironment.LightBlueDataDirectory = lightBlueDirectory;

            Directory.CreateDirectory(blobDirectory);
            Directory.CreateDirectory(queuesDirectory);

            switch (directoryType)
            {
                case DirectoryType.Account:
                    BasePath = accountDirectory;
                    break;
                case DirectoryType.BlobStorage:
                    BasePath = blobDirectory;
                    break;
                case DirectoryType.Container:
                    var containerDirectory = Path.Combine(blobDirectory, "container");
                    Directory.CreateDirectory(Path.Combine(containerDirectory, MetadataDirectory));
                    BasePath = containerDirectory;
                    break;
                case DirectoryType.QueueStorage:
                    BasePath = queuesDirectory;
                    break;
                case DirectoryType.Queue:
                    var queueDirectory = Path.Combine(queuesDirectory, "queue");
                    Directory.CreateDirectory(Path.Combine(queueDirectory, MetadataDirectory));
                    BasePath = queueDirectory;
                    break;
            }
        }

        protected Uri BasePathUri
        {
            get {  return new Uri(BasePath); }
        }

        public static IEnumerable<object[]> BlobNames
        {
            get
            {
                yield return new object[] {"someblob"};
                yield return new object[] {@"with\path\blob"};
                yield return new object[] {"with/alternate/separator"};
            }
        }

        public void Dispose()
        {
            StandaloneEnvironment.SetStandardLightBlueDataDirectory();
            if (string.IsNullOrWhiteSpace(_appDataDirectory) || !Directory.Exists(_appDataDirectory))
            {
                return;
            }

            var tries = 0;
            while (tries++ < 2)
                try
                {
                    Directory.Delete(_appDataDirectory, true);
                    return;
                }
                catch (IOException)
                {}
                catch (UnauthorizedAccessException)
                {}
        }

        protected static void CreateBlobContent(StandaloneAzureBlockBlob blob)
        {
            var buffer = Encoding.UTF8.GetBytes("Some content");
            blob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();
        }
    }
}