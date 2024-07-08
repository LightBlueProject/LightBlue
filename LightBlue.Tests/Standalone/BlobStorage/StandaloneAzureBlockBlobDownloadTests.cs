using System.IO;
using System.Text;
using System.Threading.Tasks;

using LightBlue.Standalone;

using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Standalone.BlobStorage
{
    public class StandaloneAzureBlockBlobDownloadTests : StandaloneAzureTestsBase
    {
        public StandaloneAzureBlockBlobDownloadTests()
            : base(DirectoryType.Container)
        {}

        [Theory]
        [MemberData(nameof(BlobNames))]
        public async Task CanDownloadBlobToStream(string blobName)
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
        [MemberData(nameof(BlobNames))]
        public async Task CanDownloadBlobToStreamAsync(string blobName)
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

        [Theory]
        [MemberData(nameof(BlobNames))]
        public async Task CanOpenBlobStream(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);
            blob.Properties.ContentType = "customcontenttype";
            await blob.UploadFromByteArrayAsync(Encoding.UTF8.GetBytes("Streamable content"));

            var readBlob = new StandaloneAzureBlockBlob(BasePath, blobName);

            using (var stream = readBlob.OpenRead())
            {
                using (var streamReader = new StreamReader(stream))
                {
                    Assert.Equal("Streamable content", streamReader.ReadToEnd());
                }
                
                Assert.Equal("customcontenttype", readBlob.Properties.ContentType);
            }
        }
    }
}