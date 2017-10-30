using System;

namespace LightBlue.Setup.Contexts
{
    class ExternalLightBlueContext : LightBlueContextBase
    {
        public override string RoleName
        {
            get { return "External (LightBlueContext)"; }
        }

        public override IAzureSettings Settings
        {
            get { throw new NotSupportedException("Azure Settings are not supported in an external (non-hosted or emulated) environment."); }
        }
    }
}