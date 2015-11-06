using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ExpectedObjects;

using LightBlue.Standalone;

using Xunit;

namespace LightBlue.Tests.Standalone.BlobStorage
{
    public class StandaloneAzureBlockBlobTests : StandaloneAzureTestsBase
    {
        public StandaloneAzureBlockBlobTests()
            : base(DirectoryType.Container)
        {}

        [Theory]
        [MemberData("BlobNames")]
        public void WillHaveCorrectValuesWhenGivenContainerDirectoryAndBlobName(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);

            new
            {
                Uri = ToUri(blobName),
                Name = blobName,
                Properties = new
                {
                    Length = (long) -1,
                    ContentType = (string) null
                },
                Metadata = new Dictionary<string, string>()
            }.ToExpectedObject().ShouldMatch(blob);
        }

        [Theory]
        [MemberData("BlobNames")]
        public void CanUploadContentFromFullByteArray(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);
            blob.UploadFromByteArrayAsync(Encoding.UTF8.GetBytes("File content")).Wait();

            Assert.Equal("File content", File.ReadAllText(blob.Uri.LocalPath));
        }

        [Theory]
        [InlineData(0, 12, "File content", "randomblob")]
        [InlineData(5, 4, "cont", "randomblob")]
        [InlineData(0, 12, "File content", @"with\path\blob")]
        [InlineData(5, 4, "cont", @"with\path\blob")]
        [InlineData(0, 12, "File content", "with/alternate/separator")]
        [InlineData(5, 4, "cont", "with/alternate/separator")]
        public void CanUploadContentFromByteArrayWithRangeSpecifier(int index, int count, string expectedContent, string blobName)
        {
            var buffer = Encoding.UTF8.GetBytes("File content");

            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);
            blob.UploadFromByteArrayAsync(buffer, index, count).Wait();

            Assert.Equal(expectedContent, File.ReadAllText(blob.Uri.LocalPath));
        }

        [Theory]
        [MemberData("BlobNames")]
        public void CanUploadContentFromFile(string blobName)
        {
            var sourceFilePath = Path.Combine(BasePath, "source");
            File.WriteAllText(sourceFilePath, "Source file");

            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);
            blob.UploadFromFileAsync(sourceFilePath);

            Assert.Equal("Source file", File.ReadAllText(blob.Uri.LocalPath));
        }

        [Theory]
        [MemberData("BlobNames")]
        public void CanUploadContentFromStream(string blobName)
        {
            using (var memoryStream = new MemoryStream())
            {
                var sourceContent = GenerateSourceContent();

                var buffer = Encoding.UTF8.GetBytes(sourceContent);
                memoryStream.Write(buffer, 0, buffer.Length);
                memoryStream.Position = 0;

                var blob = new StandaloneAzureBlockBlob(BasePath, blobName);
                blob.UploadFromStreamAsync(memoryStream).Wait();

                Assert.Equal(sourceContent, File.ReadAllText(blob.Uri.LocalPath));
            }
        }

        [Fact]
        public void WillThrowIfConstructorContainerDirectoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StandaloneAzureBlockBlob(null, "blob"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void WillThrowIfConstructorContainerDirectoryIsEmpty(string containerDirectory)
        {
            Assert.Throws<ArgumentException>(() => new StandaloneAzureBlockBlob(containerDirectory, "blob"));
        }

        [Fact]
        public void WillThrowIfConstructorBlobNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StandaloneAzureBlockBlob(BasePath, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void WillThrowIfConstructorBlobNameIsEmpty(string blobName)
        {
            Assert.Throws<ArgumentException>(() => new StandaloneAzureBlockBlob(BasePath, blobName));
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

        public Uri ToUri(string path)
        {
            return new Uri(Path.Combine(BasePath, path));
        }
    }
}