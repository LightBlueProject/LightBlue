using System.Diagnostics;
using System.Globalization;
using System.IO;

using ExpectedObjects;

using LightBlue.Infrastructure;
using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureLocalResourceSourceTests : StandaloneAzureTestsBase
    {
        public StandaloneAzureLocalResourceSourceTests()
            : base(DirectoryType.Account)
        {}

        [Fact]
        public void CanRetreiveSettingsFromFile()
        {
            var source = new StandaloneAzureLocalResourceSource(
                "ServiceDefinition.csdef",
                "TestWebRole",
                BasePath,
                RoleEnvironmentExceptionCreatorFactory.BuildRoleEnvironmentExceptionCreator());

            new
            {
                MaximumSizeInMegabytes = 512,
                Name = "TestStorage",
                RootPath = Path.Combine(
                    BasePath,
                    ".resources",
                    "TestWebRole-" + Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture),
                    "TestStorage")
            }.ToExpectedObject().ShouldMatch(source["TestStorage"]);
        }

        [Fact]
        public void CanParseRoleWithNoResources()
        {
            Assert.DoesNotThrow(() => new StandaloneAzureLocalResourceSource(
                "ServiceDefinition.csdef",
                "TestWorkerRole",
                BasePath,
                RoleEnvironmentExceptionCreatorFactory.BuildRoleEnvironmentExceptionCreator()));
        }

        [Fact]
        public void RetreivingUnknownResourceThrowsRoleEnvironmentException()
        {
            var source = new StandaloneAzureLocalResourceSource(
                "ServiceDefinition.csdef",
                "TestWebRole",
                BasePath,
                RoleEnvironmentExceptionCreatorFactory.BuildRoleEnvironmentExceptionCreator());

            Assert.Throws<RoleEnvironmentException>(() => source["unknown"]);
        }

        [Fact]
        public void RetreivingUnknownResourceThrowsRoleEnvironmentExceptionForRoleWithNoResources()
        {
            var source = new StandaloneAzureLocalResourceSource(
                "ServiceDefinition.csdef",
                "TestWorkerRole",
                BasePath,
                RoleEnvironmentExceptionCreatorFactory.BuildRoleEnvironmentExceptionCreator());

            Assert.Throws<RoleEnvironmentException>(() => source["unknown"]);
        }
    }
}