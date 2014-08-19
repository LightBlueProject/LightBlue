using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ExpectedObjects;

using LightBlue.Standalone;

using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureBlockBlobTests : StandaloneAzureTestsBase
    {
        private readonly Uri _blobUri;
        private const string BlobName = "someblob";
        public StandaloneAzureBlockBlobTests()
            : base(DirectoryType.Container)
        {
            _blobUri = new Uri(Path.Combine(BasePath, BlobName));
        }

        [Fact]
        public void WillHaveCorrectValuesWhenGivenContainerDirectoryAndBlobName()
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, BlobName);

            new
            {
                Uri = _blobUri,
                Name = BlobName,
                Properties = new
                {
                    Length = (long) -1,
                    ContentType = (string) null
                },
                Metadata = new Dictionary<string, string>()
            }.ToExpectedObject().ShouldMatch(blob);
        }

        [Fact]
        public void WillHaveCorrectValuesWhenGivenUri()
        {
            var blob = new StandaloneAzureBlockBlob(_blobUri);

            new
            {
                Uri = _blobUri,
                Name = BlobName,
                Properties = new
                {
                    Length = (long) -1,
                    ContentType = (string) null
                },
                Metadata = new Dictionary<string, string>()
            }.ToExpectedObject().ShouldMatch(blob);
        }

        [Theory]
        [InlineData(0, 12, "File content")]
        [InlineData(5, 4, "cont")]
        public void CanUploadContentFromByteArray(int index, int count, string expectedContent)
        {
            var buffer = Encoding.UTF8.GetBytes("File content");

            var blob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            blob.UploadFromByteArrayAsync(buffer, index, count).Wait();

            Assert.Equal(expectedContent, File.ReadAllText(blob.Uri.LocalPath));
        }

        [Fact]
        public void CanUploadContentFromFile()
        {
            var sourceFilePath = Path.Combine(BasePath, "source");
            File.WriteAllText(sourceFilePath, "Source file");

            var blob = new StandaloneAzureBlockBlob(BasePath, BlobName);
            blob.UploadFromFileAsync(sourceFilePath);

            Assert.Equal("Source file", File.ReadAllText(blob.Uri.LocalPath));
        }

        [Fact]
        public void CanUploadContentFromStream()
        {
            using (var memoryStream = new MemoryStream())
            {
                var sourceContent = GenerateSourceContent();

                var buffer = Encoding.UTF8.GetBytes(sourceContent);
                memoryStream.Write(buffer, 0, buffer.Length);
                memoryStream.Position = 0;

                var blob = new StandaloneAzureBlockBlob(BasePath, BlobName);
                blob.UploadFromStreamAsync(memoryStream).Wait();

                Assert.Equal(sourceContent, File.ReadAllText(blob.Uri.LocalPath));
            }
        }

        private static string GenerateSourceContent()
        {
            var stringBuilder = new StringBuilder();
            for (var count = 0; count < 2048; count++)
            {
                stringBuilder.Append("Source content");
            }
            return stringBuilder.ToString();
        }
    }
}