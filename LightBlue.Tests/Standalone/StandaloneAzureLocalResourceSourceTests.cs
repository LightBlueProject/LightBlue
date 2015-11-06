using System.Diagnostics;
using System.Globalization;
using System.IO;

using ExpectedObjects;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureLocalResourceSourceTests : StandaloneAzureTestsBase
    {
        private readonly StandaloneConfiguration _standaloneConfiguration;

        public StandaloneAzureLocalResourceSourceTests()
            : base(DirectoryType.Account)
        {
            _standaloneConfiguration = new StandaloneConfiguration
            {
                ServiceDefinitionPath = "ServiceDefinition.csdef",
                RoleName = "TestWebRole"
            };
        }

        [Fact]
        public void CanRetreiveSettingsFromFile()
        {
            var source = new StandaloneAzureLocalResourceSource(_standaloneConfiguration, BasePath);

            new
            {
                MaximumSizeInMegabytes = 512,
                Name = "TestStorage",
                RootPath = DetermineResourceDirectoryPath("TestStorage")
            }.ToExpectedObject().ShouldMatch(source["TestStorage"]);
        }

        [Fact]
        public void ResourceDirectoryCreatedOnRetreival()
        {
            var source = new StandaloneAzureLocalResourceSource(_standaloneConfiguration, BasePath);

            var resource = source["TestStorage"];

            Assert.True(Directory.Exists(resource.RootPath));
        }

        [Fact]
        public void CanRetrieveTheResourceDirectoruRepeatedly()
        {
            var source = new StandaloneAzureLocalResourceSource(_standaloneConfiguration, BasePath);

            var resource = source["TestStorage"];

            resource.ToExpectedObject().ShouldMatch(source["TestStorage"]);
        }

        [Fact]
        public void ResourceDirectoryDoesNotExistUntilRetreival()
        {
            new StandaloneAzureLocalResourceSource(_standaloneConfiguration, BasePath);

            Assert.False(Directory.Exists(DetermineResourceDirectoryPath("TestStorage")));
        }

        [Fact]
        public void CanParseRoleWithNoResources()
        {
            new StandaloneAzureLocalResourceSource(new StandaloneConfiguration
            {
                ServiceDefinitionPath = "ServiceDefinition.csdef",
                RoleName = "TestWorkerRole"
            }, 
            BasePath);
        }

        [Fact]
        public void RetreivingUnknownResourceThrowsRoleEnvironmentException()
        {
            var source = new StandaloneAzureLocalResourceSource(_standaloneConfiguration, BasePath);

            Assert.Throws<RoleEnvironmentException>(() => source["unknown"]);
        }

        [Fact]
        public void RetreivingUnknownResourceThrowsRoleEnvironmentExceptionForRoleWithNoResources()
        {
            var source = new StandaloneAzureLocalResourceSource(_standaloneConfiguration, BasePath);

            Assert.Throws<RoleEnvironmentException>(() => source["unknown"]);
        }

        private string DetermineResourceDirectoryPath(string resourceName)
        {
            return Path.Combine(
                BasePath,
                ".resources",
                "TestWebRole-" + Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture),
                resourceName);
        }
    }
}