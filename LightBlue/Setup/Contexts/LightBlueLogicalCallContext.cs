using System;
using System.Threading;
using LightBlue.Standalone;

namespace LightBlue.Setup.Contexts
{
    class LightBlueLogicalCallContext : LightBlueContextBase
    {
        private const string Key = "LightBlueLogicalCallContext";
        private readonly AsyncLocal<StandaloneConfiguration> _value = new AsyncLocal<StandaloneConfiguration>();
        
        public bool IsInitialized()
        {
            return _value != null;
            //return CallContext.LogicalGetData(Key) != null;
        }

        public StandaloneConfiguration Config
        {
            get
            {
                //var value = (StandaloneConfiguration)CallContext.LogicalGetData(Key); 
                if (_value == null) throw new InvalidOperationException("Logical call context has not been initialized for this thread.");
                return _value.Value;
            }
            set => _value.Value = value;
        }

        public void InitializeLogicalContext(string configurationPath, string roleName, bool useHostedStorage)
        {
            if (IsInitialized())
            {
                throw new InvalidOperationException("Logical call context has already been initialized for this thread.");
            }

            Config = new StandaloneConfiguration
            {
                ConfigurationPath = configurationPath,
                RoleName = roleName,
                UseHostedStorage = useHostedStorage,
            };
        }

        public override string RoleName => Config.RoleName;

        public override IAzureSettings Settings => new StandaloneAzureSettings(Config);
    }
}
