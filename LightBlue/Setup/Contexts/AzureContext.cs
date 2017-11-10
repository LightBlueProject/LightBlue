using System.Diagnostics;
using LightBlue.Hosted;

namespace LightBlue.Setup.Contexts
{
    class AzureContext : AzureContextBase
    {
        private readonly HostedAzureSettings _settings;

        public AzureContext()
            : base(AzureEnvironment.Azure)
        {
            RoleName = Process.GetCurrentProcess().ProcessName;
            _settings = new HostedAzureSettings();
        }

        public override string RoleName { get; }

        public override IAzureSettings Settings
        {
            get { return _settings; }
        }
    }
}