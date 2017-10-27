using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;

using Xunit;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureSettingsTests
    {
        private readonly StandaloneAzureSettings _settings;

        public StandaloneAzureSettingsTests()
        {
            _settings = new StandaloneAzureSettings(new StandaloneConfiguration
            {
                ConfigurationPath = "testservice.dll.config"
            });
        }

        [Fact]
        public void CanRetrieveSettingFromFile()
        {
            var settings = new StandaloneAzureSettings(new StandaloneConfiguration
            {
                ConfigurationPath = "testservice.dll.config"
            });

            Assert.Equal("Running locally", settings["RandomSetting"]);
        }

        [Fact]
        public void WillThrowOnUnknownKey()
        {
            Assert.Throws<RoleEnvironmentException>(() => _settings["Unknown"]);
        }
    }
}