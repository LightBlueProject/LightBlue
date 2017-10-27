using LightBlue.Standalone;

namespace LightBlue.Setup.Contexts
{
    class LightBlueAppDomainContext : LightBlueContextBase
    {
        private readonly string _roleName;
        private readonly StandaloneAzureSettings _settings;

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
        }

        public override string RoleName
        {
            get { return _roleName; }
        }

        public override IAzureSettings Settings
        {
            get { return _settings; }
        }
    }
}