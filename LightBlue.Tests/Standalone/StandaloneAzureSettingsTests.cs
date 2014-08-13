using LightBlue.Infrastructure;
using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureSettingsTests
    {
        [Fact]
        public void CanRetrieveSettingFromFile()
        {
            var settings = new StandaloneAzureSettings(
                "ServiceConfiguration.Local.cscfg",
                "TestWorkerRole",
                RoleEnvironmentExceptionCreatorFactory.BuildRoleEnvironmentExceptionCreator());

            Assert.Equal("Running locally", settings["RandomSetting"]);
        }

        [Fact]
        public void WillThrowOnUnknownKey()
        {
            var settings = new StandaloneAzureSettings(
                "ServiceConfiguration.Local.cscfg",
                "TestWorkerRole",
                RoleEnvironmentExceptionCreatorFactory.BuildRoleEnvironmentExceptionCreator());

            Assert.Throws<RoleEnvironmentException>(() => settings["Unknown"]);
        }
    }
}