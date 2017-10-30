using System;
using LightBlue.Hosted;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Setup.Contexts
{
    class AzureContext : AzureContextBase
    {
        private readonly string _roleName;
        private readonly HostedAzureSettings _settings;

        public AzureContext()
            : base(RoleEnvironment.IsAvailable && RoleEnvironment.IsEmulated
                ? AzureEnvironment.Emulator
                : AzureEnvironment.ActualAzure)
        {
            _roleName = (RoleEnvironment.IsAvailable) ? RoleEnvironment.CurrentRoleInstance.Role.Name : String.Empty;
            _settings = new HostedAzureSettings();
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