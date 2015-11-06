using System.IO;
using System.Text;

using LightBlue.Standalone;

using Xunit;

namespace LightBlue.Tests.Standalone.BlobStorage
{
    public class StandaloneAzureBlockBlobDeleteTests : StandaloneAzureTestsBase
    {
        public StandaloneAzureBlockBlobDeleteTests()
            : base(DirectoryType.Container)
        {}

        [Theory]
        [MemberData("BlobNames")]
        public void CanDeleteBlob(string blobName)
        {
            var buffer = Encoding.UTF8.GetBytes("File content");

            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);
            blob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();

            blob.Delete();

            Assert.False(File.Exists(blob.Uri.LocalPath));
        }

        [Theory]
        [MemberData("BlobNames")]
        public void CanDeleteBlobMetadata(string blobName)
        {
            var buffer = Encoding.UTF8.GetBytes("File content");

            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);
            blob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();
            blob.Metadata["thing"] = "something";
            blob.SetMetadata();

            blob.Delete();

            Assert.False(File.Exists(Path.Combine(BasePath, ".meta", blobName)));
        }
    }
}