using System.Diagnostics;

namespace LightBlue.Hosted
{
    public class HostedAzureRoleInformation : IAzureRoleInformation
    {
        public string Name => Process.GetCurrentProcess().ProcessName;
    }
}