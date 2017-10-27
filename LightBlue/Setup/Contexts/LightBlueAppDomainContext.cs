using LightBlue.Standalone;

namespace LightBlue.Setup.Contexts
{
    class LightBlueAppDomainContext : LightBlueContextBase
    {
        private readonly string _roleName;
        private readonly StandaloneAzureSettings _settings;
        private readonly StandaloneAzureLocalResourceSource _localResources;

        public LightBlueAppDomainContext(string configurationPath, string roleName, bool useHostedStorage)
        {
            _roleName = roleName;

            var configuration = new StandaloneConfiguration
            {
                ConfigurationPath = configurationPath,
                RoleName = roleName,
                UseHostedStorage = useHostedStorage,
            };

            _settings = new StandaloneAzureSettings(configuration);
            _localResources = new StandaloneAzureLocalResourceSource(configuration, StandaloneEnvironment.LightBlueDataDirectory);
        }

        public override string RoleName
        {
            get { return _roleName; }
        }

        public override IAzureSettings Settings
        {
            get { return _settings; }
        }

        public override IAzureLocalResourceSource LocalResources
        {
            get { return _localResources; }
        }
    }
}