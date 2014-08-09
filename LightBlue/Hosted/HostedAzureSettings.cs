using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Hosted
{
    public class HostedAzureSettings : IAzureSettings
    {
        public string this[string index]
        {
            get { return RoleEnvironment.GetConfigurationSettingValue(index); }
        }
    }
}