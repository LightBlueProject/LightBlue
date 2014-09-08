using System;
using System.IO;

using LightBlue.Standalone;

using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureBlobDirectoryTests : StandaloneAzureTestsBase
    {
        public StandaloneAzureBlobDirectoryTests()
            : base(DirectoryType.Container)
        {}

        [Theory]
        [InlineData("directory")]
        [InlineData(@"directory\subdirectory")]
        [InlineData("directory/subdirectory")]
        public void WillHaveCorrectUriWhenGivenDirectory(string directoryName)
        {
            var directoryPath = Path.Combine(BasePath, directoryName);

            var directory = new StandaloneAzureBlobDirectory(BasePath, directoryName);

            Assert.Equal(new Uri(directoryPath), directory.Uri);
        }
    }
}