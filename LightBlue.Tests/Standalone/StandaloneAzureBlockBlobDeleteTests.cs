using System.IO;
using System.Text;

using LightBlue.Standalone;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureBlockBlobDeleteTests : StandaloneAzureTestsBase
    {
        private const string BlobName = "someblob";

        public StandaloneAzureBlockBlobDeleteTests()
            : base(DirectoryType.Container)
        {}

        [Fact]
        public void CanDeleteBlob()
        {
            var buffer = Encoding.UTF8.GetBytes("File content");

            var blob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            blob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();

            blob.Delete();

            Assert.False(File.Exists(blob.Uri.LocalPath));
        }

        [Fact]
        public void CanDeleteBlobMetadata()
        {
            var buffer = Encoding.UTF8.GetBytes("File content");

            var blob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            blob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();
            blob.Metadata["thing"] = "something";
            blob.SetMetadata();

            blob.Delete();

            Assert.False(File.Exists(Path.Combine(BasePath, ".meta", BlobName)));
        }
    }
}