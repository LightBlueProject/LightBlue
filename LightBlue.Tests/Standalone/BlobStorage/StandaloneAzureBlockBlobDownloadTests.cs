using System.IO;
using System.Text;
using System.Threading.Tasks;

using LightBlue.Standalone;

using Xunit;

namespace LightBlue.Tests.Standalone.BlobStorage
{
    public class StandaloneAzureBlockBlobDownloadTests : StandaloneAzureTestsBase
    {
        public StandaloneAzureBlockBlobDownloadTests()
            : base(DirectoryType.Container)
        {}

        [Theory]
        [MemberData("BlobNames")]
        public async Task CanDownloadBlboToStream(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);
            await blob.UploadFromByteArrayAsync(Encoding.UTF8.GetBytes("Streamable content"));

            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                memoryStream.Position = 0;

                using (var streamReader = new StreamReader(memoryStream))
                {
                    Assert.Equal("Streamable content", streamReader.ReadToEnd());
                }
            }
        }

        [Theory]
        [MemberData("BlobNames")]
        public async Task CanDownloadBlboToStreamAsync(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);
            await blob.UploadFromByteArrayAsync(Encoding.UTF8.GetBytes("Streamable content"));

            using (var memoryStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(memoryStream);
                memoryStream.Position = 0;

                using (var streamReader = new StreamReader(memoryStream))
                {
                    Assert.Equal("Streamable content", streamReader.ReadToEnd());
                }
            }
        }
    }
}