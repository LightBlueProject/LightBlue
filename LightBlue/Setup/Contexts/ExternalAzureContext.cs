using System;

namespace LightBlue.Setup.Contexts
{
    class ExternalAzureContext : AzureContextBase
    {
        public override string RoleName
        {
            get { return "External (AzureContext)"; }
        }

        public override IAzureSettings Settings
        {
            get { throw new NotSupportedException("Azure Settings are not supported in an external (non-hosted or emulated) environment."); }
        }

        public override IAzureLocalResourceSource LocalResources
        {
            get { throw new NotSupportedException("Azure Local Resources are not supported in an external (non-hosted or emulated) environment."); }
        }
    }
}