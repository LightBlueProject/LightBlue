using System;
using System.IO;

using LightBlue.Standalone;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureBlobDirectoryTests : StandaloneAzureTestsBase
    {
        public StandaloneAzureBlobDirectoryTests()
            : base(DirectoryType.Container)
        {}

        [Fact]
        public void WillHaveCorrectUriWhenGivenDirectory()
        {
            var directoryPath = Path.Combine(BasePath, "directory");

            var directory = new StandaloneAzureBlobDirectory(directoryPath);

            Assert.Equal(new Uri(directoryPath), directory.Uri);
        }
    }
}