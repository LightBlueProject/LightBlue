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
        public StandaloneAzureBlockBlobTests()
        {
            Directory.CreateDirectory(BasePath);
        }

        [Fact]
        public void WillHaveCorrectValuesWhenGivenContainerDirectoryAndBlobName()
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, SubPathElement);

            new
            {
                Uri = SubPathUri,
                Name = SubPathElement,
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
            var blob = new StandaloneAzureBlockBlob(SubPathUri);

            new
            {
                Uri = SubPathUri,
                Name = SubPathElement,
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

            var blob = new StandaloneAzureBlockBlob(SubPathUri);
            blob.UploadFromByteArrayAsync(buffer, index, count).Wait();

            Assert.Equal(expectedContent, File.ReadAllText(blob.Uri.LocalPath));
        }

        [Fact]
        public void CanUploadContentFromFile()
        {
            var sourceFilePath = Path.Combine(BasePath, "source");
            File.WriteAllText(sourceFilePath, "Source file");

            var blob = new StandaloneAzureBlockBlob(SubPathUri);
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

                var blob = new StandaloneAzureBlockBlob(SubPathUri);
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