using LightBlue.Standalone;

namespace LightBlue.Setup
{
    public class EnvironmentDefinition
    {
        private readonly AzureEnvironment _azureEnvironment;
        private readonly HostingType _hostingType;
        private readonly StandaloneConfiguration _standaloneConfiguration;

        public EnvironmentDefinition(
            AzureEnvironment azureEnvironment,
            HostingType hostingType,
            StandaloneConfiguration standaloneConfiguration)
        {
            _azureEnvironment = azureEnvironment;
            _hostingType = hostingType;
            _standaloneConfiguration = standaloneConfiguration;
        }

        public AzureEnvironment AzureEnvironment
        {
            get { return _azureEnvironment; }
        }

        public HostingType HostingType
        {
            get { return _hostingType; }
        }

        public StandaloneConfiguration StandaloneConfiguration
        {
            get { return _standaloneConfiguration; }
        }
    }
}