using System;
using System.IO;

using ExpectedObjects;

using LightBlue.Standalone;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneEnvironmentFixture
    {
        private readonly string _containerPath;

        public StandaloneEnvironmentFixture()
        {
            StandaloneEnvironment.SetStandardLightBlueDataDirectory();
            _containerPath = Path.Combine(
                StandaloneEnvironment.LightBlueDataDirectory,
                "dev",
                "blob",
                "testcontainer");
        }

        [Theory]
        [InlineData("randomblob")]
        [InlineData(@"various\path\elements\to\blob")]
        public void CanSplitBlobUri(string blobPath)
        {
            var blobUri = new Uri(Path.Combine(_containerPath, blobPath));

            var locationParts = StandaloneEnvironment.SeparateBlobUri(blobUri);

            new
            {
                ContainerPath = _containerPath,
                BlobPath = blobPath
            }.ToExpectedObject().ShouldMatch(locationParts);
        }

        [Fact]
        public void WillStripAccessToken()
        {
            var blobUri = new Uri(Path.Combine(_containerPath, "randomblob") + "?some=token");

            var locationParts = StandaloneEnvironment.SeparateBlobUri(blobUri);

            new
            {
                ContainerPath = _containerPath,
                BlobPath = "randomblob"
            }.ToExpectedObject().ShouldMatch(locationParts);
            
        }

        [Fact]
        public void ThrowsIfBlobUriIsNotAFileUri()
        {
            var blobUri = new Uri("http://www.abstractcode.com/");

            Assert.Throws<ArgumentException>(() => StandaloneEnvironment.SeparateBlobUri(blobUri));
        }

        [Fact]
        public void ThrowsIfBlobUriIsNotInLightBlueDataDirectoru()
        {
            var blobUri = new Uri("file:///c:/temp/");

            Assert.Throws<ArgumentException>(() => StandaloneEnvironment.SeparateBlobUri(blobUri));
        }
    }
}