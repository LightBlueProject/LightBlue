using System.Configuration;

namespace LightBlue.Hosted
{
    public class HostedAzureSettings : IAzureSettings
    {
        public string this[string index] => ConfigurationManager.AppSettings[index];
    }
}