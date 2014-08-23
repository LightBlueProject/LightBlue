using System;

namespace LightBlue.External
{
    public class ExternalAzureSettings : IAzureSettings
    {
        public string this[string index]
        {
            get
            {
                throw new InvalidOperationException("Cannot retrieve settings when running external to a host.");
            }
        }
    }
}