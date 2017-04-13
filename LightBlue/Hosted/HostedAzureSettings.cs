using Microsoft.WindowsAzure;


namespace LightBlue.Hosted
{
    public class HostedAzureSettings : IAzureSettings
    {
        public string this[string index]
        {
            get { return CloudConfigurationManager.GetSetting(index); }
        }
    }
}