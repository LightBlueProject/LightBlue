using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Hosted
{
    public class HostedAzureEnvironmentSource : IAzureEnvironmentSource
    {
        public AzureEnvironment CurrentEnvironment
        {
            get { return RoleEnvironment.IsEmulated ? AzureEnvironment.Emulator : AzureEnvironment.ActualAzure; }
        }
    }
}