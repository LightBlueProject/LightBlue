using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Hosted
{
    public class HostedAzureRoleInformation : IAzureRoleInformation
    {
        public string Name
        {
            get { return RoleEnvironment.CurrentRoleInstance.Role.Name; }
        }
    }
}