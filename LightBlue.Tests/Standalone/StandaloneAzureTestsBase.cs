using System;
using System.IO;
using System.Text;

using LightBlue.Standalone;

namespace LightBlue.Tests.Standalone
{
    public enum DirectoryType
    {
        Account = 0,
        Container = 1,
    }

    public abstract class StandaloneAzureTestsBase
    {
        public const string MetadataDirectory = ".meta";

        protected readonly string BasePath;

        protected StandaloneAzureTestsBase(DirectoryType directoryType)
        {
            BasePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(BasePath);

            if (directoryType == DirectoryType.Container)
            {
                Directory.CreateDirectory(Path.Combine(BasePath, MetadataDirectory));
            }
        }

        protected Uri BasePathUri
        {
            get {  return new Uri(BasePath); }
        }

        public void Dispose()
        {
            if (!string.IsNullOrWhiteSpace(BasePath) && Directory.Exists(BasePath))
            {
                Directory.Delete(BasePath, true);
            }
        }

        protected static void CreateBlobContent(StandaloneAzureBlockBlob blob)
        {
            var buffer = Encoding.UTF8.GetBytes("Some content");
            blob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();
        }
    }
}