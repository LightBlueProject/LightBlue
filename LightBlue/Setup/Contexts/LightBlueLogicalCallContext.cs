using System;
using System.Collections.Concurrent;
using System.Threading;
using LightBlue.Standalone;

namespace LightBlue.Setup.Contexts
{
    class LightBlueLogicalCallContext : LightBlueContextBase
    {
        private const string Key = "LightBlueLogicalCallContext";
        private static readonly ConcurrentDictionary<string, AsyncLocal<object>> _state = new ConcurrentDictionary<string, AsyncLocal<object>>();
        private static void LogicalSetData(string name, object data) => _state.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;
        private static object LogicalGetData(string name) => _state.TryGetValue(name, out AsyncLocal<object> data) ? data.Value : null;

        public bool IsInitialized()
        {
            return LogicalGetData(Key) != null;
        }

        public StandaloneConfiguration Config
        {
            get
            {
                var value = (StandaloneConfiguration)LogicalGetData(Key); 
                if(value == null) throw new InvalidOperationException("Logical call context has not been initialized for this thread.");
                return value;
            }
            set { LogicalSetData(Key, value); }
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
