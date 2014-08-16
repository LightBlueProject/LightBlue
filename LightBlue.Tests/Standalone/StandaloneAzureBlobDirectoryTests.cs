using LightBlue.Standalone;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureBlobDirectoryTests : StandaloneAzureTestsBase
    {
        [Fact]
        public void WillHaveCorrectUriWhenGivenDirectory()
        {
            var directory = new StandaloneAzureBlobDirectory(BasePath);

            Assert.Equal(BasePathUri, directory.Uri);
        }
    }
}