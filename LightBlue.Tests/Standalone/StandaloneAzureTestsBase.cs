using System;
using System.IO;
using System.Text;

using LightBlue.Standalone;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureTestsBase
    {
        protected const string SubPathElement = "test";

        protected readonly string BasePath;

        protected StandaloneAzureTestsBase()
        {
            BasePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        protected string SubPath
        {
            get { return Path.Combine(BasePath, SubPathElement); }
        }

        protected Uri BasePathUri
        {
            get {  return new Uri(BasePath); }
        }

        protected Uri SubPathUri
        {
            get { return new Uri(SubPath); }
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