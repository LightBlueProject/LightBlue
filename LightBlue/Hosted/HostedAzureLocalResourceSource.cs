using LightBlue.Standalone;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Hosted
{
    public class HostedAzureLocalResourceSource : IAzureLocalResourceSource
    {
        public IAzureLocalResource this[string index]
        {
            get
            {
                if (RoleEnvironment.IsAvailable)
                {
                    return new HostedAzureLocalResource(RoleEnvironment.GetLocalResource(index));
                }
            
                return new StandaloneAzureLocalResource();
             
            }
        }
    }
}