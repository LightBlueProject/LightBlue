using System;
using System.Runtime.Remoting.Messaging;
using LightBlue.Standalone;

namespace LightBlue.Setup.Contexts
{
    class LightBlueLogicalCallContext : LightBlueContextBase
    {
        private const string Key = "LightBlueLogicalCallContext";

        public bool IsInitialized()
        {
            return CallContext.LogicalGetData(Key) != null;
        }

        public StandaloneConfiguration Config
        {
            get
            {
                var value = (StandaloneConfiguration)CallContext.LogicalGetData(Key); 
                if(value == null) throw new InvalidOperationException("Logical call context has not been initialized for this thread.");
                return value;
            }
            set { CallContext.LogicalSetData(Key, value); }
        }

        public void InitializeLogicalContext(string configurationPath, string roleName, bool useHostedStorage)
        {
            if (IsInitialized()) throw new InvalidOperationException("Logical call context has already been initialized for this thread.");

            Config = new StandaloneConfiguration
            {
                ConfigurationPath = configurationPath,
                RoleName = roleName,
                UseHostedStorage = useHostedStorage,
            };
        }

        public override string RoleName
        {
            get { return Config.RoleName; }
        }

        public override IAzureSettings Settings
        {
            get { return new StandaloneAzureSettings(Config); }
        }
    }
}
