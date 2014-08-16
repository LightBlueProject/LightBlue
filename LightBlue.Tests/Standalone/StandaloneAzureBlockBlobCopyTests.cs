using System.Collections.Generic;
using System.IO;
using System.Text;

using ExpectedObjects;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage.Blob;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureBlockBlobCopyTests : StandaloneAzureTestsBase
    {
        public StandaloneAzureBlockBlobCopyTests()
        {
            Directory.CreateDirectory(Path.Combine(BasePath, ".meta"));
        }

        [Fact]
        public void CanCopyBlob()
        {
            var buffer = Encoding.UTF8.GetBytes("File content");
            var sourceBlob = new StandaloneAzureBlockBlob(SubPathUri);
            sourceBlob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();

            var destinationBlob = new StandaloneAzureBlockBlob(BasePath, "destination");
            destinationBlob.StartCopyFromBlob(sourceBlob);

            Assert.Equal("File content", File.ReadAllText(destinationBlob.Uri.LocalPath));
        }

        [Fact]
        public void WillNotCopyMetadataWhereItDoesNotExist()
        {
            var buffer = Encoding.UTF8.GetBytes("File content");
            var sourceBlob = new StandaloneAzureBlockBlob(SubPathUri);
            sourceBlob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();

            var destinationBlob = new StandaloneAzureBlockBlob(BasePath, "destination");
            destinationBlob.StartCopyFromBlob(sourceBlob);

            Assert.False(File.Exists(Path.Combine(BasePath, ".meta", "destination")));
        }

        [Fact]
        public void WillCopyMetadataFromSourceWherePresent()
        {
            var buffer = Encoding.UTF8.GetBytes("File content");
            var sourceBlob = new StandaloneAzureBlockBlob(SubPathUri);
            sourceBlob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();
            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.Properties.ContentType = "whatever";
            sourceBlob.SetMetadata();
            sourceBlob.SetProperties();

            var destinationBlob = new StandaloneAzureBlockBlob(BasePath, "destination");
            destinationBlob.StartCopyFromBlob(sourceBlob);
            destinationBlob.FetchAttributes();

            new
            {
                Metadata = new Dictionary<string, string>
                {
                    {"thing", "something"}
                },
                Properties = new
                {
                    ContentType = "whatever",
                    Length = (long) 12
                }
            }.ToExpectedObject().ShouldMatch(destinationBlob);
        }

        [Fact]
        public void CopyStateIsNullBeforeCopy()
        {
            var blob = new StandaloneAzureBlockBlob(SubPathUri);

            Assert.Null(blob.CopyState);
        }

        [Fact]
        public void CopyStateIsSuccessAfterCopy()
        {
            var buffer = Encoding.UTF8.GetBytes("File content");
            var sourceBlob = new StandaloneAzureBlockBlob(SubPathUri);
            sourceBlob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();

            var destinationBlob = new StandaloneAzureBlockBlob(BasePath, "destination");
            destinationBlob.StartCopyFromBlob(sourceBlob);
        }
    }
}