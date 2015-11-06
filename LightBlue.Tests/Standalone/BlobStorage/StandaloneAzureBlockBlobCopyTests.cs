using System.Collections.Generic;
using System.IO;
using System.Text;

using ExpectedObjects;
using ExpectedObjects.Comparisons;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage.Blob;

using Xunit;

namespace LightBlue.Tests.Standalone.BlobStorage
{
    public class StandaloneAzureBlockBlobCopyTests : StandaloneAzureTestsBase
    {
        public StandaloneAzureBlockBlobCopyTests()
            : base(DirectoryType.Container)
        {}

        public static IEnumerable<object[]> CopyBlobNames
        {
            get
            {
                yield return new object[] { "source", "destination" };
                yield return new object[] { "source", @"destination\with\path" };
                yield return new object[] { "source", "destination/with/path/alternate" };
                yield return new object[] { @"source\with\path", "destination" };
                yield return new object[] { "source/with/path/alternate", "destination" };
                yield return new object[] { @"source\with\path", @"destination\with\path" };
                yield return new object[] { "source/with/path/alternate", @"destination\with\path" };
                yield return new object[] { "source/with/path/alternate", @"destination/with/path/alternate" };
            }
        }

        [Theory]
        [MemberData("CopyBlobNames")]
        public void CanCopyBlob(string source, string destination)
        {
            var buffer = Encoding.UTF8.GetBytes("File content");
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, source);
            sourceBlob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();

            var destinationBlob = new StandaloneAzureBlockBlob(BasePath, destination);
            destinationBlob.StartCopyFromBlob(sourceBlob);

            Assert.Equal("File content", File.ReadAllText(destinationBlob.Uri.LocalPath));
        }

        [Theory]
        [MemberData("CopyBlobNames")]
        public void CanOverwriteBlob(string source, string destination)
        {
            var sourceBuffer = Encoding.UTF8.GetBytes("File content");
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, source);
            sourceBlob.UploadFromByteArrayAsync(sourceBuffer, 0, sourceBuffer.Length).Wait();

            var originalContentBuffer = Encoding.UTF8.GetBytes("Original content");
            var destinationBlob = new StandaloneAzureBlockBlob(BasePath, destination);
            destinationBlob.UploadFromByteArrayAsync(originalContentBuffer, 0, originalContentBuffer.Length).Wait();
            destinationBlob.StartCopyFromBlob(sourceBlob);

            Assert.Equal("File content", File.ReadAllText(destinationBlob.Uri.LocalPath));
        }

        [Theory]
        [MemberData("CopyBlobNames")]
        public void WillNotCopyMetadataWhereItDoesNotExist(string source, string destination)
        {
            var buffer = Encoding.UTF8.GetBytes("File content");
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, source);
            sourceBlob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();

            var destinationBlob = new StandaloneAzureBlockBlob(BasePath, destination);
            destinationBlob.StartCopyFromBlob(sourceBlob);

            Assert.False(File.Exists(Path.Combine(BasePath, ".meta", destination)));
        }

        [Theory]
        [MemberData("CopyBlobNames")]
        public void WillCopyMetadataFromSourceWherePresent(string source, string destination)
        {
            var buffer = Encoding.UTF8.GetBytes("File content");
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, source);
            sourceBlob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();
            sourceBlob.Metadata["thing"] = "something";
            sourceBlob.Properties.ContentType = "whatever";
            sourceBlob.SetMetadata();
            sourceBlob.SetProperties();

            var destinationBlob = new StandaloneAzureBlockBlob(BasePath, destination);
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

        [Theory]
        [MemberData("CopyBlobNames")]
        public void WillRemoveExistingMetadataWhereSourceDoesNotHaveMetadata(string source, string destination)
        {
            var buffer = Encoding.UTF8.GetBytes("File content");
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, source);
            sourceBlob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();

            var originalContentBuffer = Encoding.UTF8.GetBytes("Original content");
            var destinationBlob = new StandaloneAzureBlockBlob(BasePath, destination);
            destinationBlob.UploadFromByteArrayAsync(originalContentBuffer, 0, originalContentBuffer.Length).Wait();
            destinationBlob.Metadata["thing"] = "other thing";
            destinationBlob.SetMetadata();
            destinationBlob.StartCopyFromBlob(sourceBlob);

            Assert.False(File.Exists(Path.Combine(BasePath, ".meta", destination)));
        }

        [Theory]
        [MemberData("BlobNames")]
        public void CopyStateIsNullBeforeCopy(string blobName)
        {
            var blob = new StandaloneAzureBlockBlob(BasePath, blobName);

            Assert.Null(blob.CopyState);
        }

        [Theory]
        [MemberData("CopyBlobNames")]
        public void CopyStateIsSuccessAfterCopy(string source, string destination)
        {
            var buffer = Encoding.UTF8.GetBytes("File content");
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, source);
            sourceBlob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();

            var destinationBlob = new StandaloneAzureBlockBlob(BasePath, destination);
            destinationBlob.StartCopyFromBlob(sourceBlob);

            Assert.Equal(CopyStatus.Success, destinationBlob.CopyState.Status);
        }

        [Theory]
        [MemberData("CopyBlobNames")]
        public void CopyStateIsFailedIfBlobLocked(string source, string destination)
        {
            var buffer = Encoding.UTF8.GetBytes("File content");
            var sourceBlob = new StandaloneAzureBlockBlob(BasePath, source);
            sourceBlob.UploadFromByteArrayAsync(buffer, 0, buffer.Length).Wait();

            using (new FileStream(sourceBlob.Uri.LocalPath, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                var destinationBlob = new StandaloneAzureBlockBlob(BasePath, destination);
                destinationBlob.StartCopyFromBlob(sourceBlob);

                new
                {
                    Status = CopyStatus.Failed,
                    StatusDescription = new NotNullComparison()
                }.ToExpectedObject().ShouldMatch(destinationBlob.CopyState);
            }
        }
    }
}